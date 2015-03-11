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
using Pencil.Gaming.MathUtils;

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
    public class Source : IResource, IManageable
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Source_";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Source"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="groupGains">Reference to the group gains.</param>
        /// <param name="sounds">Sounds.</param>
        internal Source (string name, Dictionary<SourceGroup, float> groupGains, params Sound[] sounds)
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName + Name, "Creating new audio source instance '{0}'", Name);
            Name = name;
            Sounds = sounds;
            GroupGains = groupGains;
            Loaded = false;
        }

        /// <summary>
        /// The openal source identifier (name).
        /// </summary>
        protected uint AlSourceId;

        /// <summary>
        /// Load this resource.
        /// </summary>
        internal void Load ()
        {
            Loaded = false;
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName + Name, "Loading audio source '{0}'...", Name);
            AL.GenSources (1, out AlSourceId);
            if (Sounds.Length <= 0)
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name, "You have not specified any sounds for " +
                        "this audio source!");

            uint[] bids = new uint[Sounds.Length];
            for (int i = 0; i < bids.Length; i++)
                if (Sounds[i] != null)
                    bids[i] = Sounds[i].GetId ();

            AL.SourceQueueBuffers (AlSourceId, bids.Length, bids);
            Loaded = true;
        }

        /// <summary>
        /// Gets the currently played sound.
        /// </summary>
        /// <returns>The currently played sound.</returns>
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

        /// <summary>
        /// Gets the source state.
        /// </summary>
        /// <returns>The source state.</returns>
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

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <returns>The source type.</returns>
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

        /// <summary>
        /// Gets the openal identifier (name).
        /// </summary>
        /// <returns>The openal identifier (name).</returns>
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

        /// <summary>
        /// Play the specified sounds.
        /// </summary>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void Play (bool shuffleSounds = false)
        {
            AL.SourcePlay (AlSourceId);
        }

        /// <summary>
        /// Plays sound at given time offset.
        /// </summary>
        /// <param name="timeOffset">Time offset.</param>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlayAt (TimeSpan timeOffset, bool shuffleSounds = false)
        {
            //TODO
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="soundName">Sound name.</param>
        /// <param name="proceedWithNextSoundWhenFinished">If set to <c>true</c> proceed with next sound when finished.</param>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlaySound (string soundName, bool proceedWithNextSoundWhenFinished = false,
            bool shuffleSounds = false)
        {
            //TODO
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="soundIndex">Sound index.</param>
        /// <param name="proceedWithNextSoundWhenFinished">If set to <c>true</c> proceed with next sound when finished.</param>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlaySound (int soundIndex, bool proceedWithNextSoundWhenFinished = false,
            bool shuffleSounds = false)
        {
            //TODO
        }

        /// <summary>
        /// Plays the given sounds from playlist.
        /// </summary>
        /// <param name="soundPlaylist">Playlist.</param>
        public void PlaySounds (string[] soundPlaylist)
        {
            //TODO
        }

        /// <summary>
        /// Plays the sounds from playlist.
        /// </summary>
        /// <param name="soundIndexPlaylist">Playlist.</param>
        public void PlaySounds (int[] soundIndexPlaylist)
        {
            //TODO
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

        /// <summary>
        /// Gets or sets the velocity of this audio source.
        /// </summary>
        /// <value>The velocity.</value>
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

        /// <summary>
        /// Gets or sets the direction of this audio source.
        /// </summary>
        /// <value>The direction.</value>
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

        /// <summary>
        /// Gets or sets the reference distance of this audio source.
        /// </summary>
        /// <value>The reference distance.</value>
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

        /// <summary>
        /// Gets or sets the max distance.
        /// </summary>
        /// <value>The max distance.</value>
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

        /// <summary>
        /// Gets or sets the rolloff factor.
        /// </summary>
        /// <value>The rolloff factor.</value>
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

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get Gain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                /*float gain;
                AL.GetSource (AlSourceId, ALSourcef.Gain, out gain);
                return gain;*/
                return CleanGain;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set Gain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get MinGain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float mg;
                AL.GetSource (AlSourceId, ALSourcef.MinGain, out mg);
                return mg;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set MinGain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get MaxGain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float gain;
                AL.GetSource (AlSourceId, ALSourcef.MaxGain, out gain);
                return gain;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set MaxGain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get ConeInnerAngle property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float angle;
                AL.GetSource (AlSourceId, ALSourcef.ConeInnerAngle, out angle);
                return angle;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set ConeInnerAngle property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get ConeOuterAngle property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float angle;
                AL.GetSource (AlSourceId, ALSourcef.ConeOuterAngle, out angle);
                return angle;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set ConeOuterAngle property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to get ConeOuterGain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float gain;
                AL.GetSource (AlSourceId, ALSourcef.ConeOuterGain, out gain);
                return gain;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                        "Trying to set ConeOuterGain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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

        #region IResource implementation
        /// <summary>
        /// Fire this event when you need the binded load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        public event Handler NeedsLoad;

        /// <summary>
        /// Gets the init jobs. The init jobs may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            return list;
        }

        /// <summary>
        /// Gets the load jobs. The load jobs will be executed sequentially in the gl thread.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, Handler reloader)
        {
            NeedsLoad = reloader;
            list.Add (Load);
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the garbage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName + Name, "Destroying audio source '{0}'", Name);
            AL.DeleteSources (1, ref AlSourceId);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Audio.Source"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }
        #endregion

        #region IManageable implementation
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion
    }
}
