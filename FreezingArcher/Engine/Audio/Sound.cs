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
using FreezingArcher.Core;

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
    public class Sound : IManageable, IDisposable
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Sound";

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
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Creating new sound '{0}' from '{1}'", name,
                file.FullName);
            Name = name;
            File = file;
            Load ();
        }

        /// <summary>
        /// Load this resource.
        /// </summary>
        void Load ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading sound '{0}'...", Name);
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
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName, Status.BadData, "Invalid file format '{0}'!", File.Extension);
                throw new FileLoadException ("Invalid file format");
            }
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

        /// <summary>
        /// Releases all resource used by the <see cref="FreezingArcher.Audio.Sound"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="FreezingArcher.Audio.Sound"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="FreezingArcher.Audio.Sound"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="FreezingArcher.Audio.Sound"/> so the garbage collector can reclaim the memory that the
        /// <see cref="FreezingArcher.Audio.Sound"/> was occupying.</remarks>
        public void Dispose ()
        {
            AL.DeleteBuffers (1, ref AlBufferId);
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
