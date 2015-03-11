//
//  FrequencyShifter.cs
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
    /// Frequency shifter. For details headover to the effects extension guide.
    /// </summary>
    public class FrequencyShifter : Effect
    {
        /// <summary>
        /// Enumeration of all possible directions for the <see cref="FrequencyShifter"/> effect.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Shift down.
            /// </summary>
            Down = 0,
            /// <summary>
            /// Shift up.
            /// </summary>
            Up = 1,

            /// <summary>
            /// No shifting.
            /// </summary>
            Off = 2,
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.FrequencyShifter"/> class.
        /// </summary>
        public FrequencyShifter()
        {
        }

        #region implemented abstract members of Effect
        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.FrequencyShifter);
            Frequency = _Frequency;
            LeftDirection = _LeftDirection;
            RightDirection = _RightDirection;
            return true;
        }

        #endregion
        private float _Frequency = 0f;
        private Direction _LeftDirection = Direction.Down;
        private Direction _RightDirection = Direction.Down;

        /// <summary>
        /// Gets or sets the right direction.
        /// </summary>
        /// <value>The right direction.</value>
        public Direction RightDirection
        {
            get
            {
                return _RightDirection;
            }
            set
            {
                _RightDirection = value;
                if (Loaded)
                    AL.Effect(ALID, ALEffecti.FrequencyShifterRightDirection, (int)_RightDirection);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the left direction.
        /// </summary>
        /// <value>The left direction.</value>
        public Direction LeftDirection
        {
            get
            {
                return _LeftDirection;
            }
            set
            {
                _LeftDirection = value;
                if (Loaded)
                    AL.Effect(ALID, ALEffecti.FrequencyShifterLeftDirection, (int)_LeftDirection);
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
                _Frequency = value.Clamp(0f, 24000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.FrequencyShifterFrequency, _Frequency);
                TriggerUpdate();
            }
        }
    }
}

