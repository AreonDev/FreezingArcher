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

namespace FreezingArcher.Content
{
    public sealed class MovementSystem : EntitySystem
    {
        public override void Init(FreezingArcher.Messaging.MessageManager msgmnr, Entity entity)
        {
            base.Init(msgmnr, entity);

            //Added needed components
            NeededComponents = new[] { typeof(TransformComponent) };

            //Needs more Initializing?
            //Scene does this for me, so no!
            internalValidMessages = new[] { (int) MessageId.Input };
            msgmnr += this;
        }

        public override void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.Input)
            {
                TransformComponent tc = Entity.GetComponent<TransformComponent>();
                InputMessage im = msg as InputMessage;

                // TODO willy
            }
        }
    }
}
