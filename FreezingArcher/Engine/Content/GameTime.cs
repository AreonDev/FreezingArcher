//
//  GameTime.cs
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
    /// Game time class.
    /// </summary>
    public class GameTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.GameTime"/> class.
        /// </summary>
        /// <param name="dateTime">Date time.</param>
        /// <param name="lengthOfDay">Length of one day in real time seconds.</param>
        public GameTime(DateTime dateTime, long lengthOfDay)
        {
            DateTime = dateTime;
            LengthOfDay = lengthOfDay;
        }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the length of day in realtime seconds.
        /// </summary>
        /// <value>The length of day.</value>
        public long LengthOfDay { get; set; }
    }
}

