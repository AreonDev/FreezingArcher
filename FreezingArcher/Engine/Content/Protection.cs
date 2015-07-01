//
//  Protection.cs
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
    /// This class describes the protection against forces from the environment.
    /// </summary>
    public sealed class Protection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Protection"/> class.
        /// </summary>
        /// <param name="heat">The heat protection.</param>
        /// <param name="cold">The protection against coldiness.</param>
        /// <param name="hit">The protection against hits.</param>
        public Protection (float heat = 0, float cold = 0, float hit = 0)
        {
            Heat = heat;
            Cold = cold;
            Hit = hit;
        }

        /// <summary>
        /// Gets or sets the heat protection.
        /// </summary>
        /// <value>The heat protection.</value>
        public float Heat { get; set; }

        /// <summary>
        /// Gets or sets the protection against hits.
        /// </summary>
        /// <value>The hit protection.</value>
        public float Hit { get; set; }

        /// <summary>
        /// Gets or sets the protection against coldiness.
        /// </summary>
        /// <value>The coldiness protection.</value>
        public float Cold { get; set; }
    }
}
