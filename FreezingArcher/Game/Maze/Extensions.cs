//
//  Extensions.cs
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
using FreezingArcher.DataStructures.Graphs;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the neighbour nodes.
        /// </summary>
        /// <returns>The neighbour nodes.</returns>
        /// <param name="node">The node.</param>
        /// <typeparam name="TData">Data type.</typeparam>
        /// <typeparam name="TWeight">Weight type.</typeparam>
        public static IEnumerable<Tuple<WeightedNode<TData, TWeight>, WeightedEdge<TData, TWeight>>>
        GetNeighbours<TData, TWeight> (this WeightedNode<TData, TWeight> node)
            where TWeight : IComparable
        {
            foreach (var e in node.Edges)
                yield return e.FirstNode != node ?
                    new Tuple<WeightedNode<TData, TWeight>, WeightedEdge<TData, TWeight>> (e.FirstNode, e) :
                    new Tuple<WeightedNode<TData, TWeight>, WeightedEdge<TData, TWeight>> (e.SecondNode, e);
        }
    }
}
