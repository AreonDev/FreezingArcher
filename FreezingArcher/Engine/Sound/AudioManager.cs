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

namespace FreezingArcher.Sound
{
    public enum DistanceModel
    {
        None,
        InverseDistance = 53249,
        InverseDistanceClamped,
        LinearDistance,
        LinearDistanceClamped,
        ExponentDistance,
        ExponentDistanceClamped
    }

    public class AudioManager
    {
        public AudioFile LoadAudioFile (string name, string file)
        {
            return null;
        }

        public AudioFile LoadAudioFile (string name, FileInfo file)
        {
            return null;
        }

        public AudioFile[] LoadAudioFiles (Pair<string, string>[] namesAndPaths)
        {
            return null;
        }

        public AudioFile[] LoadAudioFiles (Pair<string, FileInfo>[] namesAndPaths)
        {
            return null;
        }

        public Sound CreateSound (string name, Pair<string, FileInfo>[] audioFiles)
        {
            return null;
        }

        public Sound CreateSound (string name, Pair<string, string>[] audioFiles)
        {
            return null;
        }

        public Sound CreateSound (string name, string[] audioFileNames)
        {
            return null;
        }

        public Sound GetSound (string name)
        {
            return null;
        }

        public bool RemoveSound (string name)
        {
            return false;
        }

        public void PlaySound (string name)
        {}

        public void StopSound (string name)
        {}

        public void PauseSound (string name)
        {}

        public void RewindSound (string name)
        {}

        public Listener Listener { get; set; }

        public DistanceModel DistanceModel { get; set; }

        public float DopplerFactor { get; set; }
        public float DopplerVelocity { get; set; }
        public float SpeedOfSound { get; set; }
    }
}
