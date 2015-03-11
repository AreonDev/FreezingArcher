//
//  Effect.cs
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
using Pencil.Gaming.Audio;
using System;
using FreezingArcher.Core.Interfaces;
using System.Collections.Generic;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// Abstract base class for effects such as reverb, echo and flanger
    /// </summary>
    public abstract class Effect : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Effect"/> class.
        /// </summary>
        protected Effect()
        {
            Create();
        }

        #region IDisposable implementation



        /// <summary>
        /// Releases all resource used by the <see cref="FreezingArcher.Audio.Effect"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="FreezingArcher.Audio.Effect"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="FreezingArcher.Audio.Effect"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="FreezingArcher.Audio.Effect"/> so the garbage collector can reclaim the memory that the
        /// <see cref="FreezingArcher.Audio.Effect"/> was occupying.</remarks>
        public void Dispose()
        {
            AL.DeleteEffects(new uint[]{ ALID });
        }

        #endregion

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected abstract bool Initialize();
        void Create()
        {
            var effect = new uint[1];
            AL.GenEffects(effect);
            if (AL.GetError() == (int)ALError.NoError)
            {
                ALID = effect[0];
                Initialize(); //set additional parameters
            }
        }

        internal uint ALID {get; private set;}
        /// <summary>
        /// Occurs when the effect is updated and needs to be rebound
        /// </summary>
        public event EventHandler Update;
        /// <summary>
        /// Triggers the update.
        /// </summary>
        protected void TriggerUpdate()
        {
            if (Update != null)
                Update(this, EventArgs.Empty);
        }
    }
}

