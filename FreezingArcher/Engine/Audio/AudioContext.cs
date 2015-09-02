//
//  AudioContext.cs
//
//  Author:
//       david <${AuthorEmail}>
//
//  Copyright (c) 2015 david
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
using System.Collections.Generic;
using FreezingArcher.Audio;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Content;

namespace FreezingArcher.Audio
{
    public enum SoundAction
    {
        Nothing,
        Play,
        PlayOverdub,
        Pause,
        Stop
    }

    public class SoundSourceDescription
    {
        public SoundSourceDescription(Source source, SoundAction sa, Entity entity = null)
        {
            Entity = entity;
            SoundSource = source;
            SoundAction = sa;
        }
            
        public Entity Entity;
        public Source SoundSource;
        public SoundAction SoundAction;
    }

    public class AudioContext : IMessageConsumer 
    {
        public MessageProvider MessageProvider { get; private set;}

        public AudioContext (MessageProvider mpv)
        {
            MessageProvider = mpv;

            SoundDictionary = new Dictionary<MessageId, List<SoundSourceDescription>> ();
            _ValidMessages = new List<int>();

            MessageProvider += this;
        }

        public Filter GlobalFilter { get; set; }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if(SoundDictionary.ContainsKey((MessageId)msg.MessageId))
            {
                List<SoundSourceDescription> ssds = SoundDictionary[(MessageId)msg.MessageId];
                if (ssds != null)
                {
                    foreach (SoundSourceDescription ssd in ssds)
                    {
                        TransformComponent tfc = null;

                        if (ssd.Entity != null)
                            tfc = ssd.Entity.GetComponent<TransformComponent> ();
                        else
                        {
                            if (msg.MessageId == (int) MessageId.BeginWallMovement)
                            {
                                BeginWallMovementMessage bwmm = msg as BeginWallMovementMessage;
                                tfc = bwmm.Entity.GetComponent<TransformComponent> ();
                            }
                        }

                        if (tfc != null)
                            ssd.SoundSource.Position = tfc.Position;

                        switch (ssd.SoundAction)
                        {
                        case SoundAction.Play:
                            if (ssd.SoundSource.GetState () != SourceState.Playing)
                                ssd.SoundSource.Play ();
                            break;

                        case SoundAction.PlayOverdub:
                            ssd.SoundSource.Play ();
                            break;

                        case SoundAction.Pause:
                            ssd.SoundSource.Pause ();
                            break;

                        case SoundAction.Stop:
                            ssd.SoundSource.Stop ();
                            break;
                        }
                    }
                }
            }
        }

        public void RegisterSoundPlaybackOnMessage(MessageId Message, SoundSourceDescription desc)
        {
            MessageProvider.UnregisterMessageConsumer (this);

            if (SoundDictionary.ContainsKey (Message))
            {
                if (SoundDictionary [Message] != null)
                    SoundDictionary [Message].Add (desc);
                else
                {
                    SoundDictionary [Message] = new List<SoundSourceDescription> ();
                    SoundDictionary [Message].Add (desc);
                }
            }
            else
            {
                SoundDictionary.Add (Message, new List<SoundSourceDescription> ());
                SoundDictionary [Message].Add (desc);
            }

            _ValidMessages.Add ((int) Message);

            MessageProvider.RegisterMessageConsumer (this);
        }

        Dictionary<MessageId, List<SoundSourceDescription>> SoundDictionary;

        public List<int> _ValidMessages;

        public int[] ValidMessages
        {
            get
            {
                return _ValidMessages.ToArray();
            }
        }

        #endregion
    }
}

