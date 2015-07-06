//
//  AddItemToInventoryBarMessage.cs
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
using FreezingArcher.Math;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// This message occurs when an item should be added to the inventory bar.
    /// </summary>
    public sealed class AddItemToInventoryBarMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.AddItemToInventoryBarMessage"/> class.
        /// </summary>
        /// <param name="itemPositionInInventory">Item position in inventory.</param>
        /// <param name="inventoryBarPosition">Inventory bar position.</param>
        public AddItemToInventoryBarMessage(Vector2i itemPositionInInventory, byte? inventoryBarPosition = null)
        {
            ItemPositionInInventory = itemPositionInInventory;
            InventoryBarPosition = inventoryBarPosition;
            MessageId = (int) Messaging.MessageId.AddItemToInventoryBar;
        }

        public AddItemToInventoryBarMessage(int itemPositionInInventoryX, int itemPositionInInventoryY,
            byte? inventoryBarPosition = null) :
        this(new Vector2i(itemPositionInInventoryX, itemPositionInInventoryY), inventoryBarPosition)
        {}

        /// <summary>
        /// Gets the position of the item in the inventory.
        /// </summary>
        /// <value>The item position in inventory.</value>
        public Vector2i ItemPositionInInventory { get; private set; }

        /// <summary>
        /// Gets the position that the item should have in the inventory bar.
        /// </summary>
        /// <value>The inventory bar position.</value>
        public byte? InventoryBarPosition { get; private set; }

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

