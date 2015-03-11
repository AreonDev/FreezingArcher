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
    public class Reverb : Effect
    {
        public Reverb()
        {

        }

        #region implemented abstract members of Effect

        protected override bool Initialize()
        {
            AL.EffectType(ALID, ALEffectType.Reverb);
            AL.Effect(ALID, ALEffectf.ReverbDensity, 1f);
            AL.Effect(ALID, ALEffectf.ReverbDecayTime, 5f);
            AL.Effect(ALID, ALEffectf.ReverbDiffusion, 1f);
            return true;
        }
        #endregion
    }
}

