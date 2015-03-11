//
//  Flanger.cs
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
    /// Flanger effect. For details headover to the effects extension guide.
    /// </summary>
    public class Flanger : Effect
    {
        /// <summary>
        /// Waveform enum. 
        /// Enumeration of all possible Waveforms for the <see cref="Flanger"/> effect
        /// </summary>
        public enum Waveform 
        {
            /// <summary>
            /// Sinus waveform.
            /// </summary>
            Sinus = 0,
            /// <summary>
            /// Triangle waveform.
            /// </summary>
            Triangle = 1,
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Flanger"/> class.
        /// </summary>
        public Flanger()
        {
        }


        #region implemented abstract members of Effect
        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Flanger);
            return true;
        }
        #endregion

        private Waveform _WaveForm = Waveform.Triangle;
        private int _Phase = 0;
        private float _Rate = 0.27f;
        private float _Depth = 1f;
        private float _Feedback = -0.5f;
        private float _Delay = 0.002f;

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public float Delay
        {
            get
            {
                return _Delay;
            }
            set
            {
                _Delay = value.Clamp(0f, 0.004f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.FlangerDelay, _Delay);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the feedback value.
        /// </summary>
        /// <value>The feedback value.</value>
        public float Feedback
        {
            get
            {
                return _Feedback;
            }
            set
            {
                _Feedback = value.Clamp(-1f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.FlangerFeedback, _Feedback);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the depth value.
        /// </summary>
        /// <value>The depth value.</value>
        public float Depth
        {
            get
            {
                return _Depth;
            }
            set
            {
                _Depth = value.Clamp(0f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.FlangerDepth, _Depth);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        /// <value>The rate.</value>
        public float Rate
        {
            get
            {
                return _Rate;
            }
            set
            {
                _Rate = value.Clamp(0f, 10f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.FlangerRate, _Rate);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the phase.
        /// </summary>
        /// <value>The phase.</value>
        public int Phase
        {
            get
            {
                return _Phase;
            }
            set
            {
                _Phase = value.Clamp(-180, 180);
                if (Loaded)
                    AL.Effect(ALID, ALEffecti.FlangerPhase, _Phase);
                TriggerUpdate();
            }
        }
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
                    AL.Effect(ALID, ALEffecti.FlangerWaveform, (int)_WaveForm);
                TriggerUpdate();
            }
        }
    }
}

