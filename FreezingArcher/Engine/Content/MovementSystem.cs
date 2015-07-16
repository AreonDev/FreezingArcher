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
using FreezingArcher.Core;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;
using Jitter.LinearMath;

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
            NeededComponents = new[] { typeof(PhysicsComponent), typeof(TransformComponent) };

            internalValidMessages = new[] { (int) MessageId.Movement, (int) MessageId.MoveStraight,
                (int) MessageId.MoveSidewards, (int)MessageId.MoveVertical, (int) MessageId.Update };
            messageProvider += this;
        }

        private bool was_something_pressed = false;

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            var pc = Entity.GetComponent<PhysicsComponent>();

            was_something_pressed = false;

            if (msg.MessageId == (int)MessageId.Movement)
            {
                TransformMessage mm = msg as TransformMessage;

                if (mm.Entity.Name != Entity.Name)
                    return;

                pc.RigidBody.IsActive = false;
                pc.RigidBody.LinearVelocity = JVector.Zero;

                pc.RigidBody.Position = mm.Movement.ToJitterVector();
                pc.RigidBody.Orientation = JMatrix.CreateFromQuaternion(
                    mm.Rotation.ToJitterQuaternion() * JQuaternion.CreateFromMatrix(pc.RigidBody.Orientation));
                pc.RigidBody.IsActive = true;
            }
            else if (msg.MessageId == (int)MessageId.MoveStraight)
            {
                var tc = Entity.GetComponent<TransformComponent>();

                MoveStraightMessage msm = msg as MoveStraightMessage;

                if (msm.Entity.Name != Entity.Name)
                    return;

                //pc.RigidBody.IsActive = false;
                Vector3 rotation = Vector3.Transform(Vector3.UnitZ, tc.Rotation);
                rotation = new Vector3(rotation.X, 0, rotation.Z);
                rotation.Normalize();
                pc.RigidBody.LinearVelocity += ((rotation * msm.Movement).ToJitterVector());
                //pc.RigidBody.IsActive = true;

                was_something_pressed = true;
            }
            else if (msg.MessageId == (int)MessageId.MoveSidewards)
            {
                var tc = Entity.GetComponent<TransformComponent>();

                MoveSidewardsMessage msm = msg as MoveSidewardsMessage;

                if (msm.Entity.Name != Entity.Name)
                    return;

                //pc.RigidBody.IsActive = false;
                Vector3 rotation = Vector3.Transform(Vector3.UnitX, tc.Rotation);
                rotation = new Vector3(rotation.X, 0, rotation.Z);
                rotation.Normalize();
                pc.RigidBody.LinearVelocity += ((rotation * -msm.Movement).ToJitterVector());
                //pc.RigidBody.IsActive = true;

                was_something_pressed = true;
            }
            else if (msg.MessageId == (int)MessageId.MoveVertical)
            {
                var tc = Entity.GetComponent<TransformComponent>();

                MoveVerticalMessage mvm = msg as MoveVerticalMessage;

                if (mvm.Entity.Name != Entity.Name)
                    return;

                //pc.RigidBody.IsActive = false;
                JVector vec2 = pc.RigidBody.LinearVelocity;
                pc.RigidBody.LinearVelocity = new JVector(vec2.X, mvm.Movement, vec2.Z);
                //pc.RigidBody.IsActive = true;

                was_something_pressed = true;
            }
                
            JVector vec = pc.RigidBody.LinearVelocity;
            vec.Y = 0.0f;

            if (vec.Length() > 2.829f)
                vec *= (1.0f / vec.Length()) * 2.829f;

            pc.RigidBody.LinearVelocity = new JVector(vec.X, pc.RigidBody.LinearVelocity.Y, vec.Z);

            if (msg.MessageId == (int)MessageId.Update)
            {
                if (!was_something_pressed && vec.Length() < 2.8f)
                    pc.RigidBody.LinearVelocity = new JVector(0.0f, pc.RigidBody.LinearVelocity.Y, 0.0f);
            }
        }
    }
}
