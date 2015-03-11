//
//  AudioRouting.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2015 Martin Koppehel
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
using Pencil.Gaming.Audio;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Core;
using System.Linq;
using FreezingArcher.Output;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// Helper class for modeling the connections between sources and effect slots
    /// </summary>
    public sealed class AudioRouting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.AudioRouting"/> class.
        /// </summary>
        public AudioRouting ()
        {
            effectSlots = new List<EffectSlot> ();
            currentRouting = new List<RoutingEntry> ();

            var effectSlot = new EffectSlot();
            while (effectSlot.Load())
            {
                effectSlots.Add(effectSlot);
                effectSlot = new EffectSlot();
            }
            effectSlot = null;
            Logger.Log.AddLogEntry(LogLevel.Info, AudioManager.ClassName, "{0} auxiliary effect slots generated.", effectSlots.Count);
        }
        private readonly List<RoutingEntry> currentRouting;
        private List<EffectSlot> effectSlots;

        /// <summary>
        /// Gets the first free effect slot.
        /// </summary>
        /// <returns>The free slot.</returns>
        public EffectSlot GetFreeSlot()
        {
            for(int i = 0; i< effectSlots.Count; i++)
            {
                if (effectSlots[i].LoadedEffect == null)
                    return effectSlots[i];
            }
            return null;
        }

        /// <summary>
        /// Gets all effect slots.
        /// </summary>
        /// <returns>The slots.</returns>
        public IEnumerable<EffectSlot> GetSlots()
        {
            return effectSlots.AsReadOnly ();
        }

        /// <summary>
        /// Adds an audio routing entry from the given source to the given target, where a filter can be applied optionally
        /// </summary>
        /// <returns>The audio routing entry</returns>
        /// <param name="source">Where does the signal come from?</param>
        /// <param name="target">Where should the signal go to?</param>
        /// <param name="gain">How loud should the final effect be?</param>
        /// <param name="filter">Should the signal be filtered?</param>
        public RoutingEntry AddAudioRouting(Source source, EffectSlot target, float gain, Filter filter = null)
        {
            var routing = new RoutingEntry (source, target, gain.Clamp (0f, 1f), filter);
            currentRouting.Add (routing);
            routing.Setup ();
            return routing;
        }

        /// <summary>
        /// Remove the specified entry.
        /// </summary>
        /// <param name="entry">Entry to remove</param>
        public void Remove(RoutingEntry entry)
        {
            currentRouting.Remove (entry);
            entry.Clear ();
        }

        /// <summary>
        /// Destroy this instance including all objects created by it
        /// Will destroy the EffectSlots
        /// </summary>
        public void Destroy()
        {
            foreach (var item in effectSlots)
            {
                item.Destroy();
            }
        }
    }
}

