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
using Pencil.Gaming.Audio;
using FreezingArcher.Output;

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
        public static readonly string ClassName = "Source_";

        public Source (string name, params Sound[] sounds)
        {
            Name = name;
            Sounds = sounds;
        }

        protected uint AlSourceId;

        public Sound GetCurrentlyPlayedSound ()
        {
            if (!Loaded)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                    "Trying to read BufferProcessed property before resource was loaded!");
                throw new InvalidOperationException ();
            }

            int idx;
            AL.GetSource (AlSourceId, ALGetSourcei.BuffersProcessed, out idx);
            return Sounds[idx];
        }

        public SourceState GetState ()
        {
            if (!Loaded)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                    "Trying to read State property before resource was loaded!");
                throw new InvalidOperationException ();
            }

            int state;
            AL.GetSource (AlSourceId, ALGetSourcei.SourceState, out state);
            return (SourceState) state;
        }

        public SourceType GetSourceType ()
        {
            if (!Loaded)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                    "Trying to read SourceType property before resource was loaded!");
                throw new InvalidOperationException ();
            }

            int type;
            AL.GetSource (AlSourceId, ALGetSourcei.SourceType, out type);
            return (SourceType) type;
        }

        internal uint GetId ()
        {
            if (!Loaded)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                    "Trying to read openal id (name) property before resource was loaded!");
                throw new InvalidOperationException ();
            }

            return AlSourceId;
        }

        public void Play (bool shuffleSounds = false)
        {
            AL.SourcePlay (AlSourceId);
        }

        public void PlayAt (TimeSpan timeOffset, bool shuffleSounds = false)
        {
            //TODO
        }

        public void PlaySound (string soundName, bool proceedWithNextSoundWhenFinished = false,
            bool shuffleSounds = false)
        {
            //TODO
        }

        public void PlaySound (int soundIndex, bool proceedWithNextSoundWhenFinished = false,
            bool shuffleSounds = false)
        {
            //TODO
        }

        public void PlaySounds (string[] soundPlaylist)
        {
            //TODO
        }

        public void PlaySounds (int[] soundIndexPlaylist)
        {
            //TODO
        }

        public void Pause ()
        {
            AL.SourcePause (AlSourceId);
        }

        public void Stop ()
        {
            AL.SourceStop (AlSourceId);
        }

        public void Rewind ()
        {
            AL.SourceRewind (AlSourceId);
        }

        public Sound[] Sounds { get; protected set; }

        public Vector3 Position
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read Position property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float px, py, pz;

                AL.GetSource (AlSourceId, ALSource3f.Position, out px, out py, out pz);
                return new Vector3 (px, py, pz);
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set Position property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSource3f.Position, value.X, value.Y, value.Z);
            }
        }

        public Vector3 Velocity
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read Velocity property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float vx, vy, vz;
                AL.GetSource (AlSourceId, ALSource3f.Velocity, out vx, out vy, out vz);
                return new Vector3 (vx, vy, vz);
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set Velocity property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSource3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        public Vector3 Direction
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read Direction property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float dx, dy, dz;
                AL.GetSource (AlSourceId, ALSource3f.Direction, out dx, out dy, out dz);
                return new Vector3 (dx, dy, dz);
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set Direction property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSource3f.Direction, value.X, value.Y, value.Z);
            }
        }

        //TODO: On Filter set, call openal update filters
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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read SourceRelative property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                bool rel;
                AL.GetSource (AlSourceId, ALSourceb.SourceRelative, out rel);
                return rel;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read openal id (name) property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read Looping property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                bool loop;
                AL.GetSource (AlSourceId, ALSourceb.Looping, out loop);
                return loop;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read Looping property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSourceb.Looping, value);
            }
        }

        public float ReferenceDistance
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read ReferenceDistance property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float dis;
                AL.GetSource (AlSourceId, ALSourcef.ReferenceDistance, out dis);
                return dis;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set ReferenceDistance property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSourcef.ReferenceDistance, value);
            }
        }

        public float MaxDistance
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read MaxDistance property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float dis;
                AL.GetSource (AlSourceId, ALSourcef.MaxDistance, out dis);
                return dis;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set MaxDistance property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSourcef.MaxDistance, value);
            }
        }

        public float RolloffFactor
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read RolloffFactor property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float fac;
                AL.GetSource (AlSourceId, ALSourcef.RolloffFactor, out fac);
                return fac;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set RolloffFactor property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSourcef.RolloffFactor, value);
            }
        }

        public float Pitch
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to read Pitch property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float pitch;
                AL.GetSource (AlSourceId, ALSourcef.Pitch, out pitch);
                return pitch;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set Pitch property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSourcef.Pitch, value);
            }
        }

        public float Gain
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get Gain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float gain;
                AL.GetSource (AlSourceId, ALSourcef.Gain, out gain);
                return gain;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set Gain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                AL.Source (AlSourceId, ALSourcef.Gain, value);
            }
        }

        public float MinGain { get; set; }
        public float MaxGain { get; set; }
        public float ConeInnerAngle { get; set; }
        public float ConeOuterAngle { get; set; }
        public float ConeOuterGain { get; set; }

        //TODO Group gain

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
        {
            AL.DeleteSources (1, ref AlSourceId);
        }

        public bool Loaded { get; protected set; }
        #endregion

        #region IManageable implementation
        public string Name { get; set; }
        #endregion
    }
}
