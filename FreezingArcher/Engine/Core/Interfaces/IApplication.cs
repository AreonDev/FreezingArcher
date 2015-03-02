//
//  IApplication.cs
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
using FreezingArcher;

namespace FreezingArcher.Core.Interfaces
{
    /// <summary>
    /// Application interface.
    /// </summary>
    public interface IApplication : IResource
    {
        // <summary>
        // Gets the game manager.
        // </summary>
        // <value>The game manager.</value>
        //IGameManager GameManager { get; }FIXME

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>The window.</value>
        IWindow Window { get; }

        // <summary>
        // Gets the resource manager.
        // </summary>
        // <value>The resource manager.</value>
        //IResourceManager ResourceManager { get; }FIXME

        // <summary>
        // Gets or sets the binded resource.
        // </summary>
        // <value>The resource.</value>
        //IGraphicsResource Resource { get; set; }FIXME

        /// <summary>
        /// Run this instance.
        /// </summary>
        void Run ();
    }
}
