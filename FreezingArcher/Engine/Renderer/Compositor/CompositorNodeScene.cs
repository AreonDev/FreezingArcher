//
//  EmptyClass.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using System.Collections.Generic;
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;
using FreezingArcher.Output;

using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorNodeScene : CompositorNode, IMessageConsumer
    {
        private class RCActionTextureResize : RendererCore.RCAction
        {
            Texture2D _FrameBufferOutputTexture;
            Texture2D _FrameBufferColorTexture;
            Texture2D _FrameBufferDepthTexture;
            Texture2D _FrameBufferNormalTexture;
            Texture2D _FrameBufferSpecularTexture;
            TextureDepthStencil _FrameBufferDepthStencilTexture;

            int Width;
            int Height;

            public RCActionTextureResize(Texture2D frameBufferOutputTexture,
                Texture2D frameBufferColorTexture,
                Texture2D frameBufferDepthTexture,
                Texture2D frameBufferNormalTexture,
                Texture2D frameBufferSpecularTexture,
                TextureDepthStencil frameBufferDepthStencilTexture, int width, int height)
            {
                _FrameBufferColorTexture = frameBufferColorTexture;
                _FrameBufferDepthStencilTexture = frameBufferDepthStencilTexture;
                _FrameBufferDepthTexture = frameBufferDepthTexture;
                _FrameBufferNormalTexture = frameBufferNormalTexture;
                _FrameBufferOutputTexture = frameBufferOutputTexture;
                _FrameBufferSpecularTexture = frameBufferSpecularTexture;

                Width = width;
                Height = height;
            }

            public RendererCore.RCActionDelegate Action
            {
                get
                {
                    return delegate()
                    {
                        _FrameBufferColorTexture.Resize(Width, Height);
                        _FrameBufferDepthStencilTexture.Resize(Width, Height);
                        _FrameBufferDepthTexture.Resize(Width, Height);
                        _FrameBufferNormalTexture.Resize(Width, Height);
                        _FrameBufferOutputTexture.Resize(Width, Height);
                        _FrameBufferSpecularTexture.Resize(Width, Height);
                    };
                }
            }
        }

        private int TextureDiffuseLocation = 0;
        private int TexturePositionLocation = 0;
        private int TextureNomalLocation = 0;
        private int TextureSpecularLocation = 0;
        private int DistanceFogIntensityLocation = 0;
        private int DistanceFogColorLocation = 0;
        private int CameraPositionLocation = 0;        

        private struct LightLocationStruct
        {
            public int TypeLocation;
            public int OnLocation;
            public int LightColorLocation;
            public int DirectionalLightDirectionLocation;
            public int PointLightPositionLocation;
            public int PointLightConstantAttLocation;
            public int PointLightLinearAttLocation;
            public int PointLightExpAttLocation;
            public int SpotLightConeAngleLocation;
            public int SpotLightConeCosineLocation;
        }

        private int AmbientColorLocation = 0;
        private int AmbientIntensityLocation = 0;

        private LightLocationStruct[] LightLocations = new LightLocationStruct[16];


        Texture2D FrameBufferOutputTexture;
        Texture2D FrameBufferColorTexture;
        Texture2D FrameBufferDepthTexture;
        Texture2D FrameBufferNormalTexture;
        Texture2D FrameBufferSpecularTexture;
        TextureDepthStencil FrameBufferDepthStencilTexture;

        Effect NoNodeEffect;

        public CoreScene Scene { get; set;}

        public CompositorNodeScene(RendererContext rc, MessageProvider prov) : base("NodeStart", rc, prov)
        {
            PostRenderingObjects = new List<SceneObject>();

            ValidMessages = new[] { (int)MessageId.WindowResize };
            prov += this;
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void Draw()
        {
            PrivateRendererContext.Clear(Color4.Transparent);

            if (Scene != null)
            {
                if (Scene.Active)
                {
                    PrivateRendererContext.EnableDepthTest(true);
                    PrivateRendererContext.EnableDepthMaskWriting(true);

                    DrawScene();

                    //Setup deferred Shading!
                    PrivateRendererContext.EnableDepthMaskWriting(false);
                    PrivateRendererContext.EnableDepthTest(false);

                    ShadeScene();

                    //Now render all PostRendering objects!
                    PrivateRendererContext.EnableDepthTest(true);
                    DrawPostSceneObjects();

                    PrivateRendererContext.EnableDepthMaskWriting(true);
                }
            }
        }

        private List<SceneObject> PostRenderingObjects;

        private void ShadeScene()
        {
            Sprite spr = new Sprite();
            spr.Init(FrameBufferColorTexture);

            spr.CustomEffect = true;

            FrameBufferDepthTexture.Bind(2);
            FrameBufferNormalTexture.Bind(3);
            FrameBufferSpecularTexture.Bind(4);

            NoNodeEffect.BindPipeline();

            NoNodeEffect.PixelProgram.SetUniform(TextureDiffuseLocation, 1);
            NoNodeEffect.PixelProgram.SetUniform(TexturePositionLocation, 2);
            NoNodeEffect.PixelProgram.SetUniform(TextureNomalLocation, 3);
            NoNodeEffect.PixelProgram.SetUniform(TextureSpecularLocation, 4);

            NoNodeEffect.PixelProgram.SetUniform(DistanceFogIntensityLocation, Scene.DistanceFogIntensity);
            NoNodeEffect.PixelProgram.SetUniform(DistanceFogColorLocation, Scene.DistanceFogColor);

            if (Scene != null && Scene.CameraManager.ActiveCamera != null)
                NoNodeEffect.PixelProgram.SetUniform(CameraPositionLocation, Scene.CameraManager.ActiveCamera.Position);

            //Setup lights
            NoNodeEffect.PixelProgram.SetUniform(AmbientIntensityLocation, Scene.AmbientIntensity);
            NoNodeEffect.PixelProgram.SetUniform(AmbientColorLocation, Scene.AmbientColor);

            List<Light> Lights = (Scene != null) ? Scene.Lights : null;

            if (Lights != null)
            {
                for (int i = 0; i < Lights.Count; i++)
                {
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].TypeLocation, (int)Lights[i].Type);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].OnLocation, Lights[i].On ? 1 : 0);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].LightColorLocation, Lights[i].Color);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].DirectionalLightDirectionLocation, Lights[i].DirectionalLightDirection);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].PointLightPositionLocation, Lights[i].PointLightPosition);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].PointLightConstantAttLocation, Lights[i].PointLightConstantAttenuation);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].PointLightLinearAttLocation, Lights[i].PointLightLinearAttenuation);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].PointLightExpAttLocation, Lights[i].PointLightExponentialAttenuation);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].SpotLightConeAngleLocation, Lights[i].SpotLightConeAngle);
                    NoNodeEffect.PixelProgram.SetUniform(LightLocations[i].SpotLightConeCosineLocation, Lights[i].SpotLightConeCosine);
                }
            }

            PrivateRendererContext.DrawSpriteAbsolute(spr, 1);

            NoNodeEffect.UnbindPipeline();
        }

        private void DrawPostSceneObjects()
        {
            foreach (SceneObject obj in PostRenderingObjects)
            {
                if (obj.Enabled)
                {
                    obj.Update();

                    obj.Draw(PrivateRendererContext);

                    RendererErrorCode err_code = PrivateRendererContext.GetError();
                    if (err_code != RendererErrorCode.NoError)
                        obj.ErrorCount++;

                    if (obj.ErrorCount > 5)
                    {
                        Logger.Log.AddLogEntry(LogLevel.Error, "RendererContext", FreezingArcher.Core.Status.DeveloperWasDrunk,
                            "Too many errors on Object " + obj.GetName() + "\nDelete object!");
                        Scene.RemoveObject(obj);
                    }
                }
            }

            PostRenderingObjects.Clear();
        }

        private void DrawScene()
        {
            Scene.Update();

            /*
                    if (Scene.FrameBufferDepthStencilTexture.Width != ViewportSize.X ||
                        Scene.FrameBufferDepthStencilTexture.Height != ViewportSize.Y)
                        Scene.ResizeTextures(ViewportSize.X, ViewportSize.Y);*/

            /*Scene.FrameBuffer.UseAttachments(new FrameBuffer.AttachmentUsage[]
                    {FrameBuffer.AttachmentUsage.Color0,
                        FrameBuffer.AttachmentUsage.Color1, FrameBuffer.AttachmentUsage.Color2,
                        FrameBuffer.AttachmentUsage.Color3
                    });*/

            //Scene.FrameBuffer.Bind(FrameBuffer.FrameBufferTarget.Draw);

            //Clear scene
            /*PrivateRendererContext.Clear(Scene.BackgroundColor, 0);
                    PrivateRendererContext.Clear(Scene.BackgroundColor, 1);
                    PrivateRendererContext.Clear(Color4.Transparent, 2);
                    PrivateRendererContext.Clear(Color4.Transparent, 3);
                    PrivateRendererContext.Clear(Color4.Transparent, 4);
                    */

            PrivateRendererContext.Clear(Scene.BackgroundColor);

            foreach (SceneObject obj in Scene.GetObjectsSorted())
            {
                if (obj.Enabled)
                {
                    //Find name for SceneObjectArray....
                    bool array = (obj.GetName() == "SceneObjectArray");

                    Vector3 dir = obj.Position - Scene.CameraManager.ActiveCamera.Position;
                    float length = dir.Length;

                    if (obj.Priority <= 5000)
                        obj.Update();

                    if (array || (length <= Scene.MaxRenderingDistance))
                    {
                        if (obj.Priority > 5000)
                        {
                            PostRenderingObjects.Add(obj);
                            continue;
                        }

                        obj.Draw(PrivateRendererContext);

                        RendererErrorCode err_code = PrivateRendererContext.GetError();
                        if (err_code != RendererErrorCode.NoError)
                            obj.ErrorCount++;

                        if (obj.ErrorCount > 5)
                        {
                            Logger.Log.AddLogEntry(LogLevel.Error, "RendererContext", FreezingArcher.Core.Status.DeveloperWasDrunk,
                                "Too many errors on Object " + obj.GetName() + "\nDelete object!");
                            Scene.RemoveObject(obj);
                        }
                    }
                }
            }
            //Scene.FrameBuffer.Unbind();

            //Now.... use Funny stupid Deferred Shading shader

            //Sprite spr = new Sprite();
            //spr.Init(Scene.FrameBufferColorTexture);
            //spr.AbsolutePosition = new Vector2(0.0f, 0.0f);
            //spr.Scaling = new Vector2(1, 1);

            //DrawSpriteAbsolute(spr);
        }

        public override void End()
        {
            base.End();
        }

        public override void InitOtherStuff()
        {    
            long ticks = DateTime.Now.Ticks;

            FrameBufferNormalTexture = PrivateRendererContext.CreateTexture2D("CoreSceneFrameBufferNormalTexture_"+ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, false);

            FrameBufferColorTexture = PrivateRendererContext.CreateTexture2D("CoreSceneFrameBufferColorTexture_" + ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, false);

            FrameBufferSpecularTexture = PrivateRendererContext.CreateTexture2D("CoreSceneFrameBufferSpecularTexture_" + ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, false);

            FrameBufferDepthTexture = PrivateRendererContext.CreateTexture2D("CoreSceneFrameBufferDepthTexture_" + ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, true);

            FrameBufferOutputTexture = PrivateRendererContext.CreateTexture2D("CoreSceneFrameBufferOutputTexture_" + ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, false);

            FrameBufferDepthStencilTexture = PrivateRendererContext.CreateTextureDepthStencil("CoreSceneFrameBufferDepthStencil_" + ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, IntPtr.Zero, false);
        }

        public override void PostInit()
        {
            OutputFramebuffer.AddTexture(FrameBufferDepthStencilTexture, FrameBuffer.AttachmentUsage.DepthStencil);
        }

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots()
        {
            Active = true;

            InputSlots = null;
            OutputSlots = new CompositorOutputSlot[5];
           
            OutputSlots[0] = new CompositorOutputSlot("SceneOutput", 0, FrameBufferOutputTexture, CompositorSlotType.Texture);
            OutputSlots[1] = new CompositorOutputSlot("DiffuseColor", 1, FrameBufferColorTexture, CompositorSlotType.Texture);
            OutputSlots[2] = new CompositorOutputSlot("PositionColor", 2, FrameBufferDepthTexture, CompositorSlotType.Texture);
            OutputSlots[3] = new CompositorOutputSlot("NormalColor", 3, FrameBufferNormalTexture, CompositorSlotType.Texture);
            OutputSlots[4] = new CompositorOutputSlot("SpecularColor", 4, FrameBufferSpecularTexture, CompositorSlotType.Texture);
        }

        public override void LoadEffect()
        {
            long ticks = DateTime.Now.Ticks;

            NoNodeEffect = PrivateRendererContext.CreateEffect("DeferredShading_Effect_" + ticks);

            NoNodeEffect.PixelProgram = PrivateRendererContext.CreateShaderProgramFromFile("DeferredShading_PixelProgram_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/DeferredShading/pixel_shader.ps");

            NoNodeEffect.VertexProgram = PrivateRendererContext.RC2DEffect.VertexProgram;

            NodeEffect = null;

            //Load all Locations
            TextureDiffuseLocation =       NoNodeEffect.PixelProgram.GetUniformLocation("TextureDiffuse");
            TexturePositionLocation =      NoNodeEffect.PixelProgram.GetUniformLocation("TexturePosition");
            TextureNomalLocation =         NoNodeEffect.PixelProgram.GetUniformLocation("TextureNormal");
            TextureSpecularLocation =      NoNodeEffect.PixelProgram.GetUniformLocation("TextureSpecular");
            DistanceFogIntensityLocation = NoNodeEffect.PixelProgram.GetUniformLocation("DistanceFogIntensity");
            DistanceFogColorLocation =     NoNodeEffect.PixelProgram.GetUniformLocation("DistanceFogColor");
            CameraPositionLocation =       NoNodeEffect.PixelProgram.GetUniformLocation("CameraPosition");
            AmbientIntensityLocation = NoNodeEffect.PixelProgram.GetUniformLocation("AmbientIntensity");
            AmbientColorLocation = NoNodeEffect.PixelProgram.GetUniformLocation("AmbientColor");

            for (int i = 0; i < LightLocations.Length; i++)
            {
                LightLocations[i].TypeLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].Type");
                LightLocations[i].OnLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].On");
                LightLocations[i].LightColorLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].LightColor");
                LightLocations[i].DirectionalLightDirectionLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].DirectionalLightDirection");
                LightLocations[i].PointLightPositionLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightPosition");
                LightLocations[i].PointLightConstantAttLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightConstantAtt");
                LightLocations[i].PointLightLinearAttLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightLinearAtt");
                LightLocations[i].PointLightExpAttLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightExpAtt");
                LightLocations[i].SpotLightConeAngleLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].SpotLightConeAngle");
                LightLocations[i].SpotLightConeCosineLocation = NoNodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].SpotLightConeCosine");
            }
        }

        #endregion

        #region IMessageConsumer implementation

        public void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.WindowResize)
            {
                WindowResizeMessage wrm = msg as WindowResizeMessage;

                PrivateRendererContext.AddRCActionJob(new RCActionTextureResize(FrameBufferOutputTexture,
                        FrameBufferColorTexture, FrameBufferDepthTexture, FrameBufferNormalTexture, 
                        FrameBufferSpecularTexture, FrameBufferDepthStencilTexture, wrm.Width, wrm.Height));
            }
        }

        public int[] ValidMessages { get; private set;}

        #endregion
    }
}

