//
//  PitchShifter.cs
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
    /// Simple test implementation of reverb
    /// </summary>
    public class Reverb : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effects.Reverb"/> class.
        /// </summary>
        public Reverb()
        {
        }

        #region implemented abstract members of Effect

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Reverb); //Standard parameters
            #region Parameters
            Gain = _Gain;
            GainHF = _GainHF;
            Diffusion = _Diffusion;
            Density = _Density;
            DecayTime = _DecayTime;
            DecayHFRatio = _DecayHFRatio;
            ReflectionsGain = _ReflectionsGain;
            ReflectionsDelay = _ReflectionsDelay;
            LateReverbGain = _LateReverbGain;
            LateReverbDelay = _LateReverbDelay;
            AirAbsorptionGainHF = _AirAbsorptionGainHF;
            RoomRolloffFactor = _RoomRolloffFactor;
            #endregion
            return true;
        }

        #endregion

        private float _Gain = 0.32f;
        private float _GainHF = 0.89f;
        private float _Diffusion = 1f;
        private float _Density = 1f;
        private float _DecayTime = 1.49f;
        private float _DecayHFRatio = 0.83f;
        private float _ReflectionsGain = 0.05f;
        private float _ReflectionsDelay = 0.007f;
        private float _LateReverbGain = 1.26f;
        private float _LateReverbDelay = 0.011f;
        private float _AirAbsorptionGainHF = 0.994f;
        private float _RoomRolloffFactor = 0f;


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
                AL.Effect(ALID, ALEffectf.ReverbGain, _Gain);
                TriggerUpdate();
            }
        }
            
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
                if(Loaded)
                AL.Effect(ALID, ALEffectf.ReverbGainHF, _GainHF);
                TriggerUpdate();

            }
        }
            
        /// <summary>
        /// Gets or sets the diffusion.
        /// </summary>
        /// <value>The diffusion.</value>
        public float Diffusion
        {
            get
            {
                return _Diffusion;
            }
            set
            {
                _Diffusion = value.Clamp(0f, 1f);
                if(Loaded)
                AL.Effect(ALID, ALEffectf.ReverbDiffusion, _Diffusion);
                TriggerUpdate();
            }
        }
            
        /// <summary>
        /// Gets or sets the density.
        /// </summary>
        /// <value>The density.</value>
        public float Density
        {
            get
            {
                return _Density;
            }
            set
            {
                _Density = value.Clamp(0f, 1f);
                if(Loaded)
                AL.Effect(ALID, ALEffectf.ReverbDensity, _Density);
                TriggerUpdate();
            }
        }
            
        /// <summary>
        /// Gets or sets the decay time.
        /// </summary>
        /// <value>The decay time.</value>
        public float DecayTime
        {
            get
            {
                return _DecayTime;
            }
            set
            {
                _DecayTime = value.Clamp(0.1f, 20f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbDecayTime, _DecayTime);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the decay HF ratio.
        /// </summary>
        /// <value>The decay HF ratio.</value>
        public float DecayHFRatio
        {
            get
            {
                return _DecayHFRatio;
            }
            set
            {
                _DecayHFRatio = value.Clamp(0.1f, 2f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbDecayHFRatio, _DecayHFRatio);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the reflections gain.
        /// </summary>
        /// <value>The reflections gain.</value>
        public float ReflectionsGain
        {
            get
            {
                return _ReflectionsGain;
            }
            set
            {
                _ReflectionsGain = value.Clamp(0f, 3.16f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbReflectionsGain, _ReflectionsGain);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the reflections delay.
        /// </summary>
        /// <value>The reflections delay.</value>
        public float ReflectionsDelay
        {
            get
            {
                return _ReflectionsDelay;
            }
            set
            {
                _ReflectionsDelay = value.Clamp(0f, 0.3f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbReflectionsDelay, _ReflectionsDelay);
                TriggerUpdate();
            }
        }
            
        /// <summary>
        /// Gets or sets the late reverb gain.
        /// </summary>
        /// <value>The late reverb gain.</value>
        public float LateReverbGain
        {
            get
            {
                return _LateReverbGain;
            }
            set
            {
                _LateReverbGain = value.Clamp(0f, 10f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbLateReverbGain, _LateReverbGain);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the late reverb delay.
        /// </summary>
        /// <value>The late reverb delay.</value>
        public float LateReverbDelay
        {
            get
            {
                return _LateReverbDelay;
            }
            set
            {
                _LateReverbDelay = value.Clamp(0f, 0.1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbLateReverbDelay, _LateReverbDelay);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the air absorption gain for high frequencies.
        /// </summary>
        /// <value>The air absorption gain for HF.</value>
        public float AirAbsorptionGainHF
        {
            get
            {
                return _AirAbsorptionGainHF;
            }
            set
            {
                _AirAbsorptionGainHF = value.Clamp(0.892f, 1f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbAirAbsorptionGainHF, _AirAbsorptionGainHF);
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the room rolloff factor.
        /// </summary>
        /// <value>The room rolloff factor.</value>
        public float RoomRolloffFactor
        {
            get
            {
                return _RoomRolloffFactor;
            }
            set
            {
                _RoomRolloffFactor = value.Clamp(0f, 10f);
                if (Loaded)
                    AL.Effect(ALID, ALEffectf.ReverbRoomRolloffFactor, _RoomRolloffFactor);
                TriggerUpdate();
            }
        }
    }
}

