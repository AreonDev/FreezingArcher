//
//  EffectSlot.cs
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
using Pencil.Gaming.Audio;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// ALEffectSlot equivalent class
    /// </summary>
    public class EffectSlot
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.EffectSlot"/> class.
        /// </summary>
        public EffectSlot()
        {
            ALID = uint.MaxValue;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FreezingArcher.Audio.EffectSlot"/> is reclaimed by garbage collection.
        /// </summary>
        ~EffectSlot()
        {
            if (ALID == uint.MaxValue)
                return;
            AL.DeleteAuxiliaryEffectSlots(new uint[]{ ALID });
        }

        internal bool Load()
        {
            uint[] alId = new uint[1];
            AL.GenAuxiliaryEffectSlots(alId);
            if (AL.GetError() != (int)ALError.NoError)
                return false;
            this.ALID = alId[0];
            AL.AuxiliaryEffectSlot(ALID, ALAuxiliaryi.EffectslotAuxiliarySendAuto, 0);
            return true;
        }

        internal uint ALID { get; set; }

        internal string Name
        { 
            get
            { 
                return "EffectSlot/" + ALID;
            }
        }

        private Effect efx;

        /// <summary>
        /// Gets or sets the loaded effect.
        /// </summary>
        /// <value>The loaded effect.</value>
        public Effect LoadedEffect
        {
            get
            {
                return efx;
            }
            set
            {
                if (efx != null)
                {
                    efx.Update -= HandleEFXUpdate;
                }
                efx = value;
                if (efx != null)
                {
                    efx.Update += HandleEFXUpdate;
                    AL.AuxiliaryEffectSlot(ALID, ALAuxiliaryi.EffectslotEffect, (int)efx.ALID);
                }
                else
                {
                    AL.AuxiliaryEffectSlot(ALID, ALAuxiliaryi.EffectslotEffect, 0);
                }
            }
        }

        void HandleEFXUpdate(object sender, EventArgs e)
        {
            //reattach effect to ensure parameter updates are used instantly
            AL.AuxiliaryEffectSlot(ALID, ALAuxiliaryi.EffectslotEffect, (int)((Effect)sender).ALID);
        }
    }
}

