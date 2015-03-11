//
//  Chorus.cs
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
    /// Chorus effect. For details headover to the effects extension guide.
    /// </summary>
    public class Chorus : Effect
    {
        /// <summary>
        /// All possible (supported) waveforms which can be used for <see cref="Chorus"/>
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
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Chorus"/> class.
        /// </summary>
        public Chorus()
        {
        }

        #region implemented abstract members of Effect

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Chorus);
            WaveForm = _WaveForm;
            Phase = _Phase;
            Rate = _Rate;
            Depth = _Depth;
            Feedback = _Feedback;
            return true;
        }

        #endregion

        private Waveform _WaveForm = Waveform.Triangle;
        private int _Phase = 90;
        private float _Rate = 1.1f;
        private float _Depth = 0.1f;
        private float _Feedback = 0.25f;

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
                    AL.Effect(ALID, ALEffectf.ChorusFeedback, _Feedback);
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
                    AL.Effect(ALID, ALEffectf.ChorusDepth, _Depth);
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
                    AL.Effect(ALID, ALEffectf.ChorusRate, _Rate);
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
                    AL.Effect(ALID, ALEffecti.ChorusPhase, _Phase);
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
                    AL.Effect(ALID, ALEffecti.ChorusWaveform, (int)_WaveForm);
                TriggerUpdate();
            }
        }
    }
}

