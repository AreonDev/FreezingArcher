//
//  WallComponent.cs
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
using FreezingArcher.Content;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Wall component.
    /// </summary>
    public sealed class WallComponent : EntityComponent
    {
        #region defaults

        /// <summary>
        /// The default is edge.
        /// </summary>
        public static readonly bool DefaultIsEdge = false;

        /// <summary>
        /// The default is moveable.
        /// </summary>
        public static readonly bool DefaultIsMoveable = true;

        /// <summary>
        /// The default is moving.
        /// </summary>
        public static readonly bool DefaultIsMoving = false;

        /// <summary>
        /// The default is overworld.
        /// </summary>
        public static readonly bool DefaultIsOverworld = true;

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this instance is edge.
        /// </summary>
        /// <value><c>true</c> if this instance is edge; otherwise, <c>false</c>.</value>
        public bool IsEdge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is moveable.
        /// </summary>
        /// <value><c>true</c> if this instance is moveable; otherwise, <c>false</c>.</value>
        public bool IsMoveable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is moving.
        /// </summary>
        /// <value><c>true</c> if this instance is moving; otherwise, <c>false</c>.</value>
        public bool IsMoving { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is overworld.
        /// </summary>
        /// <value><c>true</c> if this instance is overworld; otherwise, <c>false</c>.</value>
        public bool IsOverworld { get; set; }
    }
}
