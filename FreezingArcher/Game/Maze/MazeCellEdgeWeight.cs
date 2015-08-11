//
//  MazeCellEdgeWeight.cs
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

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Maze cell edge weight.
    /// </summary>
    public class MazeCellEdgeWeight : IComparable<MazeCellEdgeWeight>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.Maze.MazeCellEdgeWeight"/> class.
        /// </summary>
        /// <param name="even">If set to <c>true</c> even.</param>
        /// <param name="direction">Direction.</param>
        public MazeCellEdgeWeight(bool even, Direction direction, bool isNextGenerationStep = false)
        {
            Even = even;
            Direction = direction;
            IsNextGenerationStep = isNextGenerationStep;
        }

        /// <summary>
        /// The even flag.
        /// </summary>
        public readonly bool Even;

        /// <summary>
        /// The direction.
        /// </summary>
        public readonly Direction Direction;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is next generation step.
        /// </summary>
        /// <value><c>true</c> if this instance is next generation step; otherwise, <c>false</c>.</value>
        public bool IsNextGenerationStep { get; set; }

        #region IComparable implementation

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <returns>The compare result.</returns>
        /// <param name="other">The value to compare to.</param>
        public int CompareTo (MazeCellEdgeWeight other)
        {
            return other != null ? Even.CompareTo (other.Even) : -1;
        }

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <returns>The compare result.</returns>
        /// <param name="obj">The value to compare to.</param>
        public int CompareTo (object obj)
        {
            return CompareTo (obj as MazeCellEdgeWeight);
        }

        #endregion
    }
}
