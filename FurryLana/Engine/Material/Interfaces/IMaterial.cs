//
//  IMaterial.cs
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
using FurryLana.Engine.Texture.Interfaces;
using FurryLana.Engine.Renderer.Interfaces;

namespace FurryLana.Engine.Interfaces.Material
{
    /// <summary>
    /// Material interface.
    /// </summary>
    public interface IMaterial
    {
        /// <summary>
        /// Gets or sets the diffuse color map.
        /// RGBA channel used.
        /// </summary>
        /// <value>The diffuse color map.</value>
        ITexture Diffuse      { get; set; }

        /// <summary>
        /// Gets or sets the height and normal map.
        /// Normalmap channels: RGB
        /// Heightmap channels: A
        /// </summary>
        /// <value>The height and normal map.</value>
        ITexture NormalHeight { get; set; }

        /// <summary>
        /// Gets or sets the specular map.
        /// Specularcolor: RGB
        /// Specularintensity: A
        /// </summary>
        /// <value>The specular map.</value>
        ITexture Specular     { get; set; }

        /// <summary>
        /// Gets or sets the ambient map.
        /// </summary>
        /// <value>The ambient map.</value>
        ITexture Ambient      { get; set; }


        /// <summary>
        /// Gets or sets the shader program.
        /// </summary>
        /// <value>The shader program.</value>
        IShaderProgram ShaderProgram { get; set; }
    }
}
