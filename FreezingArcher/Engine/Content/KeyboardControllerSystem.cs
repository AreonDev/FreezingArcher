//
//  KeyboardControllerSystem.cs
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
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Keyboard controller system. This system converts keyboard input to movement messages.
    /// </summary>
    public sealed class KeyboardControllerSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="messageProvider">The message provider instance for this instance.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            internalValidMessages = new[] { (int) MessageId.Input };
            messageProvider += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            float movement = 2f;

            if (msg.MessageId == (int)MessageId.Input)
            {
                InputMessage im = msg as InputMessage;

                if (im.IsActionDown("sneek"))
                    movement *= 0.5f;

                if (im.IsActionDown("run"))
                    movement *= 2;

                if (im.IsActionDown("forward"))
                {
                    CreateMessage(new MoveStraightMessage(Entity, movement));
                }
                if (im.IsActionDown("backward"))
                {
                    CreateMessage(new MoveStraightMessage(Entity, -movement));
                }
                if (im.IsActionDown("left"))
                {
                    CreateMessage(new MoveSidewardsMessage(Entity, -movement));
                }
                if (im.IsActionDown("right"))
                {
                    CreateMessage(new MoveSidewardsMessage(Entity, movement));
                }
                if (im.IsActionDown("up"))
                {
                    CreateMessage(new MoveVerticalMessage(Entity, movement));
                }
                if (im.IsActionDown("down"))
                {
                    CreateMessage(new MoveVerticalMessage(Entity, -movement));
                }
                if (im.IsActionDown("jump"))
                {
                    var rb = Entity.GetComponent<PhysicsComponent>().RigidBody;
                    if (rb.Arbiters.Count > 0 && rb.Position.Y < 3)
                        CreateMessage(new MoveVerticalMessage(Entity, 5));
                }
            }
        }
    }
}
