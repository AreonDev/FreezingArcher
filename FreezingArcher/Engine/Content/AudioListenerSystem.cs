//
//  AudioListenerSystem.cs
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
using FreezingArcher.Core;
using FreezingArcher.Audio;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;
using Jitter;
using Jitter.Dynamics;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Model system. Updates scene data.
    /// </summary>
    public sealed class AudioListenerSystem : EntitySystem
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
            NeededComponents = new[] { typeof(TransformComponent), typeof(AudioComponent), typeof(PhysicsComponent)};

            onPositionChangedHandler = (pos) => {
                RigidBody body = Entity.GetComponent<PhysicsComponent>().RigidBody;
                AudioManager model = Entity.GetComponent<AudioComponent>().AudioManager;
                if (model != null && body != null)
                {
                    model.Listener.Position = pos;
                    model.Listener.Velocity = body.LinearVelocity.ToFreezingArcherVector();
                }
            };

            onRotationChangedHandler = (rot) => {
                //var model = Entity.GetComponent<AudioComponent>().AudioManager;
                //if (model != null)
                //    model.Listener.Orientation = new FreezingArcher.Core.Pair<Vector3, UpVector>(
            };

            onScaleChangedHandler = (scale) => {
                //var model = Entity.GetComponent<ModelComponent>().Model;
                //if (model != null)
                //    model.Scaling = scale;
            };

            internalValidMessages = new int[0];
            messageProvider += this;
        }

        Action<Vector3> onPositionChangedHandler;

        Action<Quaternion> onRotationChangedHandler;

        Action<Vector3> onScaleChangedHandler;

        /// <summary>
        /// This method is called when the entity is fully intialized.
        /// </summary>
        public override void PostInit()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged += onPositionChangedHandler;
            transform.OnRotationChanged += onRotationChangedHandler;
            transform.OnScaleChanged += onScaleChangedHandler;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
        }

        public override void Destroy()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged -= onPositionChangedHandler;
            transform.OnRotationChanged -= onRotationChangedHandler;
            transform.OnScaleChanged -= onScaleChangedHandler;
            base.Destroy();
        }
    }
}


