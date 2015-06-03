//
//  DefferedShading.cs
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

namespace FreezingArcher.Renderer
{
    public class DefferedShading : Effect
    {
        #region Private Region
        FrameBuffer m_Framebuffer;
        Texture2D   m_TextureNormal;
        Texture2D   m_TextureDiffuse;
        Texture2D   m_TextureDepth;
        Texture2D   m_TextureSpecular;
        TextureDepthStencil   m_TextureDepthStencilBuffer;

        #endregion

        public DefferedShading(string name, int id) : base (name, id)
        {
            m_Framebuffer = null;
            m_TextureNormal = null;
            m_TextureDiffuse = null;
            m_TextureDepth = null;
            m_TextureSpecular = null;

            m_TextureDepthStencilBuffer = null;
        }

        public bool Init(RendererCore rc)
        {
            long ticks = DateTime.Now.Ticks;

            m_Framebuffer = rc.CreateFrameBuffer("DefferedShading_" + ticks);

            m_TextureNormal = rc.CreateTexture2D("DefferedShading_Normal_" + ticks, 
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            m_TextureDiffuse = rc.CreateTexture2D("DefferedShading_Diffuse_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            m_TextureDepth = rc.CreateTexture2D("DefferedShading_Depth_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            m_TextureSpecular = rc.CreateTexture2D("DefferedShading_Specular_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            m_TextureDepthStencilBuffer = rc.CreateTextureDepthStencil("DefferedShading_DepthStencil_Buffer_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, IntPtr.Zero, false);

            m_Framebuffer.BeginPrepare();

            m_Framebuffer.AddTexture(m_TextureNormal, FrameBuffer.AttachmentUsage.Color0);
            m_Framebuffer.AddTexture(m_TextureDiffuse, FrameBuffer.AttachmentUsage.Color1);
            m_Framebuffer.AddTexture(m_TextureDepth, FrameBuffer.AttachmentUsage.Color2);
            m_Framebuffer.AddTexture(m_TextureSpecular, FrameBuffer.AttachmentUsage.Color3);

            m_Framebuffer.AddTexture(m_TextureDepthStencilBuffer, FrameBuffer.AttachmentUsage.DepthStencil);

            m_Framebuffer.EndPrepare();

            this.VertexProgram = rc.CreateShaderProgramFromFile("DefferedShading_VertexProgram_" + ticks, ShaderType.VertexShader,
                "lib/Renderer/Effects/DefferedShading/vertex_shader.vs");

            this.PixelProgram = rc.CreateShaderProgramFromFile("DefferedShading_PixelProgram_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/DefferedShading/pixel_shader.ps");

            return true;
        }

        public void Use()
        {
            this.BindPipeline();
        }
    }
}

