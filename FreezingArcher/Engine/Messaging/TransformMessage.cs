//
//  TransformMessage.cs
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
using FreezingArcher.Math;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// This message describes a movement and rotation request on an entity. The movement does not affect the rotation
    /// (view direction) of the entity. The rotation of the entity is only affected by the rotation given with this
    /// message.
    /// </summary>
    public class TransformMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.TransformMessage"/> class. This message
        /// describes a movement and rotation request on an entity. The movement does not affect the rotation (view
        /// direction) of the entity. The rotation of the entity is only affected by the rotation given with this
        /// message.
        /// </summary>
        /// <param name="movement">Movement.</param>
        /// <param name="rotation">Rotation.</param>
        public TransformMessage(Vector3 movement, Quaternion rotation)
        {
            Movement = movement;
            Rotation = rotation;
            MessageId = (int) Messaging.MessageId.MovementMessage;
        }

        /// <summary>
        /// Gets the movement stored in this message. This movement vector only creates a movement of an entity, it does
        /// not affect the rotation (view direction) of an entity.
        /// </summary>
        /// <value>The movement.</value>
        public Vector3 Movement { get; private set; }

        /// <summary>
        /// Gets the rotation stored in this message. This rotation specifies the relative rotation of the entity. The
        /// rotation is the view direction of the entity.
        /// </summary>
        /// <value>The rotation.</value>
        public Quaternion Rotation { get; private set; }

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
