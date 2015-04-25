//
//  TEdge.cs
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
    [TypeIdentifier (1)]
    public class Edge<T> : FAObject where T : IComparable
    {
        /// <summary>
        /// Initialize this edge with the given data.
        /// </summary>
        /// <param name="weight">Edge weight.</param>
        /// <param name="startNode">Source node identifier.</param>
        /// <param name="endNode">Destination node identifier.</param>
        /// <param name="directed">If set to <c>true</c> this edge will have a direction.</param>
        internal void Init (T weight, int startNode, int endNode, bool directed)
        {
            Weight = weight;
            StartNode = startNode;
            EndNode = endNode;
            Directed = directed;
        }

        /// <summary>
        /// Gets the edge identifier.
        /// </summary>
        /// <value>The edge identifier.</value>
        public int EdgeIdentifier
        {
            get
            {
                return GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public T Weight { get; set; }

        /// <summary>
        /// Gets the start node.
        /// </summary>
        /// <value>The start node.</value>
        public int StartNode { get; internal set; }

        /// <summary>
        /// Gets the end node.
        /// </summary>
        /// <value>The end node.</value>
        public int EndNode { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this edge is directed.
        /// </summary>
        /// <value><c>true</c> if directed; otherwise, <c>false</c>.</value>
        public bool Directed { get; internal set; }
    }
}
