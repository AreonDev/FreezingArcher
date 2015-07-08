//
//  ModelSystem.cs
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
    /// <summary>
    /// Model system. Updates scene data.
    /// </summary>
    public sealed class ModelSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="msgmnr">Msgmnr.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            //Added needed components
            NeededComponents = new[] { typeof(TransformComponent), typeof(ModelComponent) };

            //Needs more Initializing?
            //Scene does this for me, so no!
            internalValidMessages = new[] { (int) MessageId.PositionChangedMessage,
                (int) MessageId.RotationChangedMessage, (int) MessageId.ScaleChangedMessage };
            messageProvider += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            ModelComponent mc = Entity.GetComponent<ModelComponent>();
            TransformComponent tc = Entity.GetComponent<TransformComponent>();

            if (mc == null || mc.Model == null)
                return;
            
            if (msg.MessageId == (int) MessageId.PositionChangedMessage)
            {
                PositionChangedMessage pcm = msg as PositionChangedMessage;
                if (pcm.Entity.Name == Entity.Name)
                    mc.Model.Position = tc.Position;
            }

            if (msg.MessageId == (int) MessageId.RotationChangedMessage)
            {
                RotationChangedMessage rcm = msg as RotationChangedMessage;
                if (rcm.Entity.Name == Entity.Name)
                    mc.Model.Rotation = tc.Rotation;
            }

            if (msg.MessageId == (int) MessageId.ScaleChangedMessage)
            {
                ScaleChangedMessage scm = msg as ScaleChangedMessage;
                if (scm.Entity.Name == Entity.Name)
                    mc.Model.Scaling = tc.Scale;
            }
        }
    }
}
