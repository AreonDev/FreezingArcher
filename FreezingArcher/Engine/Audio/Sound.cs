//
//  AudioFile.cs
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
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Output;
using Pencil.Gaming.Audio;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// Enum describing an audio codec.
    /// </summary>
    public enum AudioCodec : byte
    {
        /// <summary>
        /// Unknown audio codec.
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// RIFF WAVE-PCM audio codec.
        /// </summary>
        WAV,
        /// <summary>
        /// OGG Vorbis audio codec.
        /// </summary>
        OGG,
        /// <summary>
        /// AAC audio codec.
        /// </summary>
        AAC,
        /// <summary>
        /// MPEG Audio Layer 3 audio codec.
        /// </summary>
        MP3,
        /// <summary>
        /// Free lossless audio codec.
        /// </summary>
        FLAC
    }

    /// <summary>
    /// Sound class (openal equivalent: Buffer).
    /// </summary>
    public class Sound : IResource, IManageable
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Sound_";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Sound"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="fileName">File name.</param>
        internal Sound (string name, string fileName) : this (name, new FileInfo (fileName))
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Sound"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="file">File.</param>
        internal Sound (string name, FileInfo file)
        {
            Name = name;
            File = file;
            Loaded = false;
        }

        /// <summary>
        /// Load this resource.
        /// </summary>
        internal void Load ()
        {
            Loaded = false;
            if (File.Extension == ".wav")
            {
                AlBufferId = AL.Utils.BufferFromWav (File.FullName);
                AudioCodec = AudioCodec.WAV;
            }
            else if (File.Extension == ".ogg")
            {
                AlBufferId = AL.Utils.BufferFromOgg (File.FullName);
                AudioCodec = AudioCodec.OGG;
            }
            else
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name, "Invalid file format '{0}'!", File.Extension);
                throw new FileLoadException ("Invalid file format");
            }
            Loaded = true;
        }

        /// <summary>
        /// The openal buffer identifier (name).
        /// </summary>
        protected uint AlBufferId;

        /// <summary>
        /// The audio file.
        /// </summary>
        protected FileInfo File;

        /// <summary>
        /// The audio codec.
        /// </summary>
        protected AudioCodec AudioCodec;

        /// <summary>
        /// Gets the bits per channel.
        /// </summary>
        /// <returns>The bits per channel.</returns>
        public int GetBitsPerChannel ()
        {
            int bits;
            AL.GetBuffer (AlBufferId, ALGetBufferi.Bits, out bits);
            return bits;
        }

        /// <summary>
        /// Gets the channel count.
        /// </summary>
        /// <returns>The channel count.</returns>
        public int GetChannelCount ()
        {
            int channels;
            AL.GetBuffer (AlBufferId, ALGetBufferi.Channels, out channels);
            return channels;
        }

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        /// <returns>The frequency.</returns>
        public int GetFrequency ()
        {
            int freq;
            AL.GetBuffer (AlBufferId, ALGetBufferi.Frequency, out freq);
            return freq;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <returns>The size.</returns>
        public int GetSize ()
        {
            int bytes;
            AL.GetBuffer (AlBufferId, ALGetBufferi.Size, out bytes);
            return bytes;
        }

        /// <summary>
        /// Gets the openal identifier (name).
        /// </summary>
        /// <returns>The identifier.</returns>
        internal uint GetId ()
        {
            if (!Loaded)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName + Name,
                    "Trying to read openal id (name) property before resource was loaded!");
                throw new InvalidOperationException ();
            }

            return AlBufferId;
        }

        /// <summary>
        /// Gets the audio format.
        /// </summary>
        /// <returns>The audio format.</returns>
        public AudioCodec GetAudioCodec ()
        {
            return AudioCodec;
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
            list.Add (Load);
            NeedsLoad = reloader;
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
            AL.DeleteBuffers (1, ref AlBufferId);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Audio.Sound"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; set; }

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
