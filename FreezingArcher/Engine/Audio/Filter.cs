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
    public abstract class Filter : IResource
    {
        protected Filter()
        {
            Loaded = false;
        }

        #region IResource implementation

        public event Handler NeedsLoad;

        public List<Action> GetInitJobs(List<Action> list)
        {
            return list;
        }

        public List<Action> GetLoadJobs(List<Action> list, Handler reloader)
        {
            NeedsLoad = reloader;
            list.Add(this.Create);
            return list;
        }

        public void Destroy()
        {
            if (!Loaded)
                return;
            AL.DeleteFilters(new uint[]{ ALID });
        }

        public bool Loaded
        {
            get;
            private set;
        }

        #endregion

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
        /// Occurs when update.
        /// </summary>
        public event EventHandler Update;
        protected void TriggerUpdate()
        {
            if (Update != null)
                Update (this, EventArgs.Empty);
        }
    }
}

