//
//  Environment.cs
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
    /// The environment class. One environment is associated with one scene.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// Gets the default environment.
        /// </summary>
        /// <value>The default environment.</value>
        public static Environment Default
        {
            get
            {
                return new Environment(new Weather(), new GameTime(new DateTime(), 3600));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Environment"/> class.
        /// </summary>
        /// <param name="weather">Weather.</param>
        /// <param name="time">Time.</param>
        public Environment(Weather weather, GameTime time)
        {
            Weather = weather;
            Time = time;
        }

        /// <summary>
        /// Gets or sets the weather.
        /// </summary>
        /// <value>The weather.</value>
        public Weather Weather { get; set; }

        /// <summary>
        /// Gets or sets the game time.
        /// </summary>
        /// <value>The game time.</value>
        public GameTime Time { get; set; }
    }
}
