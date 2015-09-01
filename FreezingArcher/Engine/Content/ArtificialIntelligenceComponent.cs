//
//  ArtificialIntelligenceComponent.cs
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

namespace FreezingArcher.Content
{
    /// <summary>
    /// Artificial intelligence component.
    /// </summary>
    public sealed class ArtificialIntelligenceComponent : EntityComponent
    {
        #region defaults

        /// <summary>
        /// The default artificial intelligence.
        /// </summary>
        public static readonly ArtificialIntelligence DefaultArtificialIntelligence = null;

        /// <summary>
        /// The default AI manager.
        /// </summary>
        public static readonly AIManager DefaultAIManager = null;

        /// <summary>
        /// The default maximum distance.
        /// </summary>
        public static readonly float DefaultMaximumEntityDistance = 10f;

        #endregion

        /// <summary>
        /// Gets or sets the artificial intelligence.
        /// </summary>
        /// <value>The artificial intelligence.</value>
        public ArtificialIntelligence ArtificialIntelligence { get; set; }

        /// <summary>
        /// Gets or sets the AI manager.
        /// </summary>
        /// <value>The AI manager.</value>
        public AIManager AIManager { get; set; }

        /// <summary>
        /// Gets or sets the maximum enity distance.
        /// </summary>
        /// <value>The maximum enity distance.</value>
        public float MaximumEntityDistance { get; set; }
    }
}
