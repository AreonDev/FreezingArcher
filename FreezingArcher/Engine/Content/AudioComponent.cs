//
//  AudioComponent.cs
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
using FreezingArcher.Audio;
using FreezingArcher.Messaging;
using System;
using System.Collections.Generic;

namespace FreezingArcher.Content
{
    public enum AudioComponentReaction
    {
        Nothing = 0,
        Play,
        Stop,
        Pause,
        Custom
    }

    public class AudioComponentEvent
    {
        public AudioComponentEvent(MessageId msg, AudioComponentReaction acr, 
            float cooldown, Action act = null, Action prepact = null)
        {
            MessageToReact = msg;
            Event = acr;
            EventCoolDownTime = cooldown;
            CustomEventAction = act;
            PrepareEventAction = prepact;

            TimeSpan = TimeSpan.Zero;
        }

        public MessageId MessageToReact;
        public AudioComponentReaction Event;

        public Action PrepareEventAction;
        public Action CustomEventAction;

        internal TimeSpan TimeSpan;
        public float EventCoolDownTime;
    }

    public sealed class AudioComponent : EntityComponent
    {
        #region defaults

        public static readonly AudioManager DefaultAudioManager = null;
        public static readonly Source DefaultSoundSource = null;
        public static readonly List<AudioComponentEvent> DefaultAudioComponentEvents = new List<AudioComponentEvent>();

        #endregion

        public AudioManager AudioManager { get; set;}
        public Source SoundSource { get; set; }
        public List<AudioComponentEvent> AudioComponentEvents;
    }
}

