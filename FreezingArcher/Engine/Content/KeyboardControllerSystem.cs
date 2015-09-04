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

            internalValidMessages = new[] { (int) MessageId.Input, (int) MessageId.Update };
            messageProvider += this;
        }

        bool isRunning = false;

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            float movement = 6f;

            int msr = 0;

            if (msg.MessageId == (int)MessageId.Input)
            {
                InputMessage im = msg as InputMessage;

                isRunning = false;

                if (im.IsActionDown ("sneek"))
                {
                    movement *= 0.25f;
                    msr = 1;
                }
                else if (im.IsActionDown ("run"))
                {
                    if (Entity.HasComponent<StaminaComponent>())
                    {
                        var sc = Entity.GetComponent<StaminaComponent>();

                        if (sc.Stamina > sc.StaminaDeltaPerUpdate)
                        {
                            movement *= 4;
                            msr = 2;
                            var stamina = sc.Stamina - sc.StaminaDeltaPerUpdate;
                            sc.Stamina = sc.Stamina < 0 ? 0 : stamina;
                            isRunning = true;
                        }
                    }
                    else
                    {
                        movement *= 4;
                        msr = 2;
                    }
                }   

                if (im.IsActionDown("forward"))
                {
                    switch (msr)
                    {
                    case 0:
                        CreateMessage (new PlayerMoveMessage (this.Entity));
                        break;
                    }

                    CreateMessage(new MoveStraightMessage(Entity, movement));
                }
                if (im.IsActionDown("backward"))
                {
                    switch (msr)
                    {
                    case 0:
                        CreateMessage (new PlayerMoveMessage (this.Entity));
                        break;
                    }

                    CreateMessage(new MoveStraightMessage(Entity, -movement));
                }
                if (im.IsActionDown("left"))
                {
                    switch (msr)
                    {
                    case 0:
                        CreateMessage (new PlayerMoveMessage (this.Entity));
                        break;
                    }

                    CreateMessage(new MoveSidewardsMessage(Entity, -movement));
                }
                if (im.IsActionDown("right"))
                {
                    switch (msr)
                    {
                    case 0:
                        CreateMessage (new PlayerMoveMessage (this.Entity));
                        break;
                    }

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
                    {
                        CreateMessage (new MoveVerticalMessage (Entity, 5));
                        CreateMessage (new PlayerJumpedMessage (Entity));
                    }
                }
            }

            if (!isRunning && msg.MessageId == (int) MessageId.Update && Entity.HasComponent<StaminaComponent>())
            {
                var sc = Entity.GetComponent<StaminaComponent>();
                var stamina = sc.Stamina + sc.StaminaDeltaPerUpdate / 10;
                sc.Stamina = stamina > sc.MaximumStamina ? sc.MaximumStamina : stamina;
            }
        }
    }
}
