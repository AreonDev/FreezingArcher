//
//  ParticleSystem.cs
//
//  Author:
//       dboeg <>
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
    public sealed class ParticleSystem : EntitySystem
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
            NeededComponents = new[] { typeof(TransformComponent), typeof(ParticleComponent) };

            onPositionChangedHandler = (pos) => {
                var model = Entity.GetComponent<ParticleComponent>().Emitter;
                if (model != null)
                    model.SpawnPosition = pos;
            };

            //internalValidMessages = new[] { (int) MessageId.PositionChanged,
            //    (int) MessageId.RotationChanged, (int) MessageId.ScaleChanged };
            internalValidMessages = new int[]{(int)MessageId.Update};
            messageProvider += this;
        }

        Action<Vector3> onPositionChangedHandler;
        /// <summary>
        /// This method is called when the entity is fully intialized.
        /// </summary>
        public override void PostInit()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged += onPositionChangedHandler;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update)
            {
                UpdateMessage um = msg as UpdateMessage;

                var emit = Entity.GetComponent<ParticleComponent> ().Emitter;
                if (emit != null)
                    emit.Update ((float)um.TimeStamp.TotalSeconds);
            }
        }

        public override void Destroy()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged -= onPositionChangedHandler;

            var ParticleComponent = Entity.GetComponent<ParticleComponent> ();

            ParticleComponent.Emitter.Destroy ();
            ParticleComponent.Particle.Dispose ();

            base.Destroy();
        }
    }
}

