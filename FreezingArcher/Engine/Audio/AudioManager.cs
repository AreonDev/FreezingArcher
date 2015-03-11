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
using System.IO;
using FreezingArcher.Core;
using System;
using System.Collections.Generic;
using FreezingArcher.Output;
using Pencil.Gaming.Audio;

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
    public class AudioManager
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
            Sounds = new List<Sound> ();
            Sources = new List<Source> ();
            Groups = new Dictionary<SourceGroup, float> ();
            Groups.Add (SourceGroup.Music, 1);
            Groups.Add (SourceGroup.Environment, 1);
            Groups.Add (SourceGroup.Effect, 1);
            Groups.Add (SourceGroup.Voice, 1);
            Groups.Add (SourceGroup.VoiceChat, 1);
            Groups.Add (SourceGroup.Custom1, 1);
            Groups.Add (SourceGroup.Custom2, 1);
            Groups.Add (SourceGroup.Custom3, 1);
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
        /// Sets the group gain.
        /// </summary>
        /// <param name="sourceGroup">Source group.</param>
        /// <param name="gain">Gain.</param>
        public void SetGroupGain (SourceGroup sourceGroup, float gain)
        {
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
        public Sound[] LoadSounds (Pair<string, string>[] namesAndPaths)
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
        public Sound[] LoadSounds (Pair<string, FileInfo>[] namesAndPaths)
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
        public Source CreateSource (string name, Pair<string, FileInfo>[] sounds)
        {
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
        public Source CreateSource (string name, Pair<string, string>[] sounds)
        {
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
        public Source CreateSource (string name, string[] soundNames)
        {
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
            return Sources.RemoveAll (s => name == s.Name) > 0;
        }

        /// <summary>
        /// Plays a source.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlaySource (string sourceName, bool shuffleSounds = false)
        {
            GetSource (sourceName).Play (shuffleSounds);
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
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlaySource (string sourceName, TimeSpan timeOffset, bool shuffleSounds = false)
        {
            GetSource (sourceName).PlayAt (timeOffset, shuffleSounds);
        }

        /// <summary>
        /// Plays a source beginning with a specific sound.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="soundName">Sound name.</param>
        /// <param name="proceedWithNextSoundWhenFinished">If set to <c>true</c> proceed with next sound when finished.</param>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlaySource (string sourceName, string soundName, bool proceedWithNextSoundWhenFinished = false,
            bool shuffleSounds = false)
        {
            GetSource (sourceName).PlaySound (soundName, proceedWithNextSoundWhenFinished,
                shuffleSounds);
        }

        /// <summary>
        /// Plays a source beginning with a specific sound.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="soundIndex">Sound index.</param>
        /// <param name="proceedWithNextSoundWhenFinished">If set to <c>true</c> proceed with next sound when finished.</param>
        /// <param name="shuffleSounds">If set to <c>true</c> shuffle sounds.</param>
        public void PlaySource (string sourceName, int soundIndex, bool proceedWithNextSoundWhenFinished = false,
            bool shuffleSounds = false)
        {
            GetSource (sourceName).PlaySound (soundIndex, proceedWithNextSoundWhenFinished,
                shuffleSounds);
        }

        /// <summary>
        /// Plays a source with the sounds ordered as given in the playlist.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="soundPlaylist">Sound playlist.</param>
        public void PlaySource (string sourceName, string[] soundPlaylist)
        {
            GetSource (sourceName).PlaySounds (soundPlaylist);
        }

        /// <summary>
        /// Plays a source with the sounds ordered as given in the playlist.
        /// </summary>
        /// <param name="sourceName">Source name.</param>
        /// <param name="soundIndexPlaylist">Sound index playlist.</param>
        public void PlaySource (string sourceName, int[] soundIndexPlaylist)
        {
            GetSource (sourceName).PlaySounds (soundIndexPlaylist);

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
    }
}
