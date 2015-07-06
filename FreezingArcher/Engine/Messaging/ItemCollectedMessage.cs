//
//  ItemCollectedMessage.cs
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
using FreezingArcher.Math;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// This message occurs when an entity collects an item.
    /// </summary>
    public sealed class ItemCollectedMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.ItemCollectedMessage"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="inventoryPosition">Inventory position.</param>
        /// <param name="entity">Entity.</param>
        public ItemCollectedMessage(Entity entity, ItemComponent item, Vector2i? inventoryPosition = null)
        {
            Entity = entity;
            Item = item;
            InventoryPosition = inventoryPosition;
            MessageId = (int) Messaging.MessageId.ItemCollectedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.ItemCollectedMessage"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="inventoryPositionX">Inventory position x.</param>
        /// <param name="inventoryPositionY">Inventory position y.</param>
        /// <param name="entity">Entity.</param>
        public ItemCollectedMessage(Entity entity, ItemComponent item, int inventoryPositionX, int inventoryPositionY) :
        this(entity, item, new Vector2i(inventoryPositionX, inventoryPositionY))
        {}

        /// <summary>
        /// Gets the collected item.
        /// </summary>
        /// <value>The collected item.</value>
        public ItemComponent Item { get; private set; }

        /// <summary>
        /// Gets the inventory position the collected item should have.
        /// </summary>
        /// <value>The inventory position.</value>
        public Vector2i? InventoryPosition { get; private set; }

        /// <summary>
        /// Gets the entity that collected the item.
        /// </summary>
        /// <value>The entity that collected the item.</value>
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

