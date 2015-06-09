//
//  ITTree.cs
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
using System.Collections.Generic;

namespace FreezingArcher.DataStructures.Trees
{
    /// <summary>
    /// Represents an tree of arbitrary data
    /// </summary>
    public interface ITree<TData> : IEnumerable<TData>, IEnumerable<ITree<TData>>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is the root node.
        /// </summary>
        /// <value><c>true</c> if this instance is the root; otherwise, <c>false</c>.</value>
        bool IsRoot{ get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a leaf.
        /// </summary>
        /// <value><c>true</c> if this instance is a leaf; otherwise, <c>false</c>.</value>
        bool IsLeaf{ get; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        TData Data { get; set; }

        /// <summary>
        /// Gets the level of this node.
        /// </summary>
        /// <value>The level.</value>
        int Level { get; }
    }
}

