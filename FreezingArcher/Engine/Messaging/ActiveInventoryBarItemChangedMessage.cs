﻿//
//  SetActiveInventoryBarItemMessage.cs
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Content;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// This message occurs when the active inventory position should be changed.
    /// </summary>
    public sealed class ActiveInventoryBarItemChangedMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.SetActiveInventoryBarItemMessage"/>
        /// class.
        /// </summary>
        /// <param name="item">The item at the given position in the inventory bar.</param>
        /// <param name="position">Position.</param>
        public ActiveInventoryBarItemChangedMessage(ItemComponent item, byte position)
        {
            Position = position;
            Item = item;
            MessageId = (int) Messaging.MessageId.ActiveInventoryBarItemChanged;
        }

        /// <summary>
        /// Gets the position the item should have in the inventory bar.
        /// </summary>
        /// <value>The position.</value>
        public byte Position { get; private set; }

        /// <summary>
        /// Gets the item at active inventory position.
        /// </summary>
        /// <value>The item.</value>
        public ItemComponent Item { get; private set; }

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
        public int MessageId { get; private set; }

        #endregion
    }
}
