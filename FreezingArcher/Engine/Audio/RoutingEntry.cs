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

namespace FreezingArcher.Audio
{
    public class RoutingEntry : IManageable
    {
        internal RoutingEntry (Source source, EffectSlot target, float gain, Filter filter = null)
        {
            this.Source = source;
            this.Target = target;



            this.gain = gain.Clamp (0f, 1f);
            this.Filter = filter;


            //TODO: Setup OpenAL aux sends and if necessary filters 
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
                //TODO: Update gain in openal
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
                    //TODO: Deattach filter
                }
                filter = value;
                if (filter != null)
                {
                    filter.Update += HandleFilterUpdate;
                    //TODO: Attach filter
                }
            }
        }

        void HandleFilterUpdate (object sender, EventArgs e)
        {
            //TODO: OpenAL reattach filter
        }

        internal void Setup ()
        {
            //TODO: Initialization
        }

        internal void Clear ()
        {
            //TODO: Cleanup
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

