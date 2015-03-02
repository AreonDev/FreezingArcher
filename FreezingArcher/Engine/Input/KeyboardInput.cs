//
//  KeyboardInput.cs
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

namespace FreezingArcher.Input
{
    /// <summary>
    /// Keyboard input.
    /// </summary>
    public class KeyboardInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Input.KeyboardInput"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="scancode">Scancode.</param>
        /// <param name="action">Action.</param>
        /// <param name="modifier">Modifier.</param>
        public KeyboardInput (Key key, int scancode, KeyAction action, KeyModifiers modifier)
        {
            Key = key;
            Scancode = scancode;
            Action = action;
            Modifier = modifier;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public Key Key { get; internal set; }
        /// <summary>
        /// Gets or sets the scancode.
        /// </summary>
        /// <value>The scancode.</value>
        public int Scancode { get;  internal set; }
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public KeyAction Action { get; internal set; }
        /// <summary>
        /// Gets or sets the modifier.
        /// </summary>
        /// <value>The modifier.</value>
        public KeyModifiers Modifier { get; internal set; }
    }
}
