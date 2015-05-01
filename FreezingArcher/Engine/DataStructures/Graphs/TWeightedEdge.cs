//
//  TWeightedEdge.cs
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

namespace FreezingArcher.DataStructures.Graphs
{
    /// <summary>
    /// Edge for use in graphs.
    /// </summary>
    public sealed class WeightedEdge<TData, TWeight> : FAObject where TWeight : IComparable
    {
        /// <summary>
        /// Initialize this edge with the given data.
        /// </summary>
        /// <param name="weight">Edge weight.</param>
        /// <param name="firstNode">First node.</param>
        /// <param name="secondNode">Second node.</param>
        internal void Init (TWeight weight, WeightedNode<TData, TWeight> firstNode, WeightedNode<TData, TWeight> secondNode)
        {
            Weight = weight;
            FirstNode = firstNode;
            SecondNode = secondNode;
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public TWeight Weight { get; set; }

        /// <summary>
        /// Gets the first node.
        /// </summary>
        /// <value>The source node.</value>
        public WeightedNode<TData, TWeight> FirstNode { get; internal set; }

        /// <summary>
        /// Gets the second node.
        /// </summary>
        /// <value>The destination node.</value>
        public WeightedNode<TData, TWeight> SecondNode { get; internal set; }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public override void Destroy()
        {
            Weight = default(TWeight);
            FirstNode = null;
            SecondNode = null;
            base.Destroy();
        }
    }
}
