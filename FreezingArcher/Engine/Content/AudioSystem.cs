//
//  AudioSystem.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;
using FreezingArcher.Audio;
using Jitter.Dynamics;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Model system. Updates scene data.
    /// </summary>
    public sealed class AudioSystem : EntitySystem
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
            NeededComponents = new[] { typeof(AudioComponent), typeof(TransformComponent), typeof(PhysicsComponent)};

            //internalValidMessages = new[] { (int) MessageId.PositionChanged,
            //    (int) MessageId.RotationChanged, (int) MessageId.ScaleChanged };


            onPositionChangedHandler = (pos) =>
            {
                RigidBody body = Entity.GetComponent<PhysicsComponent> ().RigidBody;
                var model = Entity.GetComponent<AudioComponent> ();
                if (model != null && body != null)
                {
                    model.SoundSource.Position = pos;
                    model.SoundSource.Velocity = body.LinearVelocity.ToFreezingArcherVector ();
                }
            };

            internalValidMessages = new[] {(int)MessageId.All};
            messageProvider += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            AudioComponent ac = Entity.GetComponent<AudioComponent> ();

            if (ac.AudioManager == null)
                return;

            if (msg.MessageId == (int) MessageId.Update)
            {
                UpdateMessage um = msg as UpdateMessage;

                foreach (AudioComponentEvent ace in ac.AudioComponentEvents)
                {
                    ace.TimeSpan += um.TimeStamp;

                    if(ace.PrepareEventAction != null)
                        ace.PrepareEventAction ();
                }
            }

            //Trigger Events
            foreach (AudioComponentEvent ace in ac.AudioComponentEvents)
            {
                if ((int)ace.MessageToReact == msg.MessageId)
                {
                    if (ace.TimeSpan.TotalMilliseconds >= ace.EventCoolDownTime)
                    {
                        ace.TimeSpan = TimeSpan.Zero;

                        switch (ace.Event)
                        {
                        case AudioComponentReaction.Play:
                            ac.SoundSource.Play ();
                            break;

                        case AudioComponentReaction.Pause:
                            ac.SoundSource.Pause ();
                            break;

                        case AudioComponentReaction.Stop:
                            ac.SoundSource.Stop ();
                            break;

                        case AudioComponentReaction.Custom:
                            if (ace.CustomEventAction != null)
                                ace.CustomEventAction ();
                            break;
                        }
                    }
                }
            }
        }

        Action<Vector3> onPositionChangedHandler;

        public override void PostInit()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged += onPositionChangedHandler;
            //transform.OnRotationChanged += onRotationChangedHandler;
            //transform.OnScaleChanged += onScaleChangedHandler;
        }

        public override void Destroy()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged -= onPositionChangedHandler;
            //transform.OnRotationChanged -= onRotationChangedHandler;
            //transform.OnScaleChanged -= onScaleChangedHandler;

            var ac = Entity.GetComponent<AudioComponent> ();
            if (ac.SoundSource != null)
                ac.SoundSource.Dispose ();

            base.Destroy();
        }
    }
}
