//
//  CompositorNodeDeferredShading.cs
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

using FreezingArcher.Renderer;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorNodeDeferredShading : CompositorNode
    {
        private Texture2D OutputTexture;

        public CompositorNodeDeferredShading(RendererContext rc, Messaging.MessageProvider mp) : base("NodeDeferredShading", rc, mp)
        {
        }

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots()
        {
            InputSlots = new CompositorInputSlot[5];
            InputSlots[0] = new CompositorInputSlot("DiffuseColor", CompositorInputSlotImportance.Required, 0, CompositorSlotType.Texture);
            InputSlots[1] = new CompositorInputSlot("PositionColor", CompositorInputSlotImportance.Required, 1, CompositorSlotType.Texture);
            InputSlots[2] = new CompositorInputSlot("NormalColor", CompositorInputSlotImportance.Required, 2, CompositorSlotType.Texture);
            InputSlots[3] = new CompositorInputSlot("SpecularColor", CompositorInputSlotImportance.Required, 3, CompositorSlotType.Texture);
            InputSlots[4] = new CompositorInputSlot("LightInformation", CompositorInputSlotImportance.Required, 4, CompositorSlotType.Value);

            OutputSlots = new CompositorOutputSlot[1];
            OutputSlots[0] = new CompositorOutputSlot("DiffuseColor", 0, OutputTexture, CompositorSlotType.Texture);
        }

        public override void InitOtherStuff()
        {
            OutputTexture = PrivateRendererContext.CreateTexture2D("DeferredShading_Output_Texture_" + DateTime.Now.Ticks, PrivateRendererContext.ViewportSize.X,
                PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, true);
        }

        public override void Draw()
        {
            PrivateRendererContext.Clear(Color4.Black, 0);
            PrivateRendererContext.Clear(Color4.Black, 1);
            PrivateRendererContext.Clear(Color4.Black, 2);
            PrivateRendererContext.Clear(Color4.Black, 3);

            PrivateRendererContext.EnableDepthTest(false);

            if (InputSlots[0].SlotTexture == null)
                return;

            if (InputSlots[0].SlotTexture.Width != OutputTexture.Width || InputSlots[0].SlotTexture.Height != OutputTexture.Height)
            {
                OutputTexture.Resize(InputSlots[0].SlotTexture.Width, InputSlots[0].SlotTexture.Height);
            }

            foreach (CompositorInputSlot cis in InputSlots)
            {
                if (cis.SlotTexture != null)
                    cis.SlotTexture.Bind(cis.SlotNumber);
            }

            PrivateRendererContext.Clear(Math.Color4.Black);

            Sprite spr = new Sprite();
            spr.Init(InputSlots[0].SlotTexture);

            spr.CustomEffect = true;

            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("TextureDiffuse"), 0);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("TexturePosition"), 1);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("TextureNormal"), 2);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("TextureSpecular"), 3);

            try
            {
                NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("CameraPosition"), PrivateRendererContext.Scene.CameraManager.ActiveCamera.Position);
            }catch{}

            //Setup lights
            List<Light> Lights = (List<Light>)InputSlots[4].Value;

            if (Lights != null)
            {
                for (int i = 0; i < Lights.Count; i++)
                {
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].Type"), 
                        (int)Lights[i].Type);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].LightColor"), 
                        Lights[i].Color);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].AmbientIntensity"), 
                        Lights[i].AmbientIntensity);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].AmbientColor"), 
                        Lights[i].AmbientColor);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].DirectionalLightDirection"), 
                        Lights[i].DirectionalLightDirection);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightPosition"), 
                        Lights[i].PointLightPosition);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightConstantAtt"), 
                        Lights[i].PointLightConstantAttenuation);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightLinearAtt"), 
                        Lights[i].PointLightLinearAttenuation);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].PointLightExpAtt"), 
                        Lights[i].PointLightExponentialAttenuation);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].SpotLightConeAngle"), 
                        Lights[i].SpotLightConeAngle);
                    NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Lights[" + i + "].SpotLightConeCosine"), 
                        Lights[i].SpotLightConeCosine);
                }
            }
            else
            {
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CompositorNodeDeferredShading",
                    "No Light information passed!");
            }

            PrivateRendererContext.DrawSpriteAbsolute(spr);

            PrivateRendererContext.EnableDepthTest(true);
        }

        public override void LoadEffect()
        {
            long ticks = DateTime.Now.Ticks;

            NodeEffect = PrivateRendererContext.CreateEffect("DeferredShading_Effect_" + ticks);

            NodeEffect.PixelProgram = PrivateRendererContext.CreateShaderProgramFromFile("DeferredShading_PixelProgram_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/DeferredShading/pixel_shader.ps");

            NodeEffect.VertexProgram = PrivateRendererContext.RC2DEffect.VertexProgram;
        }

        public override void Bypass()
        {
            if (InputSlots[0].SlotTexture.Width != OutputTexture.Width || InputSlots[0].SlotTexture.Height != OutputTexture.Height)
            {
                OutputTexture.Resize(InputSlots[0].SlotTexture.Width, InputSlots[0].SlotTexture.Height);
            }

            base.Bypass();
        }

        #endregion
    }
}

