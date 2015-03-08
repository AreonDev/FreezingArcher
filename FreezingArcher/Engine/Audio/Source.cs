//
//  Source.cs
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
using Pencil.Gaming.MathUtils;
using FreezingArcher.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace FreezingArcher.Audio
{
    public enum SourceState
    {
        Initial = 4113,
        Playing,
        Paused,
        Stopped
    }

    public enum SourceType
    {
        Static = 4136,
        Streaming,
        Undetermined = 4144
    }

    public enum SourceGroup : byte
    {
        Music,
        Environment,
        Effect,
        Voice,
        VoiceChat,
        Custom1,
        Custom2,
        Custom3
    }

    public class Source : IResource, IManageable
    {
        public Sound[] GetProcessedSounds ()
        {
            return null;
        }

        public SourceState GetState ()
        {
            return SourceState.Initial;
        }

        public SourceType GetSourceType ()
        {
            return SourceType.Undetermined;
        }

        internal uint GetId ()
        {
            return 0;
        }

        public Sound[] AudioFiles { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the position, velocity, cone and direction of this
        /// <see cref="FreezingArcher.Sound.Source"/> are to be interpreted relative to the listener position.
        /// </summary>
        /// <value><c>true</c> if relative; otherwise, <c>false</c>.</value>
        public bool Relative { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Sound.Source"/> is looping.
        /// </summary>
        /// <value><c>true</c> if looping; otherwise, <c>false</c>.</value>
        public bool Loop { get; set; }

        public float ReferenceDistance { get; set; }
        public float MaxDistance { get; set; }
        public float RolloffFactor { get; set; }
        public float Pitch { get; set; }
        public float Gain { get; set; }
        public float MinGain { get; set; }
        public float MaxGain { get; set; }
        public float ConeInnerAngle { get; set; }
        public float ConeOuterAngle { get; set; }
        public float ConeOuterGain { get; set; }
        //TODO Effects

        #region IResource implementation
        public event Handler NeedsLoad;

        public List<Action> GetInitJobs (List<Action> list)
        {
            return list;
        }

        public List<Action> GetLoadJobs (List<Action> list, Handler reloader)
        {
            return list;
        }

        public void Destroy ()
        {}

        public bool Loaded { get; protected set; }
        #endregion

        #region IManageable implementation
        public string Name { get; set; }
        #endregion
    }
}
