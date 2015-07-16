//
//  RemoveItemFromInventoryMessage.cs
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
using FreezingArcher.Content;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// This message occurs when an item is removed from the inventory.
    /// </summary>
    public sealed class RemoveItemFromInventoryMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.RemoveItemFromInventoryMessage"/>
        /// class.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="inventoryPosition">Inventory position.</param>
        public RemoveItemFromInventoryMessage(Entity entity, Vector2i inventoryPosition)
        {
            Entity = entity;
            InventoryPosition = inventoryPosition;
            MessageId = (int) Messaging.MessageId.RemoveItemFromInventory;
        }

        /// <summary>
        /// Gets the position of this item in the entities inventory.
        /// </summary>
        /// <value>The inventory position.</value>
        public Vector2i InventoryPosition { get; private set; }

        /// <summary>
        /// Gets the entity containing the inventory with this item.
        /// </summary>
        /// <value>The entity.</value>
        public Entity Entity { get; private set; }

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
