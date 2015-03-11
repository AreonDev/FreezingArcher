//
//  RingModulator.cs
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
    /// Ring modulator effect. For details headover to the effects extension guide.
    /// </summary>
    public class RingModulator : Effect
    {
        /// <summary>
        /// Enumeration of all possible waveforms for the <see cref="RingModulator"/> effect.
        /// </summary>
        public enum Waveform 
        {
            /// <summary>
            /// Sinus waveform.
            /// </summary>
            Sinus = 0,
            /// <summary>
            /// Saw waveform. (Triangle)
            /// </summary>
            Saw = 1,
            /// <summary>
            /// Square waveform.
            /// </summary>
            Square = 2,
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.RingModulator"/> class.
        /// </summary>
        public RingModulator()
        {
        }

        #region implemented abstract members of Effect

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.RingModulator);
            Frequency = _Frequency;
            HighpassCutoff = _HighpassCutoff;
            WaveForm = _WaveForm;
            return true;
        }

        #endregion

        private float _Frequency = 440f;
        private float _HighpassCutoff = 800f;
        private Waveform _WaveForm = Waveform.Sinus;

        /// <summary>
        /// Gets or sets the wave form.
        /// </summary>
        /// <value>The wave form.</value>
        public Waveform WaveForm
        {
            get
            {
                return _WaveForm;
            }
            set
            {
                _WaveForm = value;
                if (Loaded)
                    AL.Effect(ALID, ALEffecti.RingModulatorWaveform, (int)_WaveForm);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the highpass cutoff.
        /// </summary>
        /// <value>The highpass cutoff.</value>
        public float HighpassCutoff
        {
            get
            {
                return _HighpassCutoff;
            }
            set
            {
                _HighpassCutoff = value.Clamp(0f, 24000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.RingModulatorHighpassCutoff, _HighpassCutoff);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public float Frequency
        {
            get
            {
                return _Frequency;
            }
            set
            {
                _Frequency = value.Clamp(0f, 8000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.RingModulatorFrequency, _Frequency);
                TriggerUpdate();
            }
        }
    }
}

