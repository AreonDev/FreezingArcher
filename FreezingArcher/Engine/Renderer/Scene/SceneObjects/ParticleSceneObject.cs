//
//  ParticleSceneObject.cs
//
//  Author:
//       dboeg <>
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


using FreezingArcher.Output;
using FreezingArcher.Renderer;

using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer.Scene.SceneObjects
{
    public class ParticleSceneObject : SceneObject
    {
        public struct ParticleVertexDescription
        {
            public Math.Vector3 Position
            {
                get
                {
                    return new Math.Vector3(PositionX, PositionY, PositionZ);
                }

                set
                {
                    PositionX = value.X;
                    PositionY = value.Y;
                    PositionZ = value.Z;
                }
            }

            public Math.Color4 Color
            {
                get
                {
                    return new Math.Color4(ColorR, ColorG, ColorB, ColorA);
                }

                set
                {
                    ColorR = value.R;
                    ColorG = value.G;
                    ColorB = value.B;
                    ColorA = value.A;
                }
            }

            public Math.Vector2 Size
            {
                get
                {
                    return new Math.Vector2(SizeX, SizeY);
                }

                set
                {
                    SizeX = value.X;
                    SizeY = value.Y;
                }
            }

            //Position
            public float PositionX, PositionY, PositionZ;

            //Color
            public float ColorR, ColorG, ColorB, ColorA;

            //Size
            public float SizeX, SizeY;

            //Life
            public float Life;
        }

        public ParticleVertexDescription[] Particles;

        public VertexBuffer ParticleVBO { get; private set;}
        public VertexBufferArray ParticleVAO { get; private set;}

        public int ParticleCount { get; private set;}

        public Texture2D    BillboardTexture { get; set;}
        public Effect       ParticleEffect { get; set;}

        public RendererBlendingFactorDest DestinationBlendingFactor { get; set;}
        public RendererBlendingFactorSrc SourceBlendingFactor{ get; set;}
        public RendererBlendEquationMode BlendEquation {get; set;}

        public ParticleSceneObject(int particlecount)
        {
            ParticleCount = particlecount;
            Particles = new ParticleVertexDescription[ParticleCount];

            SourceBlendingFactor = RendererBlendingFactorSrc.SrcAlpha;
            DestinationBlendingFactor = RendererBlendingFactorDest.One;
            BlendEquation = RendererBlendEquationMode.FuncAdd;
        }

        public override bool Init(RendererContext rc)
        {
            CoreScene.RCActionInitSceneObject iso = new CoreScene.RCActionInitSceneObject(this, rc);

            if (rc.Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                ParticleVBO = rc.CreateVertexBuffer(IntPtr.Zero, 40 * (int)ParticleCount, RendererBufferUsage.StreamDraw, "ParticleVBO_" + DateTime.Now.Ticks);
                if (ParticleVBO == null)
                    return false;

                VertexBufferLayoutKind[] vblk = new VertexBufferLayoutKind[4];
                vblk[0] = new VertexBufferLayoutKind();
                vblk[0].AttributeID = 0;
                vblk[0].AttributeSize = 3;
                vblk[0].AttributeType = RendererVertexAttribType.Float;
                vblk[0].Normalized = false;
                vblk[0].Offset = 0;
                vblk[0].Stride = sizeof(float) * 10;

                vblk[1] = new VertexBufferLayoutKind();
                vblk[1].AttributeID = 1;
                vblk[1].AttributeSize = 4;
                vblk[1].AttributeType = RendererVertexAttribType.Float;
                vblk[1].Normalized = false;
                vblk[1].Offset = 12;
                vblk[1].Stride = sizeof(float) * 10;

                vblk[2] = new VertexBufferLayoutKind();
                vblk[2].AttributeID = 2;
                vblk[2].AttributeSize = 2;
                vblk[2].AttributeType = RendererVertexAttribType.Float;
                vblk[2].Normalized = false;
                vblk[2].Offset = 28;
                vblk[2].Stride = sizeof(float) * 10;

                vblk[3] = new VertexBufferLayoutKind();
                vblk[3].AttributeID = 3;
                vblk[3].AttributeSize = 1;
                vblk[3].AttributeType = RendererVertexAttribType.Float;
                vblk[3].Normalized = false;
                vblk[3].Offset = 36;
                vblk[3].Stride = sizeof(float) * 10;

                ParticleVAO = rc.CreateVertexBufferArray(ParticleVBO, vblk, "ParticleVAO_" + DateTime.Now.Ticks);
                if (ParticleVAO == null)
                {
                    rc.DeleteGraphicsResourceAsync(ParticleVBO);
                    return false;
                }

                //Load BasicEffect
                long ticks = DateTime.Now.Ticks;

                ParticleEffect = rc.CreateEffect("ParticleEffect_" + ticks);
                ParticleEffect.VertexProgram = rc.CreateShaderProgramFromFile("ParticleEffect_VertexProgram_" + ticks, ShaderType.VertexShader, "lib/Renderer/Effects/BasicParticleEffect/vertex_shader.vs");
                ParticleEffect.PixelProgram = rc.CreateShaderProgramFromFile("ParticleEffect_PixelProgram_" + ticks, ShaderType.PixelShader, "lib/Renderer/Effects/BasicParticleEffect/pixel_shader.ps");
                ParticleEffect.GeometryProgram = rc.CreateShaderProgramFromFile("ParticleEffect_GeometryProgram" + ticks, ShaderType.GeometryShader, "lib/Renderer/Effects/BasicParticleEffect/geometry_shader.gs");

                BillboardTexture = rc.CreateTexture2D("BillboardTexture_" + ticks, true, "lib/Renderer/TestGraphics/particle_03.png");
                BillboardTexture.Sampler.MagnificationFilter = MagnificationFilter.InterpolateLinear;
                BillboardTexture.Sampler.MinificationFilter = MinificationFilter.InterpolateLinearAndInterpolateMipmapLinear;
            }
            else
                rc.AddRCActionJob(iso);

            return true;
        }

        public override void Update()
        {
            ParticleVBO.UpdateBuffer<ParticleVertexDescription>(Particles, 40 * (int)ParticleCount);
        }

        #region implemented abstract members of SceneObject

        public override void Draw(RendererContext rc)
        {
            RendererBlendingFactorDest oldblenddest = (RendererBlendingFactorDest)rc.GetBlendDst();
            RendererBlendingFactorSrc oldblendsourc = (RendererBlendingFactorSrc)rc.GetBlendSrc();
            RendererBlendEquationMode oldequationmode = (RendererBlendEquationMode)rc.GetBlendEquationMode();

            if (ParticleEffect == null)
                return;

            rc.EnableDepthMaskWriting(false);

            ParticleEffect.BindPipeline();

            rc.Enable(RendererEnableCap.Blend);
            rc.SetBlendFunc(SourceBlendingFactor, DestinationBlendingFactor);
            rc.SetBlendEquation(BlendEquation);

            ParticleVAO.BindVertexBufferArray();

            ParticleEffect.GeometryProgram.SetUniform(ParticleEffect.GeometryProgram.GetUniformLocation("WorldMatrix"), WorldMatrix);

            if (rc.Scene.CameraManager.ActiveCamera != null)
            {
                ParticleEffect.GeometryProgram.SetUniform(ParticleEffect.GeometryProgram.GetUniformLocation("CameraPosition"), rc.Scene.CameraManager.ActiveCamera.Position);
                ParticleEffect.GeometryProgram.SetUniform(ParticleEffect.GeometryProgram.GetUniformLocation("ViewMatrix"), rc.Scene.CameraManager.ActiveCamera.ViewMatrix);
                ParticleEffect.GeometryProgram.SetUniform(ParticleEffect.GeometryProgram.GetUniformLocation("ProjectionMatrix"), rc.Scene.CameraManager.ActiveCamera.ProjectionMatrix);
            }

            if (BillboardTexture != null)
            {
                BillboardTexture.Bind(0);
                ParticleEffect.PixelProgram.SetUniform(ParticleEffect.PixelProgram.GetUniformLocation("BillboardTexture"), 0);
            }

            rc.DrawArrays(0, (int)ParticleCount, RendererBeginMode.Points);

            if (BillboardTexture != null)
                BillboardTexture.Unbind();

            ParticleVAO.UnbindVertexBufferArray();

            rc.SetBlendEquation(oldequationmode);
            rc.SetBlendFunc(oldblendsourc, oldblenddest);

            ParticleEffect.UnbindPipeline();

            rc.EnableDepthMaskWriting(true);
        }

        public override void DrawInstanced(RendererContext rc, int count)
        {
            //No Instancing!
            return;
        }

        public override string GetName()
        {
            return "ParticleSceneObject";
        }

        public override SceneObject Clone()
        {
            //Cloning? Not YET!
            return null;
        }

        #endregion
    }
}

