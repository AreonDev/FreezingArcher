//
//  AudioManager.cs
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
using System.IO;
using FreezingArcher.Core;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Output;
using Pencil.Gaming.Audio;
using Pencil.Gaming.MathUtils;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// Distance model for audio.
    /// </summary>
    public enum DistanceModel
    {
        /// <summary>
        /// No distance model.
        /// </summary>
        None,
        /// <summary>
        /// The inverse distance model.
        /// </summary>
        InverseDistance = 53249,
        /// <summary>
        /// The inverse distance clamped model.
        /// </summary>
        InverseDistanceClamped,
        /// <summary>
        /// The linear distance model.
        /// </summary>
        LinearDistance,
        /// <summary>
        /// The linear distance clamped model.
        /// </summary>
        LinearDistanceClamped,
        /// <summary>
        /// The exponent distance model.
        /// </summary>
        ExponentDistance,
        /// <summary>
        /// The exponent distance clamped model.
        /// </summary>
        ExponentDistanceClamped
    }

    /// <summary>
    /// Audio manager.
    /// </summary>
    public class AudioManager : IDisposable
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "AudioManager";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.AudioManager"/> class.
        /// </summary>
        public AudioManager ()
        {
            AL.MakeCurrent();
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Creating new AudioManager instance");
            Sounds = new List<Sound> ();
            Sources = new List<Source> ();
            SupportedEffects = new List<ALEffectType>();
            Listener = new Listener (Vector3.Zero, Vector3.Zero,
                new Pair<Vector3, UpVector> (Vector3.Zero, UpVector.UnitX), 1);
            Groups = new Dictionary<SourceGroup, float> ();
            Groups.Add (SourceGroup.Music, 1);
            Groups.Add (SourceGroup.Environment, 1);
            Groups.Add (SourceGroup.Effect, 1);
            Groups.Add (SourceGroup.Voice, 1);
            Groups.Add (SourceGroup.VoiceChat, 1);
            Groups.Add (SourceGroup.Custom1, 1);
            Groups.Add (SourceGroup.Custom2, 1);
            Groups.Add (SourceGroup.Custom3, 1);
            Routing = new AudioRouting();

            var efxTypes = Enum.GetValues(typeof(ALEffectType));
            var arr = new uint[1];
            AL.GenEffects(arr);
            foreach (ALEffectType item in efxTypes)
            {
                AL.EffectType(arr[0], item);
                if (AL.GetError() != (int)ALError.NoError)
                    Logger.Log.AddLogEntry(LogLevel.Debug, ClassName, "Effect type {0} is NOT supported.", item.ToString());
                else
                {
                    Logger.Log.AddLogEntry(LogLevel.Debug, ClassName, "Effect type {0} IS supported.", item.ToString());
                    SupportedEffects.Add(item);
                }
            }
            AL.DeleteEffects(arr);
        }

        /// <summary>
        /// The sounds registry.
        /// </summary>
        protected List<Sound> Sounds;

        /// <summary>
        /// The sources registry.
        /// </summary>
        protected List<Source> Sources;

        /// <summary>
        /// The source groups gain registry.
        /// </summary>
        protected Dictionary<SourceGroup, float> Groups;

        /// <summary>
        /// The supported effects of the OpenAL context
        /// </summary>
        protected List<ALEffectType> SupportedEffects;

        /// <summary>
        /// Gets the routing helper.
        /// </summary>
        /// <value>The routing helper.</value>
        public AudioRouting Routing {get; private set;}
        /// <summary>
        /// Sets the group gain.
        /// </summary>
        /// <param name="sourceGroup">Source group.</param>
        /// <param name="gain">Gain.</param>
        public void SetGroupGain (SourceGroup sourceGroup, float gain)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Setting group gain of '{0}' to {1}",
                sourceGroup.ToString (), gain.ToString ());
            Groups[sourceGroup] = gain;
        }

        /// <summary>
        /// Gets the group gain.
        /// </summary>
        /// <returns>The group gain.</returns>
        /// <param name="sourceGroup">Source group.</param>
        public float GetGroupGain (SourceGroup sourceGroup)
        {
            float gain;
            if (!Groups.TryGetValue (sourceGroup, out gain))
                gain = 0;
            return gain;
        }

        /// <summary>
        /// Loads a sound into the manager.
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="name">Name.</param>
        /// <param name="file">File.</param>
        public Sound LoadSound (string name, string file)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading new sound '{0}' from '{1}'", name , file);
            Sound snd = Sounds.Find (s => s.Name == name);

            if (snd == null)
            {
                snd = new Sound (name, file);

                Sounds.Add (snd);
            }

            return snd;
        }

        /// <summary>
        /// Loads a sound into the manager.
        /// </summary>
        /// <returns>The sound.</returns>
        /// <param name="name">Name.</param>
        /// <param name="file">File.</param>
        public Sound LoadSound (string name, FileInfo file)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading new sound '{0}' from '{1}'", name ,
                file.FullName);
            Sound snd = Sounds.Find (s => s.Name == name);

            if (snd == null)
            {
                snd = new Sound (name, file);

                Sounds.Add (snd);
            }

            return snd;
        }

        /// <summary>
        /// Loads sounds into the manager.
        /// </summary>
        /// <returns>The sounds.</returns>
        /// <param name="namesAndPaths">Names and paths.</param>
        public Sound[] LoadSounds (params Pair<string, string>[] namesAndPaths)
        {
            Sound[] snds = new Sound[namesAndPaths.Length];
            for (int i = 0; i < snds.Length; i++)
                snds[i] = LoadSound (namesAndPaths[i].A, namesAndPaths[i].B);
            return snds;
        }

        /// <summary>
        /// Loads sounds into the manager.
        /// </summary>
        /// <returns>The sounds.</returns>
        /// <param name="namesAndPaths">Names and paths.</param>
        public Sound[] LoadSounds (params Pair<string, FileInfo>[] namesAndPaths)
        {
            Sound[] snds = new Sound[namesAndPaths.Length];
            for (int i = 0; i < snds.Length; i++)
                snds[i] = LoadSound (namesAndPaths[i].A, namesAndPaths[i].B);
            return snds;
        }

        /// <summary>
        /// Creates a source.
        /// </summary>
        /// <returns>The source.</returns>
        /// <param name="name">Name.</param>
        /// <param name="sounds">Sounds.</param>
        public Source CreateSource (string name, params Pair<string, FileInfo>[] sounds)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating new sound source '{0}'", name);
            Sound[] snds = LoadSounds (sounds);
            Source src = new Source (name, Groups, snds);

            Source f = GetSource (name);

            // source with name already exists
            if (f != null)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                    "The source with the name '{0}' is already registered!", name);
                return null;
            }

            Sources.Add (src);

            return src;
        }

        /// <summary>
        /// Creates a source.
        /// </summary>
        /// <returns>The source.</returns>
        /// <param name="name">Name.</param>
        /// <param name="sounds">Sounds.</param>
        public Source CreateSource (string name, params Pair<string, string>[] sounds)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating new sound source '{0}'", name);
            Sound[] snds = LoadSounds (sounds);
            Source src = new Source (name, Groups, snds);

            Source f = GetSource (name);

            // source with name already exists
            if (f != null)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                    "The source with the name '{0}' is already registered!", name);
                return null;
            }

            Sources.Add (src);

            return src;
        }

        /// <summary>
        /// Creates a source.
        /// </summary>
        /// <returns>The source.</returns>
        /// <param name="name">Name.</param>
        /// <param name="soundNames">Sound names.</param>
        public Source CreateSource (string name, params string[] soundNames)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating new sound source '{0}'", name);
            Sound[] snds = new Sound[soundNames.Length];
            for (int i = 0; i < snds.Length; i++)
                snds[i] = Sounds.Find (s => s.Name == soundNames[i]);
            Source src = new Source (name, Groups, snds);

            Source f = GetSource (name);

            // source with name already exists
            if (f != null)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                    "The source with the name '{0}' is already registered!", name);
                return null;
            }

            Sources.Add (src);

            return src;
        }

        /// <summary>
        /// Gets a source.
        /// </summary>
        /// <returns>The source.</returns>
        /// <param name="name">Name.</param>
        public Source GetSource (string name)
        {
            return Sources.Find (s => name == s.Name);
        }

        /// <summary>
        /// Removes a source.
        /// </summary>
        /// <returns><c>true</c>, if source was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool RemoveSource (string name)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Removing sound source '{0}'", name);
            return Sources.RemoveAll (s => name == s.Name) > 0;
        }

        /// <summary>
        /// Plays a source.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        public void PlaySource (string sourceName)
        {
            GetSource (sourceName).Play ();
        }

        /// <summary>
        /// Plays sources simultaneously.
        /// </summary>
        /// <param name="sourceNames">Source names.</param>
        public void PlaySources (string[] sourceNames)
        {
            uint[] srcs = new uint[sourceNames.Length];
            for (int i = 0; i < srcs.Length; i++)
                srcs[i] = GetSource (sourceNames[i]).GetId ();
            AL.SourcePlay (srcs.Length, srcs);
        }

        /// <summary>
        /// Plays a source with time offset.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="timeOffset">Time offset.</param>
        public void PlaySource (string sourceName, TimeSpan timeOffset)
        {
            GetSource (sourceName).PlayAt (timeOffset);
        }

        /// <summary>
        /// Plays a source beginning with a specific sound.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="soundName">Sound name.</param>
        public void PlaySource (string sourceName, string soundName)
        {
            GetSource (sourceName).PlayAt (soundName);
        }

        /// <summary>
        /// Plays a source beginning with a specific sound.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="soundIndex">Sound index.</param>
        public void PlaySource (string sourceName, int soundIndex)
        {
            GetSource (sourceName).PlayAt (soundIndex);
        }

        /// <summary>
        /// Stops the source.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        public void StopSource (string sourceName)
        {
            GetSource (sourceName).Stop ();
        }

        /// <summary>
        /// Stops sources simultaneously.
        /// </summary>
        /// <param name="sourceNames">Source names.</param>
        public void StopSources (string[] sourceNames)
        {
            uint[] srcs = new uint[sourceNames.Length];
            for (int i = 0; i < srcs.Length; i++)
                srcs[i] = GetSource (sourceNames[i]).GetId ();
            AL.SourceStop (srcs.Length, srcs);
        }

        /// <summary>
        /// Pauses a source.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        public void PauseSource (string sourceName)
        {
            GetSource (sourceName).Pause ();
        }

        /// <summary>
        /// Pauses sources simultaneously.
        /// </summary>
        /// <param name="sourceNames">Source names.</param>
        public void PauseSources (string[] sourceNames)
        {
            uint[] srcs = new uint[sourceNames.Length];
            for (int i = 0; i < srcs.Length; i++)
                srcs[i] = GetSource (sourceNames[i]).GetId ();
            AL.SourcePause (srcs.Length, srcs);
        }

        /// <summary>
        /// Rewinds a source.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        public void RewindSource (string sourceName)
        {
            GetSource (sourceName).Rewind ();
        }

        /// <summary>
        /// Rewinds sources simultaneously.
        /// </summary>
        /// <param name="sourceNames">Source names.</param>
        public void RewindSources (string[] sourceNames)
        {
            uint[] srcs = new uint[sourceNames.Length];
            for (int i = 0; i < srcs.Length; i++)
                srcs[i] = GetSource (sourceNames[i]).GetId ();
            AL.SourceRewind (srcs.Length, srcs);
        }

        /// <summary>
        /// Gets or sets the listener.
        /// </summary>
        /// <value>The listener.</value>
        public Listener Listener { get; set; }

        /// <summary>
        /// Gets or sets the distance model.
        /// </summary>
        /// <value>The distance model.</value>
        public DistanceModel DistanceModel
        {
            get
            {
                return (DistanceModel) AL.GetInteger (ALGetInteger.DistanceModel);
            }
            set
            {
                AL.DistanceModel ((int) value);
            }
        }

        /// <summary>
        /// Gets or sets the doppler factor.
        /// </summary>
        /// <value>The doppler factor.</value>
        public float DopplerFactor
        {
            get
            {
                return AL.GetFloat (ALGetFloat.DopplerFactor);
            }
            set
            {
                AL.DopplerFactor (value);
            }
        }

        /// <summary>
        /// Gets or sets the doppler velocity.
        /// </summary>
        /// <value>The doppler velocity.</value>
        public float DopplerVelocity
        {
            get
            {
                return AL.GetFloat (ALGetFloat.DopplerVelocity);
            }
            set
            {
                AL.DopplerVelocity (value);
            }
        }

        /// <summary>
        /// Gets or sets the speed of sound.
        /// </summary>
        /// <value>The speed of sound.</value>
        public float SpeedOfSound
        {
            get
            {
                return AL.GetFloat (ALGetFloat.SpeedOfSound);
            }
            set
            {
                AL.SpeedOfSound (value);
            }
        }
        /// <summary>
        /// Query if the given effect type is supported by the current context
        /// </summary>
        /// <returns><c>true</c>, if effect type is supported, <c>false</c> otherwise.</returns>
        /// <param name="type">Effect type.</param>
        public bool EffectSupported(ALEffectType type)
        {
            return SupportedEffects.Contains(type);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="FreezingArcher.Audio.AudioManager"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="FreezingArcher.Audio.AudioManager"/>.
        /// The <see cref="Dispose"/> method leaves the <see cref="FreezingArcher.Audio.AudioManager"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="FreezingArcher.Audio.AudioManager"/> so the garbage collector can reclaim the memory that the
        /// <see cref="FreezingArcher.Audio.AudioManager"/> was occupying.</remarks>
        public void Dispose()
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Destroying audio manager...");
            Sources.ForEach (s => s.Dispose ());
            Sounds.ForEach (s => s.Dispose ());
            Routing.Destroy();
            Groups = null;
            Listener = null;
            SupportedEffects = null;
        }
    }
}
