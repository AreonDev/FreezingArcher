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
    public class AudioRouting
    {
        public AudioRouting ()
        {
            effectSlots = new List<EffectSlot> ();
            currentRouting = new List<RoutingEntry> ();
            /* TODO: 
             * Query OpenAL maxEffectSlots
             * GenEffectSlots for all times
             */

            var effectSlot = new EffectSlot();
            while (effectSlot.Load())
            {
                effectSlots.Add(effectSlot);
                effectSlot = new EffectSlot();
            }
            effectSlot = null;
            Logger.Log.AddLogEntry(LogLevel.Info, "SoundSystem", "{0} Auxiliary effect slots generated.", effectSlots.Count);
        }
        private List<RoutingEntry> currentRouting;
        private List<EffectSlot> effectSlots;

        public EffectSlot GetFreeSlot()
        {
            for(int i = 0; i< effectSlots.Count; i++)
            {
                if (effectSlots[i].LoadedEffect == null)
                    return effectSlots[i];
            }
            return null;
        }

        public IEnumerable<EffectSlot> GetSlots()
        {
            return effectSlots.AsReadOnly ();
        }

        public RoutingEntry AddAudioRouting(Source source, EffectSlot target, float gain, Filter filter = null)
        {
            var routing = new RoutingEntry (source, target, gain.Clamp (0f, 1f), filter);
            currentRouting.Add (routing);
            routing.Setup ();
            return routing;
        }

        public void Remove(RoutingEntry entry)
        {
            currentRouting.Remove (entry);
            entry.Clear ();
        }
    }
}

