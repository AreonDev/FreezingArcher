//
//  BandpassFilter.cs
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
using FreezingArcher.Core;
using Pencil.Gaming.Audio;

namespace FreezingArcher.Audio.Filters
{
    /// <summary>
    /// Bandpass filter. For details headover to the effects extension guide.
    /// </summary>
    public class BandpassFilter : Filter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Filters.BandpassFilter"/> class.
        /// </summary>
        public BandpassFilter()
        {
        }

        #region implemented abstract members of Filter
        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.FilterType(ALID, ALFilterType.Bandpass);
            Gain = _Gain;
            GainHF = _GainHF;
            GainLF = _GainLF;
            return true;
        }

        #endregion

        private float _Gain = 1f;
        private float _GainLF = 1f;
        private float _GainHF = 1f;

        /// <summary>
        /// Gets or sets the gain for high frequencies.
        /// </summary>
        /// <value>The gain for HF.</value>
        public float GainHF
        {
            get
            {
                return _GainHF;
            }
            set
            {
                _GainHF = value.Clamp(0f, 1f);
                if (Loaded)
                    AL.Filter(ALID, ALFilterf.BandpassGainHF, _GainHF);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the gain for low frequencies.
        /// </summary>
        /// <value>The gain for LF.</value>
        public float GainLF
        {
            get
            {
                return _GainLF;
            }
            set
            {
                _GainLF = value.Clamp(0f, 1f);
                if (Loaded)
                    AL.Filter(ALID, ALFilterf.BandpassGainLF, _GainLF);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the gain.
        /// </summary>
        /// <value>The gain.</value>
        public float Gain
        {
            get
            {
                return _Gain;
            }
            set
            {
                _Gain = value.Clamp(0f, 1f);
                if (Loaded)
                    AL.Filter(ALID, ALFilterf.BandpassGain, _Gain);
                TriggerUpdate();
            }
        }
    }
}

