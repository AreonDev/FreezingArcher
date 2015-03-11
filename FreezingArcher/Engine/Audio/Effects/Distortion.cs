//
//  Distortion.cs
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
    /// Distortion effect. For details headover to the effects extension guide.
    /// </summary>
    public class Distortion : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Distortion"/> class.
        /// </summary>
        public Distortion()
        {
        }

        #region implemented abstract members of Effect

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Distortion);
            EQBandwidth = _EQBandwidth;
            EQCenter = _EQCenter;
            LowpassCutoff = _LowpassCutoff;
            DistortionEdge = _DistortionEdge;
            DistortionGain = _DistortionGain;
            return true;
        }

        #endregion

        private float _DistortionEdge; 
        private float _DistortionGain; 
        private float _LowpassCutoff; 
        private float _EQCenter; 
        private float _EQBandwidth; 

        /// <summary>
        /// Gets or sets the EQ bandwidth.
        /// </summary>
        /// <value>The EQ bandwidth.</value>
        public float EQBandwidth
        {
            get
            {
                return _EQBandwidth;
            }
            set
            {
                _EQBandwidth = value.Clamp(80f, 24000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.DistortionEQBandwidth, _EQBandwidth);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the EQ center.
        /// </summary>
        /// <value>The EQ center.</value>
        public float EQCenter
        {
            get
            {
                return _EQCenter;
            }
            set
            {
                _EQCenter = value.Clamp(80f, 24000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.DistortionEQCenter, _EQCenter);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the lowpass cutoff.
        /// </summary>
        /// <value>The lowpass cutoff.</value>
        public float LowpassCutoff
        {
            get
            {
                return _LowpassCutoff;
            }
            set
            {
                _LowpassCutoff = value.Clamp(80f, 24000f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.DistortionLowpassCutoff, _LowpassCutoff);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the distortion gain.
        /// </summary>
        /// <value>The distortion gain.</value>
        public float DistortionGain
        {
            get
            {
                return _DistortionGain;
            }
            set
            {
                _DistortionGain = value.Clamp(0.01f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.DistortionGain, _DistortionGain);
                TriggerUpdate();
            }
        }
        /// <summary>
        /// Gets or sets the distortion edge.
        /// </summary>
        /// <value>The distortion edge.</value>
        public float DistortionEdge
        {
            get
            {
                return _DistortionEdge;
            }
            set
            {
                _DistortionEdge = value.Clamp(0f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.DistortionEdge, _DistortionEdge);
                TriggerUpdate();
            }
        }
    }
}

