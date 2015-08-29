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
using System;
using System.Collections.Generic;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Output;
using Pencil.Gaming.Audio;
using FreezingArcher.Math;
using FreezingArcher.Core;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// Audio source state.
    /// </summary>
    public enum SourceState
    {
        /// <summary>
        /// The initial state.
        /// </summary>
        Initial = 4113,
        /// <summary>
        /// The playing state.
        /// </summary>
        Playing,
        /// <summary>
        /// The paused state.
        /// </summary>
        Paused,
        /// <summary>
        /// The stopped state.
        /// </summary>
        Stopped
    }

    /// <summary>
    /// Audio source type.
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// The static source type.
        /// </summary>
        Static = 4136,
        /// <summary>
        /// The streaming source type.
        /// </summary>
        Streaming,
        /// <summary>
        /// The undetermined source type.
        /// </summary>
        Undetermined = 4144
    }

    /// <summary>
    /// Audio source group.
    /// </summary>
    public enum SourceGroup : byte
    {
        /// <summary>
        /// The music group.
        /// </summary>
        Music,
        /// <summary>
        /// The environment group.
        /// </summary>
        Environment,
        /// <summary>
        /// The effect group.
        /// </summary>
        Effect,
        /// <summary>
        /// The voice group.
        /// </summary>
        Voice,
        /// <summary>
        /// The voice chat group.
        /// </summary>
        VoiceChat,
        /// <summary>
        /// The first custom group.
        /// </summary>
        Custom1,
        /// <summary>
        /// The second custom group.
        /// </summary>
        Custom2,
        /// <summary>
        /// The third custom group.
        /// </summary>
        Custom3
    }

    /// <summary>
    /// Audio source class.
    /// </summary>
    public class Source : IManageable, IDisposable
    {
        #region Effects
        //Cache for audio routing to aux sends
        private RoutingEntry[] auxSends;

        //retrieve free routing number (1..max aux sends), -1 if all full
        internal int GetRoutingId()
        {
            for (int j = 0; j < auxSends.Length; j++)
                if (auxSends[j] == null)
                    return j;

            return -1;
        }

        //mark route as used or free route
        internal void setRoute(RoutingEntry re, int num)
        {
            this.auxSends[num] = re;
        }
        #endregion

        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Source";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Source"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="groupGains">Reference to the group gains.</param>
        /// <param name="sounds">Sounds.</param>
        internal Source (string name, Dictionary<SourceGroup, float> groupGains, params Sound[] sounds)
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Creating new audio source instance '{0}'", name);
            Name = name;
            Sounds = sounds;
            GroupGains = groupGains;
            auxSends = new RoutingEntry[AL.MaxALCAuxiliarySends()];
            Load ();
        }


        /// <summary>
        /// The openal source identifier (name).
        /// </summary>
        protected uint AlSourceId;

        /// <summary>
        /// Load this resource.
        /// </summary>
        void Load ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading audio source '{0}'...", Name);
            AL.GenSources (1, out AlSourceId);
            if (Sounds.Length <= 0)
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName, Status.BadData, "You have not specified any sounds for " +
                        "this audio source!");

            uint[] bids = new uint[Sounds.Length];
            for (int i = 0; i < bids.Length; i++)
                if (Sounds[i] != null)
                    bids[i] = Sounds[i].GetId ();

            AL.SourceQueueBuffers (AlSourceId, bids.Length, bids);
        }

        /// <summary>
        /// Gets the currently played sound.
        /// </summary>
        /// <returns>The currently played sound.</returns>
        public Sound GetCurrentlyPlayedSound ()
        {
            int idx;
            AL.GetSource (AlSourceId, ALGetSourcei.BuffersProcessed, out idx);
            return Sounds[idx];
        }

        /// <summary>
        /// Gets the source state.
        /// </summary>
        /// <returns>The source state.</returns>
        public SourceState GetState ()
        {
            int state;
            AL.GetSource (AlSourceId, ALGetSourcei.SourceState, out state);
            return (SourceState) state;
        }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <returns>The source type.</returns>
        public SourceType GetSourceType ()
        {
            int type;
            AL.GetSource (AlSourceId, ALGetSourcei.SourceType, out type);
            return (SourceType) type;
        }

        /// <summary>
        /// Gets the openal identifier (name).
        /// </summary>
        /// <returns>The openal identifier (name).</returns>
        internal uint GetId ()
        {
            return AlSourceId;
        }

        /// <summary>
        /// Play the specified sounds.
        /// </summary>
        public void Play ()
        {
            AL.SourcePlay (AlSourceId);
        }

        /// <summary>
        /// Plays sound at given time offset.
        /// </summary>
        /// <param name="timeOffset">Time offset.</param>
        public void PlayAt (TimeSpan timeOffset)
        {
            AL.Source (AlSourceId, ALSourcef.SecOffset, timeOffset.Seconds);
            Play ();
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="soundName">Sound name.</param>
        public void PlayAt (string soundName)
        {
            int bytes = 0;
            for (int i = 0; Sounds[i].Name != soundName; i++)
                bytes += Sounds[i].GetSize ();

            AL.Source (AlSourceId, ALSourcei.ByteOffset, bytes);
            Play ();
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="soundIndex">Sound index.</param>
        public void PlayAt (int soundIndex)
        {
            int bytes = 0;
            for (int i = 0; i < soundIndex; i++)
                bytes += Sounds[i].GetSize ();

            AL.Source (AlSourceId, ALSourcei.ByteOffset, bytes);
            Play ();
        }

        /// <summary>
        /// Pause this audio source.
        /// </summary>
        public void Pause ()
        {
            AL.SourcePause (AlSourceId);
        }

        /// <summary>
        /// Stop this audio source.
        /// </summary>
        public void Stop ()
        {
            AL.SourceStop (AlSourceId);
        }

        /// <summary>
        /// Rewind this audio source.
        /// </summary>
        public void Rewind ()
        {
            AL.SourceRewind (AlSourceId);
        }

        /// <summary>
        /// Gets or sets the binded sounds.
        /// </summary>
        /// <value>The sounds.</value>
        public Sound[] Sounds { get; protected set; }

        /// <summary>
        /// Gets or sets the position of this audio source.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get
            {
                float px, py, pz;

                AL.GetSource (AlSourceId, ALSource3f.Position, out px, out py, out pz);
                return new Vector3 (px, py, pz);
            }
            set
            {
                AL.Source (AlSourceId, ALSource3f.Position, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// Gets or sets the velocity of this audio source.
        /// </summary>
        /// <value>The velocity.</value>
        public Vector3 Velocity
        {
            get
            {
                float vx, vy, vz;
                AL.GetSource (AlSourceId, ALSource3f.Velocity, out vx, out vy, out vz);
                return new Vector3 (vx, vy, vz);
            }
            set
            {
                AL.Source (AlSourceId, ALSource3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// Gets or sets the direction of this audio source.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3 Direction
        {
            get
            {
                float dx, dy, dz;
                AL.GetSource (AlSourceId, ALSource3f.Direction, out dx, out dy, out dz);
                return new Vector3 (dx, dy, dz);
            }
            set
            {
                AL.Source (AlSourceId, ALSource3f.Direction, value.X, value.Y, value.Z);
            }
        }

        //TODO: On Filter set, call openal update filters

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public Filter Filter { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether the position, velocity, cone and direction of this
        /// <see cref="FreezingArcher.Audio.Source"/> are to be interpreted relative to the listener position.
        /// </summary>
        /// <value><c>true</c> if relative; otherwise, <c>false</c>.</value>
        public bool Relative
        {
            get
            {
                bool rel;
                AL.GetSource (AlSourceId, ALSourceb.SourceRelative, out rel);
                return rel;
            }
            set
            {
                AL.Source (AlSourceId, ALSourceb.SourceRelative, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Audio.Source"/> is looping.
        /// </summary>
        /// <value><c>true</c> if looping; otherwise, <c>false</c>.</value>
        public bool Loop
        {
            get
            {
                bool loop;
                AL.GetSource (AlSourceId, ALSourceb.Looping, out loop);
                return loop;
            }
            set
            {
                AL.Source (AlSourceId, ALSourceb.Looping, value);
            }
        }

        /// <summary>
        /// Gets or sets the reference distance of this audio source.
        /// </summary>
        /// <value>The reference distance.</value>
        public float ReferenceDistance
        {
            get
            {
                float dis;
                AL.GetSource (AlSourceId, ALSourcef.ReferenceDistance, out dis);
                return dis;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.ReferenceDistance, value);
            }
        }

        /// <summary>
        /// Gets or sets the max distance.
        /// </summary>
        /// <value>The max distance.</value>
        public float MaxDistance
        {
            get
            {
                float dis;
                AL.GetSource (AlSourceId, ALSourcef.MaxDistance, out dis);
                return dis;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.MaxDistance, value);
            }
        }

        /// <summary>
        /// Gets or sets the rolloff factor.
        /// </summary>
        /// <value>The rolloff factor.</value>
        public float RolloffFactor
        {
            get
            {
                float fac;
                AL.GetSource (AlSourceId, ALSourcef.RolloffFactor, out fac);
                return fac;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.RolloffFactor, value);
            }
        }

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        public float Pitch
        {
            get
            {
                float pitch;
                AL.GetSource (AlSourceId, ALSourcef.Pitch, out pitch);
                return pitch;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.Pitch, value);
            }
        }

        /// <summary>
        /// The gain without group gain.
        /// </summary>
        protected float CleanGain;

        /// <summary>
        /// Gets or sets the gain.
        /// </summary>
        /// <value>The gain.</value>
        public float Gain
        {
            get
            {
                /*float gain;
                AL.GetSource (AlSourceId, ALSourcef.Gain, out gain);
                return gain;*/
                return CleanGain;
            }
            set
            {
                CleanGain = value;
                AL.Source (AlSourceId, ALSourcef.Gain, value * GroupGain);
            }
        }

        /// <summary>
        /// Gets or sets the minimum gain.
        /// </summary>
        /// <value>The minimum gain.</value>
        public float MinGain
        {
            get
            {
                float mg;
                AL.GetSource (AlSourceId, ALSourcef.MinGain, out mg);
                return mg;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.MinGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the max gain.
        /// </summary>
        /// <value>The max gain.</value>
        public float MaxGain
        {
            get
            {
                float gain;
                AL.GetSource (AlSourceId, ALSourcef.MaxGain, out gain);
                return gain;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.MaxGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the cone inner angle.
        /// </summary>
        /// <value>The cone inner angle.</value>
        public float ConeInnerAngle
        {
            get
            {
                float angle;
                AL.GetSource (AlSourceId, ALSourcef.ConeInnerAngle, out angle);
                return angle;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.ConeInnerAngle, value);
            }
        }

        /// <summary>
        /// Gets or sets the cone outer angle.
        /// </summary>
        /// <value>The cone outer angle.</value>
        public float ConeOuterAngle
        {
            get
            {
                float angle;
                AL.GetSource (AlSourceId, ALSourcef.ConeOuterAngle, out angle);
                return angle;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.ConeOuterAngle, value);
            }
        }

        /// <summary>
        /// Gets or sets the cone outer gain.
        /// </summary>
        /// <value>The cone outer gain.</value>
        public float ConeOuterGain
        {
            get
            {
                float gain;
                AL.GetSource (AlSourceId, ALSourcef.ConeOuterGain, out gain);
                return gain;
            }
            set
            {
                AL.Source (AlSourceId, ALSourcef.ConeOuterGain, value);
            }
        }

        /// <summary>
        /// The group gains.
        /// </summary>
        protected Dictionary<SourceGroup, float> GroupGains;

        /// <summary>
        /// The source group.
        /// </summary>
        protected SourceGroup SourceGroup;

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public SourceGroup Group
        {
            get
            {
                return SourceGroup;
            }
            set
            {
                SourceGroup = value;
                Gain = CleanGain;
            }
        }

        /// <summary>
        /// Gets the group gain.
        /// </summary>
        /// <value>The group gain.</value>
        public float GroupGain
        {
            get
            {
                float gain;
                if (!GroupGains.TryGetValue (Group, out gain))
                    gain = 1;
                return gain;
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="FreezingArcher.Audio.Source"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="FreezingArcher.Audio.Source"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="FreezingArcher.Audio.Source"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="FreezingArcher.Audio.Source"/> so the garbage collector can reclaim the memory that the
        /// <see cref="FreezingArcher.Audio.Source"/> was occupying.</remarks>
        public void Dispose ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Destroying audio source '{0}'", Name);
            AL.DeleteSources (1, ref AlSourceId);
        }

        #region IManageable implementation
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion
    }
}
