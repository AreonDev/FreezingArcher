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
    /// <summary>
    /// Helper class for audio routings from ALSource to EffectSlot
    /// </summary>
    public sealed class RoutingEntry : IManageable
    {
        internal RoutingEntry (Source source, EffectSlot target, float gain, Filter filter = null)
        {
            this.Source = source;
            this.Target = target;

            this.Filter = filter;
            this.gain = gain.Clamp (0f, 1f);
            sourceRouteIndex = -1;

        }
        private int sourceRouteIndex;
        private float gain;
        private Filter filter;

        /// <summary>
        /// Gets the source of the signal
        /// </summary>
        /// <value>The source.</value>
        public Source Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the target of the signal
        /// </summary>
        /// <value>The target.</value>
        public EffectSlot Target
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the gain of the effect (Effect will be reapplied instantly)
        /// </summary>
        /// <value>The gain.</value>
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

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
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
                }
                    if(setup)
                    AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, 1, filter==null?0:(int)filter.ALID); //disable filter
            }
        }

        void HandleFilterUpdate (object sender, EventArgs e)
        {
            if(setup)
                AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, sourceRouteIndex, Filter == null ? 0 : (int)Filter.ALID);
        }

        private bool setup = false;
        internal void Setup ()
        {
            //TODO: Update Aux Sends
            sourceRouteIndex = Source.GetRoutingId();
            if (sourceRouteIndex == -1)
                return; //too many routes known
            AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, (int)Target.ALID, sourceRouteIndex, Filter == null ? 0 : (int)Filter.ALID);
            AL.AuxiliaryEffectSlot(Target.ALID, ALAuxiliaryf.EffectslotGain, Gain);
            Source.setRoute(this, sourceRouteIndex);
            setup = true;
        }

        internal void Clear ()
        {
            AL.Source(Source.GetId(), ALSource3i.EfxAuxiliarySendFilter, 0, sourceRouteIndex, 0);
            AL.AuxiliaryEffectSlot(Target.ALID, ALAuxiliaryf.EffectslotGain, Gain);
            Source.setRoute(null, sourceRouteIndex);
            setup = false;
        }

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
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

