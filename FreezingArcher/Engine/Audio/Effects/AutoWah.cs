//
//  AutoWah.cs
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
    /// AutoWah effect. For details headover to the effects extension guide
    /// </summary>
    public class AutoWah : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.AutoWah"/> class.
        /// </summary>
        public AutoWah()
        {
        }

        #region implemented abstract members of Effect

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Autowah);
            AttackTime = _AttackTime;
            ReleaseTime = _ReleaseTime;
            PeakGain = _PeakGain;
            Resonance = _Resonance;
            return true;
        }
            
        #endregion

        private float _AttackTime = 0.06f;
        private float _ReleaseTime = 0.06f;
        private float _Resonance = 1000f;
        private float _PeakGain = 11.22f;

        /// <summary>
        /// Gets or sets the peak gain.
        /// </summary>
        /// <value>The peak gain.</value>
        public float PeakGain
        {
            get
            {
                return _PeakGain;
            }
            set
            {
                _PeakGain = value.Clamp(0.00003f, 31621f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.AutowahPeakGain, _PeakGain);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the resonance.
        /// </summary>
        /// <value>The resonance.</value>
        public float Resonance
        {
            get
            {
                return _Resonance;
            }
            set
            {
                _Resonance = value.Clamp(2f, 1000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.AutowahResonance, _Resonance);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the release time.
        /// </summary>
        /// <value>The release time.</value>
        public float ReleaseTime
        {
            get
            {
                return _ReleaseTime;
            }
            set
            {
                _ReleaseTime = value.Clamp(0.0001f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.AutowahReleaseTime, _ReleaseTime);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the attack time.
        /// </summary>
        /// <value>The attack time.</value>
        public float AttackTime
        {
            get
            {
                return _AttackTime;
            }
            set
            {
                _AttackTime = value.Clamp(0.0001f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.AutowahAttackTime, _AttackTime);
                TriggerUpdate();
            }
        }
    }
}

