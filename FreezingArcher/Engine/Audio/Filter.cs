//
//  Filter.cs
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
    /// Abstract base class for filters (LP, HP, BP)
    /// </summary>
    public abstract class Filter : IResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Filter"/> class.
        /// </summary>
        protected Filter()
        {
            Loaded = false;
        }

        #region IResource implementation

        /// <summary>
        /// Fire this event when you need the binded load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        public event Handler NeedsLoad;

        /// <summary>
        /// Gets the init jobs. The init jobs may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs(List<Action> list)
        {
            return list;
        }

        /// <summary>
        /// Gets the load jobs. The load jobs will be executed sequentially in the gl thread.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs(List<Action> list, Handler reloader)
        {
            NeedsLoad = reloader;
            list.Add(this.Create);
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the garbage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy()
        {
            if (!Loaded)
                return;
            AL.DeleteFilters(new uint[]{ ALID });
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FreezingArcher.Audio.Filter"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected abstract bool Initialize();

        void Create()
        {
            var ids = new uint[1];
            AL.GenFilters(ids);
            if (AL.GetError() == (int)ALError.NoError)
            {
                ALID = ids[0];
                Loaded = Initialize();
            }
        }

        internal uint ALID {get; private set;}
        /// <summary>
        /// Occurs when an update is needed.
        /// </summary>
        public event EventHandler Update;

        /// <summary>
        /// Triggers the "Update" event handler.
        /// </summary>
        protected void TriggerUpdate()
        {
            if (Update != null)
                Update (this, EventArgs.Empty);
        }
    }
}

