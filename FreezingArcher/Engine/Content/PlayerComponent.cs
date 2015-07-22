//
//  PlayerComponent.cs
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

namespace FreezingArcher.Content
{
    /// <summary>
    /// Player component.
    /// </summary>
    public sealed class PlayerComponent : EntityComponent
    {
        #region defaults

        /// <summary>
        /// The default health.
        /// </summary>
        public static readonly float DefaultHealth = 100f;

        /// <summary>
        /// The default maximum health.
        /// </summary>
        public static readonly float DefaultMaximumHealth = 100f;

        #endregion

        /// <summary>
        /// Gets or sets the health.
        /// </summary>
        /// <value>The health.</value>
        public float Health { get; set; }

        /// <summary>
        /// Gets or sets the maximum health.
        /// </summary>
        /// <value>The maximum health.</value>
        public float MaximumHealth { get; set; }
    }
}
