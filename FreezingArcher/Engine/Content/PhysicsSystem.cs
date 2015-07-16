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
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;
using Jitter.LinearMath;

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
        /// <param name="messageProvider">The message provider for this system.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            NeededComponents = new[] { typeof(TransformComponent), typeof(ModelComponent), typeof(PhysicsComponent) };

            internalValidMessages = new[] { (int)MessageId.Update};

            messageProvider += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update)
            {
                var pc = Entity.GetComponent<PhysicsComponent>();
                if (pc == null || pc.RigidBody == null || pc.RigidBody.IsStaticOrInactive)
                    return;

                TransformComponent tc = Entity.GetComponent<TransformComponent>();

                if (tc == null)
                    return;

                if (pc.PhysicsApplying != 0)
                {
                    if ((pc.PhysicsApplying & (int)PhysicsComponent.PhysicsApplyingEnum.Position) != 0)
                        tc.Position = pc.RigidBody.Position.ToFreezingArcherVector();

                    if ((pc.PhysicsApplying & (int)PhysicsComponent.PhysicsApplyingEnum.Orientation) != 0)
                        tc.Rotation = JQuaternion.CreateFromMatrix(pc.RigidBody.Orientation).ToFreezingArcherQuaternion();
                }
	    }
        }
    }
}
