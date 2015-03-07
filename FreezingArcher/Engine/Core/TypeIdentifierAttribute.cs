//
//  TypeIdentifierAttribute().cs
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

namespace FreezingArcher.Core
{
    /// <summary>
    /// Attribute to game-unique identify types
    /// <remarks>>Must be set on all non-abstract classes which inherit from <see cref="FreezingArcher.Core.FAObject"/></remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]

    public class TypeIdentifierAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.TypeIdentifierAttribute"/> class.
        /// </summary>
        /// <param name="TypeID">Type ID.</param>
        public TypeIdentifierAttribute(ushort TypeID)
        {
            this.TypeID = TypeID;
        }

        /// <summary>
        /// Gets the type ID.
        /// </summary>
        /// <value>The type ID.</value>
        public ushort TypeID {get; private set;}
    }
}

