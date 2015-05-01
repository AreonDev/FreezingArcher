//
//  TWeightedNode.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
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
using FreezingArcher.Core;
using System.Collections.Generic;

namespace FreezingArcher.DataStructures.Graphs
{
    /// <summary>
    /// Node for use in graphs.
    /// </summary>
    public sealed class WeightedNode<TData, TWeight> : FAObject where TWeight : IComparable
    {
        /// <summary>
        /// Initialize this node with data.
        /// </summary>
        /// <param name="data">Data this node should hold.</param>
        internal void Init (TData data)
        {
            Data = data;

            if (InternalEdges == null)
                InternalEdges = new List<WeightedEdge<TData, TWeight>>();
            else
                InternalEdges.Clear();
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public TData Data { get; set; }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        public ReadOnlyList<WeightedEdge<TData, TWeight>> Edges
        {
            get
            {
                return InternalEdges;
            }
        }

        /// <summary>
        /// The internal edges.
        /// </summary>
        internal List<WeightedEdge<TData, TWeight>> InternalEdges;

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public override void Destroy()
        {
            Data = default(TData);
            InternalEdges.Clear();
            base.Destroy();
        }
    }
}
