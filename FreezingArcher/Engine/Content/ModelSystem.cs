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
        public override void Init(FreezingArcher.Messaging.MessageManager msgmnr, Entity entity)
        {
            base.Init(msgmnr, entity);

            //Added needed components
            NeededComponents = new Type[] { typeof(TransformComponent), typeof(ModelComponent) };

            //Needs more Initializing?
            //Scene does this for me, so no!
            internalValidMessages = new int[] { (int)Messaging.MessageId.Update };
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(FreezingArcher.Messaging.Interfaces.IMessage msg)
        {
            ModelComponent mc = Entity.GetComponent<ModelComponent>();

            if (mc == null)
                return;

            PositionChangedMessage pcm = msg as PositionChangedMessage;
            if (pcm != null)
                mc.Model.Position = pcm.Position;

            RotationChangedMessage rcm = msg as RotationChangedMessage;
            if (rcm != null)
                mc.Model.Rotation = rcm.Rotation;

            ScaleChangedMessage scm = msg as ScaleChangedMessage;
            if (scm != null)
                mc.Model.Scaling = scm.Scale;
        }
    }
}
