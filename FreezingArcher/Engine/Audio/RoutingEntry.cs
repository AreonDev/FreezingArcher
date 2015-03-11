//
//  RoutingEntry.cs
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
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Core;
using Pencil.Gaming.Audio;

namespace FreezingArcher.Audio
{
    public class RoutingEntry : IManageable
    {
        internal RoutingEntry (Source source, EffectSlot target, float gain, Filter filter = null)
        {
            this.Source = source;
            this.Target = target;

            this.Filter = filter;
            this.gain = gain.Clamp (0f, 1f);


        }

        private float gain;
        private Filter filter;

        public Source Source
        {
            get;
            private set;
        }

        public EffectSlot Target
        {
            get;
            private set;
        }

        public float Gain
        {
            get
            {
                return this.gain;
            } 
            set
            {
                this.gain = value.Clamp (0f, 1f); 
                if(setup)
                AL.AuxiliaryEffectSlot(Target.ALID, ALAuxiliaryf.EffectslotGain, this.gain);
            }
        }

        public Filter Filter
        {
            get
            {
                return filter;
            }
            set
            { 
                if (filter != null)
                {
                    filter.Update -= HandleFilterUpdate;
                }
                filter = value;
                if (filter != null)
                {
                    filter.Update += HandleFilterUpdate;
                    if(setup)
                    AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, 0, (int)filter.ALID);
                }
                else
                {
                    if(setup)
                    AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, 0, 0); //disable filter
                }
            }
        }

        void HandleFilterUpdate (object sender, EventArgs e)
        {
            if(setup)
            AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, 0, (int)filter.ALID);
        }

        private bool setup = false;
        internal void Setup ()
        {
            //TODO: Update Aux Sends
            AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, 0, (int)Filter.ALID);
            AL.AuxiliaryEffectSlot(Target.ALID, ALAuxiliaryf.EffectslotGain, Gain);
            setup = true;
        }

        internal void Clear ()
        {
            //TODO: Cleanup
            AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, 0, 0, 0);
            AL.AuxiliaryEffectSlot(Target.ALID, ALAuxiliaryf.EffectslotGain, Gain);
            setup = false;
        }

        #region IManageable implementation

        public string Name
        {
            get
            {
                return Source.Name + "/" + Target.Name;
            }
            set
            {
                throw new NotSupportedException ();
            }
        }

        #endregion
    }
}

