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
using FreezingArcher.Core.Interfaces;
using System.Collections.Generic;
using System;

namespace FreezingArcher.Sound
{
    public enum AudioFormat
    {
        WAV,
        OGG,
        AAC,
        MP3,
        FLAC
    }

    public enum AudioFileState
    {
        Unused = 8208,
        Pending,
        Processed
    }

    public class AudioFile : IResource, IManageable
    {
        public int GetBitsPerChannel ()
        {
            return 0;
        }

        public int GetChannelCount ()
        {
            return 0;
        }

        public int GetFrequency ()
        {
            return 0;
        }

        public int GetSize ()
        {
            return 0;
        }

        internal uint GetId ()
        {
            return 0;
        }

        public AudioFormat GetAudioFormat ()
        {
            return AudioFormat.WAV;
        }

        public AudioFileState GetState ()
        {
            return AudioFileState.Unused;
        }

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

        public bool Loaded { get; set; }

        #endregion

        #region IManageable implementation
        public string Name { get; set; }
        #endregion
    }
}
