//
//  ArtificialIntelligenceSystem.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using System;
using FreezingArcher.Math;
using FreezingArcher.Core;
using Jitter.LinearMath;

namespace FreezingArcher.Content
{
    public sealed class ArtificialIntelligenceSystem : EntitySystem
    {
        public override void Init (MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            NeededComponents = new[] { typeof (HealthComponent), typeof (ArtificialIntelligenceComponent) };

            internalValidMessages = new[] { (int) MessageId.Update };
            messageProvider += this;
        }

        public override void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update)
            {
                var ai = Entity.GetComponent<ArtificialIntelligenceComponent> ();
                if (ai.ArtificialIntelligence != null)
                {
                    var physics = Entity.GetComponent<PhysicsComponent>();
                    ai.ArtificialIntelligence.Think (Entity.GetComponent<PhysicsComponent>(),
                        Entity.GetComponent<HealthComponent>(), ai.AIManager.Map,
                        ai.AIManager.CollectEntitiesNearby (physics.RigidBody.Position.ToFreezingArcherVector (),
                            ai.MaximumEntityDistance));
                }
            }
        }
    }
}
