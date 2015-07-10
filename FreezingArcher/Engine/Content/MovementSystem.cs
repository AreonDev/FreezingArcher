//
//  MovementSystem.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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
    /// The movement system applies movements specified with MovementMessage, MoveStraightMessage or
    /// MoveSidewardsMessage to an entity.
    /// </summary>
    public sealed class MovementSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="messageProvider">The message provider for this system instance.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            //Added needed components
            NeededComponents = new[] { typeof(TransformComponent) };

            internalValidMessages = new[] { (int) MessageId.MovementMessage, (int) MessageId.MoveStraightMessage,
                (int) MessageId.MoveSidewardsMessage, (int)MessageId.MoveVerticalMessage };
            messageProvider += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.MovementMessage)
            {
                TransformComponent tc = Entity.GetComponent<TransformComponent>();
                TransformMessage mm = msg as TransformMessage;

                if (mm.Entity.Name != Entity.Name)
                    return;

                tc.Position += mm.Movement;
                tc.Rotation = mm.Rotation * tc.Rotation;
            }
            else if (msg.MessageId == (int)MessageId.MoveStraightMessage)
            {
                TransformComponent tc = Entity.GetComponent<TransformComponent>();
                MoveStraightMessage msm = msg as MoveStraightMessage;

                if (msm.Entity.Name != Entity.Name)
                    return;

                Vector3 rotation = Vector3.Transform(Vector3.UnitZ, tc.Rotation);
                rotation = new Vector3(rotation.X, 0, rotation.Z);
                rotation.Normalize();
                tc.Position += rotation * msm.Movement;
            }
            else if (msg.MessageId == (int)MessageId.MoveSidewardsMessage)
            {
                TransformComponent tc = Entity.GetComponent<TransformComponent>();
                MoveSidewardsMessage msm = msg as MoveSidewardsMessage;

                if (msm.Entity.Name != Entity.Name)
                    return;
                
                Vector3 rotation = Vector3.Transform(Vector3.UnitX, tc.Rotation);
                rotation = new Vector3(rotation.X, 0, rotation.Z);
                rotation.Normalize();
                tc.Position += rotation * -msm.Movement;
            }
            else if (msg.MessageId == (int)MessageId.MoveVerticalMessage)
            {
                TransformComponent tc = Entity.GetComponent<TransformComponent>();

                MoveVerticalMessage mvm = msg as MoveVerticalMessage;

                if (mvm.Entity.Name != Entity.Name)
                    return;

                tc.Position += new Vector3(0.0f, mvm.Movement, 0.0f);
            }
        }
    }
}
