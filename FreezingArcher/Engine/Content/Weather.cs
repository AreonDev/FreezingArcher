//
//  Weather.cs
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
using System.Globalization;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// This class is an abstraction of the ingame weather.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Weather"/> class.
        /// </summary>
        public Weather()
        {
        }

        /// <summary>
        /// Gets or sets the temperature.
        /// </summary>
        /// <value>The temperature.</value>
        public Temperature Temperature { get; set; }

        /// <summary>
        /// Gets or sets the visibility range.
        /// </summary>
        /// <value>The visibility range.</value>
        public double VisibilityRange { get; set; }

        /// <summary>
        /// Gets or sets the cloudiness. It accepts values from 0 to 1.
        /// </summary>
        /// <value>The cloudiness.</value>
        public double Cloudiness
        {
            get
            {
                return cloudiness;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "Weather",
                        "The cloudiness should be a value between 0 and 1 but is {0}!", cloudiness);
                    return;
                }
                cloudiness = value;
            }
        }

        double cloudiness;

        /// <summary>
        /// Gets or sets the downfall.
        /// </summary>
        /// <value>The downfall.</value>
        public Downfall Downfall { get; set; }

        /// <summary>
        /// Gets or sets the wind.
        /// </summary>
        /// <value>The wind.</value>
        public Wind Wind { get; set; }

        /// <summary>
        /// Gets or sets the daylight time. This property describes sunrise and sunset.
        /// </summary>
        /// <value>The daylight time.</value>
        public DaylightTime DaylightTime { get; set; }
    }
}
