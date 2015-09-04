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

namespace FreezingArcher.Content
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

        private Filter _GlobalFilter;

        public Filter GlobalFilter 
        { 
            get
            {
                return _GlobalFilter;
            }

            set
            {
                _GlobalFilter = value;

                foreach (List<SoundSourceDescription> ssds in SoundDictionary.Values)
                {
                    ssds.ForEach (x => x.SoundSource.Filter = _GlobalFilter);
                }
            }
        }

        public void StopAllSounds()
        {
            foreach (List<SoundSourceDescription> ssds in SoundDictionary.Values)
            {
                ssds.ForEach (x => x.SoundSource.Stop ());
            }
        }

        public void PauseAllSounds()
        {
            foreach (List<SoundSourceDescription> ssds in SoundDictionary.Values)
            {
                ssds.ForEach (x => x.SoundSource.Pause ());
            }
        }

        public void ReplayAllPausedSounds()
        {
            foreach (List<SoundSourceDescription> ssds in SoundDictionary.Values)
            {
                ssds.ForEach (x => {if(x.SoundSource.GetState() == SourceState.Paused) x.SoundSource.Play ();});
            }
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if(SoundDictionary.ContainsKey((MessageId)msg.MessageId))
            {
                List<SoundSourceDescription> ssds = SoundDictionary[(MessageId)msg.MessageId];
                if (ssds != null)
                {
                    bool can_play = true;

                    foreach (SoundSourceDescription ssd in ssds)
                    {
                        TransformComponent tfc = null;

                        if (ssd.Entity != null)
                        {
                            tfc = ssd.Entity.GetComponent<TransformComponent> ();

                            if (msg.MessageId == (int) MessageId.AIAttack)
                            {
                                AIAttackMessage aam = msg as AIAttackMessage;
                                if (aam.Entity.InstId != ssd.Entity.InstId)
                                {
                                    can_play = false;
                                    continue;
                                }
                                else
                                    can_play = true;
                            }

                            ArtificialIntelligenceComponent aic = ssd.Entity.GetComponent<ArtificialIntelligenceComponent> ();
                            if (aic != null)
                            {
                                if (!aic.ArtificialIntelligence.Spawned)
                                {
                                    can_play = false;
                                    continue;
                                }
                                else
                                    can_play = true;
                            }
                        }
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

                        if (!can_play)
                            return;

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

            if(!_ValidMessages.Contains((int)Message))
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

