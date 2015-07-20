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
    public class DeferredShading : Effect
    {
        public DeferredShading() : base ("DeferredShading_Effect", (int)Convert.ToInt32(DateTime.Now.Ticks))
        {
            
        }

        public bool Init(RendererCore rc)
        {
            long ticks = DateTime.Now.Ticks;

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

