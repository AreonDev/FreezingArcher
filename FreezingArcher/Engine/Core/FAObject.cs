//
//  FAObject.cs
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
using FreezingArcher.Core.Interfaces;

namespace FreezingArcher.Core
{

    /// <summary>
    /// Base class for game objects which allows the user to recycle and compare objects really fast
    /// </summary>
    public abstract class FAObject
    {
        public FAObject()
        {
            TypeId = GetType().GetHashCode();
        }

        protected ObjectManager ObjectManager;

        internal uint InstId { get; private set; }

        internal int TypeId { get; private set; }

        internal bool Destroyed;

        internal void Init(ObjectManager manager, uint id)
        {
            ObjectManager = manager;
            InstId = id;
        }

        /// <summary>
        /// Recycle this instance.
        /// </summary>
        public virtual void Recycle()
        {
            Destroyed = false;
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public virtual void Destroy()
        {
            Destroyed = true;
            ObjectManager.PrepareForRecycling(this);
        }

        public override string ToString()
        {
            return string.Format("FAObject ({0}, Instance: {1})", TypeId, InstId);
        }
        public override bool Equals(object obj)
        {
            var fa = obj as FAObject;
            if (fa == null)
                return false;
            return fa.InstId == InstId && fa.TypeId == TypeId;
        }

        public override int GetHashCode()
        {
            return unchecked((int)(TypeId ^ InstId));               
        }
    }
}

