//
//  IResourceManager.cs
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
using FurryLana.Engine.Input.Interfaces;
using FurryLana.Engine.Model.Interfaces;
using FurryLana.Engine.Texture.Interfaces;

namespace FurryLana.Engine.Game.Interfaces
{
    /// <summary>
    /// Resource manager interface.
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gets the input manager.
        /// </summary>
        /// <value>The input manager.</value>
        IInputManager   InputManager   { get; }

        /// <summary>
        /// Gets the texture manager.
        /// </summary>
        /// <value>The texture manager.</value>
        ITextureManager TextureManager { get; }

        /// <summary>
        /// Gets the model manager.
        /// </summary>
        /// <value>The model manager.</value>
        IModelManager   ModelManager   { get; }
    }
}
