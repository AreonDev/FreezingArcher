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
        /// <summary>
        /// The object manager.
        /// </summary>
        protected ObjectManager ObjectManager;
        // Game-unique identifier for objects
        internal ulong ID {get; private set; }
        internal bool Destroyed {get; private set;}

        internal void Init(ObjectManager manager, uint id)
        {
            this.ID = ((ulong)this.GetAttribute<TypeIdentifierAttribute>(false).TypeID << 48) | (ulong)id;
            this.ObjectManager = manager;
        }

        /// <summary>
        /// Recycle this instance.
        /// </summary>
        public void Recycle()
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="FreezingArcher.Core.FAObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="FreezingArcher.Core.FAObject"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="FreezingArcher.Core.FAObject"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var fAObject = obj as FAObject;
            if (fAObject != null)
                return fAObject.ID == ID;
            else
                return false;
        }
        /// <param name="lhs">Left object</param>
        /// <param name="rhs">Right object</param>
        public static bool operator== (FAObject lhs, FAObject rhs)
        {
            return lhs.ID == rhs.ID; 
            //  will determine if
            //a) types are equal
            //b) instance is equal
        }

        /// <param name="lhs">Left object</param>
        /// <param name="rhs">Right object</param>
        public static bool operator!= (FAObject lhs, FAObject rhs)
        {
            return lhs.ID != rhs.ID;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="FreezingArcher.Core.FAObject"/> object.
        /// </summary>
        /// <remarks>>This hash function is collision-free for up to 2^16 objects, probable more.</remarks>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode ()
        {
            return (int)(((this.ID & 0xFFFF000000000000) >> 32) ^ (this.ID & 0x00000000FFFFFFFF));
            //hash the object type as higher 16 bits into the lower 32bits from instance
        }
    }
}

