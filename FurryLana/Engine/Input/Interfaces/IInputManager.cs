//
//  IInputManager.cs
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
using FurryLana.Engine.Graphics.Interfaces;
using Pencil.Gaming;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Input.Interfaces
{
    /// <summary>
    /// Input manager interface.
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// Handles the keyboard input.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="key">Key.</param>
        /// <param name="scancode">Scancode.</param>
        /// <param name="action">Action.</param>
        /// <param name="mods">Mods.</param>
        void HandleKeyboardInput (GlfwWindowPtr window, Key key, int scancode, KeyAction action, KeyModifiers mods);

        /// <summary>
        /// Handles the mouse button.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="button">Button.</param>
        /// <param name="action">Action.</param>
        void HandleMouseButton (GlfwWindowPtr window, MouseButton button, KeyAction action);

        /// <summary>
        /// Handles the mouse move.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        void HandleMouseMove (GlfwWindowPtr window, double x, double y);

        /// <summary>
        /// Handles the mouse scroll.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="xoffs">Xoffs.</param>
        /// <param name="yoffs">Yoffs.</param>
        void HandleMouseScroll (GlfwWindowPtr window, double xoffs, double yoffs);

        /// <summary>
        /// Generates the update description.
        /// </summary>
        /// <returns>The update description.</returns>
        /// <param name="deltaTime">Delta time.</param>
        UpdateDescription GenerateUpdateDescription (float deltaTime);
    }
}
