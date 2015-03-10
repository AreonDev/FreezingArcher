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

namespace FreezingArcher.Audio
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
        public Sound LoadSound (string name, string file)
        {
            return null;
        }

        public Sound LoadSound (string name, FileInfo file)
        {
            return null;
        }

        public Sound[] LoadSounds (Pair<string, string>[] namesAndPaths)
        {
            return null;
        }

        public Sound[] LoadSounds (Pair<string, FileInfo>[] namesAndPaths)
        {
            return null;
        }

        public Source CreateSource (string name, Pair<string, FileInfo>[] sounds)
        {
            return null;
        }

        public Source CreateSource (string name, Pair<string, string>[] sounds)
        {
            return null;
        }

        public Source CreateSource (string name, string[] soundNames)
        {
            return null;
        }

        public Source GetSource (string name)
        {
            return null;
        }

        public bool RemoveSource (string name)
        {
            return false;
        }

        public void PlaySource (string name)
        {}

        public void StopSource (string name)
        {}

        public void PauseSource (string name)
        {}

        public void RewindSource (string name)
        {}

        public Listener Listener { get; set; }

        public DistanceModel DistanceModel { get; set; }

        public float DopplerFactor { get; set; }
        public float DopplerVelocity { get; set; }
        public float SpeedOfSound { get; set; }
    }
}
