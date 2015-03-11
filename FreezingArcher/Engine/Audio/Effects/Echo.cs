//
//  Echo.cs
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
    /// Echo effect. For details headover to the effects extension guide.
    /// </summary>
    public class Echo : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Echo"/> class.
        /// </summary>
        public Echo()
        {
        }


        #region implemented abstract members of Effect
        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Echo);
            Spread = _Spread;
            Delay = _Delay;
            LRDelay = _LRDelay;
            Damping = _Damping;
            Feedback = _Feedback;
            return true;
        }
        #endregion

        private float _Delay = 0.1f;
        private float _LRDelay = 0.1f;
        private float _Damping = 0.5f;
        private float _Feedback = 0.5f;
        private float _Spread = -1f;

        /// <summary>
        /// Gets or sets the spread value.
        /// </summary>
        /// <value>The spread value.</value>
        public float Spread
        {
            get
            {
                return _Spread;
            }
            set
            {
                _Spread = value.Clamp(-1f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EchoSpread, _Spread);
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
                _Feedback = value.Clamp(0f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EchoFeedback, _Feedback);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the damping value.
        /// </summary>
        /// <value>The damping value.</value>
        public float Damping
        {
            get
            {
                return _Damping;
            }
            set
            {
                _Damping = value.Clamp(0f, 0.99f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EchoDamping, _Damping);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the LR delay.
        /// </summary>
        /// <value>The LR delay.</value>
        public float LRDelay
        {
            get
            {
                return _LRDelay;
            }
            set
            {
                _LRDelay = value.Clamp(0f, 0.404f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EchoLRDelay, _LRDelay);
                TriggerUpdate();
            }
        }
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
                _Delay = value.Clamp(0f, 0.207f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.EchoDelay, _Delay);
                TriggerUpdate();
            }
        }
    }
}

