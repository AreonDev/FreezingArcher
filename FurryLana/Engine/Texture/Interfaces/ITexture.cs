//
//  ITexture.cs
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
using System.Drawing;
using FurryLana.Engine.Graphics.Interfaces;

namespace FurryLana.Engine.Texture.Interfaces
{
    /// <summary>
    /// Texture interface.
    /// </summary>
    public interface ITexture : IResource, IManageable
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        int Width  { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>The depth.</value>
        int Depth  { get; set; }

        /// <summary>
        /// Convert this texture to a <see cref="System.Drawing.Bitmap"/> 
        /// </summary>
        /// <returns>The bitmap.</returns>
        Bitmap ToBitmap ();

        /// <summary>
        /// Bind the texture.
        /// </summary>
        void Bind ();
    }
}
