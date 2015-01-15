//
//  MouseInput.cs
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
using Pencil.Gaming;

namespace FurryLana.Engine.Input
{
    /// <summary>
    /// Mouse input.
    /// </summary>
    public class MouseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Input.MouseInput"/> class.
        /// </summary>
        /// <param name="button">Button.</param>
        /// <param name="action">Action.</param>
        public MouseInput (MouseButton button, KeyAction action)
        {
            Button = button;
            Action = action;
        }

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        /// <value>The button.</value>
        public MouseButton Button { get; internal set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public KeyAction Action { get; internal set; }
    }
}

