//
//  WindowMoveMessage.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
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
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Window move message.
    /// </summary>
    public class WindowMoveMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.WindowMoveMessage"/> class.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="x">The x movement.</param>
        /// <param name="y">The y movement.</param>
        public WindowMoveMessage (Window window, int x, int y)
        {
            Window = window;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>The window.</value>
        public Window Window { get; protected set; }

        /// <summary>
        /// Gets the x movement.
        /// </summary>
        /// <value>The x movement.</value>
        public int X { get; protected set; }

        /// <summary>
        /// Gets the y movement.
        /// </summary>
        /// <value>The y movement.</value>
        public int Y { get; protected set; }

        #region IMessage implementation

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public object Source { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public object Destination { get; set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId
        {
            get
            {
                return (int) Messaging.MessageId.WindowMoveMessage;
            }
        }

        #endregion
    }
}
