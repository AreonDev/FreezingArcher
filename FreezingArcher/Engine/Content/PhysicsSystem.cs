//
//  PhysicsSystem.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Physics system.
    /// </summary>
    public sealed class PhysicsSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="msgmnr">Msgmnr.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageManager msgmnr, Entity entity)
        {
            base.Init(msgmnr, entity);

            NeededComponents = new[] { typeof(TransformComponent), typeof(ModelComponent), typeof(PhysicsComponent) };

            internalValidMessages = new[] { (int)MessageId.Update };
            msgmnr += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            if (msg is UpdateMessage)
            {
                TransformComponent tc = Entity.GetComponent<TransformComponent>();
                PhysicsComponent pc = Entity.GetComponent<PhysicsComponent>();

                if (tc == null || pc == null || pc.RigidBody == null)
                    return;

                tc.Position = pc.RigidBody.Transform.Position;
                tc.Rotation = pc.RigidBody.Transform.Orientation;
                tc.Scale = new Vector3(pc.RigidBody.Transform.Scale, pc.RigidBody.Transform.Scale,
                    pc.RigidBody.Transform.Scale);
            }
        }
    }
}
