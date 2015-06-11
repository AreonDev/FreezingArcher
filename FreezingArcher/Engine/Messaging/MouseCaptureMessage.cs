//
//  MouseCapturedMessage.cs
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
using System;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Mouse capture message.
    /// </summary>
    public class MouseCaptureMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.MouseCaptureMessage"/> class.
        /// </summary>
        /// <param name="captured">If set to <c>true</c> captured.</param>
        public MouseCaptureMessage(bool captured)
        {
            IsMouseCaptured = captured;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse is captured.
        /// </summary>
        /// <value><c>true</c> if the mouse is captured; otherwise, <c>false</c>.</value>
        public bool IsMouseCaptured { get; protected set; }

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
                return (int) Messaging.MessageId.MouseCaptureMessage;
            }
        }

        #endregion
    }
}

