//
//  Equalizer.cs
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

namespace FreezingArcher.Audio.Effects
{
    /// <summary>
    /// Equalizer effect. 
    /// Allows the user to raise or lower certain frequencies.
    /// Modeled as a 4-band parametric EQ
    /// For details headover to the effects extension guide.
    /// </summary>
    public class Equalizer : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Equalizer"/> class.
        /// </summary>
        public Equalizer()
        {
        }


        #region implemented abstract members of Effect
        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Equalizer);
            LowGain = _LowGain;
            LowCutoff = _LowCutoff;
            Mid1Gain = _Mid1Gain;
            Mid1Width = _Mid1Width;
            Mid1Center = _Mid1Center;
            Mid2Gain = _Mid2Gain;
            Mid2Center = _Mid2Center;
            Mid2Width = _Mid2Width;
            HighGain = _HighGain;
            HighCutoff = _HighCutoff;
            return true;
        }
        #endregion

        private float _LowGain = 1f;
        private float _LowCutoff = 200f;
        private float _Mid1Gain = 1f;
        private float _Mid1Center = 500f;
        private float _Mid1Width = 1f;
        private float _Mid2Gain = 1f;
        private float _Mid2Center = 3000f;
        private float _Mid2Width = 1f;
        private float _HighGain = 1f;
        private float _HighCutoff = 6000f;

        /// <summary>
        /// Gets or sets the high cutoff frequency.
        /// </summary>
        /// <value>The high cutoff.</value>
        public float HighCutoff
        {
            get
            {
                return _HighCutoff;
            }
            set
            {
                _HighCutoff = value.Clamp(4000f, 16000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerHighCutoff, _HighCutoff);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the high gain.
        /// </summary>
        /// <value>The high gain.</value>
        public float HighGain
        {
            get
            {
                return _HighGain;
            }
            set
            {
                _HighGain = value.Clamp(0.126f, 7.943f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerHighGain, _HighGain);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the width of the mid2 band.
        /// </summary>
        /// <value>The width of the mid2 band.</value>
        public float Mid2Width
        {
            get
            {
                return _Mid2Width;
            }
            set
            {
                _Mid2Width = value.Clamp(0.01f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerMid2Width, _Mid2Width);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the center frequency of the mid2 band.
        /// </summary>
        /// <value>The mid2 center frequency.</value>
        public float Mid2Center
        {
            get
            {
                return _Mid2Center;
            }
            set
            {
                _Mid2Center = value.Clamp(1000f, 8000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerMid2Center, _Mid2Center);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the gain of the mid2 band.
        /// </summary>
        /// <value>The mid2 gain.</value>
        public float Mid2Gain
        {
            get
            {
                return _Mid2Gain;
            }
            set
            {
                _Mid2Gain = value.Clamp(0.126f, 7.943f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerMid2Gain, _Mid2Gain);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the width of the mid1 band.
        /// </summary>
        /// <value>The width of the mid1.</value>
        public float Mid1Width
        {
            get
            {
                return _Mid1Width;
            }
            set
            {
                _Mid1Width = value.Clamp(0.01f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerMid1Width, _Mid1Width);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the center frequency of the mid1 band.
        /// </summary>
        /// <value>The mid1 center frequency.</value>
        public float Mid1Center
        {
            get
            {
                return _Mid1Center;
            }
            set
            {
                _Mid1Center = value.Clamp(200f, 3000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerMid1Center, _Mid1Center);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the gain of the mid1 band.
        /// </summary>
        /// <value>The mid1 gain.</value>
        public float Mid1Gain
        {
            get
            {
                return _Mid1Gain;
            }
            set
            {
                _Mid1Gain = value.Clamp(0.126f, 7.943f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerMid1Gain, _Mid1Gain);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the low cutoff frequency.
        /// </summary>
        /// <value>The low cutoff frequency.</value>
        public float LowCutoff
        {
            get
            {
                return _LowCutoff;
            }
            set
            {
                _LowCutoff = value.Clamp(50f, 800f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerLowCutoff, _LowCutoff);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the low gain.
        /// </summary>
        /// <value>The low gain.</value>
        public float LowGain
        {
            get
            {
                return _LowGain;
            }
            set
            {
                _LowGain = value.Clamp(0.126f, 7.943f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EqualizerLowGain, _LowGain);
                TriggerUpdate();
            }
        }
    }
}

