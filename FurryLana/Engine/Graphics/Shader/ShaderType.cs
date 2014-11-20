//
//  ShaderType.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
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

namespace FurryLana.Engine.Graphics.Shader
{
    /// <summary>
    /// Shader type.
    /// </summary>
    public enum ShaderType
    {
        /// <summary>
        /// The fragment shader.
        /// </summary>
        FragmentShader = Pencil.Gaming.Graphics.ShaderType.FragmentShader,
        /// <summary>
        /// The vertex shader.
        /// </summary>
        VertexShader = Pencil.Gaming.Graphics.ShaderType.VertexShader,
        /// <summary>
        /// The geometry shader.
        /// </summary>
        GeometryShader = Pencil.Gaming.Graphics.ShaderType.GeometryShader,
        /// <summary>
        /// The geometry shader ext.
        /// </summary>
        GeometryShaderExt = Pencil.Gaming.Graphics.ShaderType.GeometryShaderExt
    }
}
