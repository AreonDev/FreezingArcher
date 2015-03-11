//
//  Compressor.cs
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
using Pencil.Gaming.Audio;
using FreezingArcher.Core;

namespace FreezingArcher.Audio.Effects
{
    /// <summary>
    /// Compressor effect. For details headover to the effects extension guide.
    /// </summary>
    public class Compressor : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Compressor"/> class.
        /// </summary>
        public Compressor()
        {
        }

        #region implemented abstract members of Effect

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Compressor);
            Enabled = _Enabled;
            return true;
        }

        #endregion

        private int _Enabled = 1;

        /// <summary>
        /// Gets or sets if the compressor should be enabled.
        /// </summary>
        /// <value>The enabled.</value>
        public int Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value.Clamp(0, 1);
                
                AL.Effect(ALID, ALEffecti.CompressorOnoff, _Enabled);
                TriggerUpdate();
            }
        }
    }
}

