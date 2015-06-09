//
//  Downfall.cs
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
    /// This class is used to describe the downfall.
    /// </summary>
    public class Downfall
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Downfall"/> class.
        /// </summary>
        /// <param name="type">The type of downfall.</param>
        /// <param name="strength">The strength of the downfall.</param>
        public Downfall(DownfallType type, double strength)
        {
            Type = type;
            Strength = strength;
        }

        /// <summary>
        /// Gets or sets the type of the downfall.
        /// </summary>
        /// <value>The downfall type.</value>
        public DownfallType Type { get; set; }

        /// <summary>
        /// Gets or sets the strength of the downfall.
        /// </summary>
        /// <value>The downfall strength.</value>
        public double Strength { get; set; }
    }
}
