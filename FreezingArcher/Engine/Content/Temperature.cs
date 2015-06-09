//
//  Temperature.cs
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
    /// Temperature class. The purpose if this class is to give an abstraction of a temperature and
    /// enable simple conversion between temperature units.
    /// </summary>
    public class Temperature
    {
        /// <summary>
        /// Convert the value of a temperature object into the given temperature unit and return the
        /// computed value.
        /// </summary>
        /// <returns>The converted temperature value.</returns>
        /// <param name="temp">The temperature whos value should be converted.</param>
        /// <param name="unit">The temperature unit to convert to.</param>
        public static double ValueAs(Temperature temp, TemperatureUnit unit)
        {
            double value = 0;
            if (temp.Unit == TemperatureUnit.Celsius)
            {
                if (unit == TemperatureUnit.Kelvin)
                {
                    // convert celsius to kelvin
                    value = temp.Value - 273.15;
                }
                else if (unit == TemperatureUnit.Fahrenheit)
                {
                    // convert celsius to fahrenheit
                    value = temp.Value * (9 / 5) + 32;
                }
                else if (unit == TemperatureUnit.Rankine)
                {
                    // convert celsius to rankine
                    value = (temp.Value + 273.15) * (9 / 5);
                }
            }
            else if (temp.Unit == TemperatureUnit.Kelvin)
            {
                if (unit == TemperatureUnit.Celsius)
                {
                    // convert kelvin to celsius
                    value = temp.Value + 273.15;
                }
                else if (unit == TemperatureUnit.Fahrenheit)
                {
                    // convert kelvin to fahrenheit
                    value = temp.Value * (9 / 5) - 459.67;
                }
                else if (unit == TemperatureUnit.Rankine)
                {
                    // convert kelvin to rankine
                    value = temp.Value * (9 / 5);
                }
            }
            else if (temp.Unit == TemperatureUnit.Fahrenheit)
            {
                if (unit == TemperatureUnit.Celsius)
                {
                    // convert fahrenheit to celsius
                    value = (temp.Value - 32) * (5 / 9);
                }
                else if (unit == TemperatureUnit.Kelvin)
                {
                    // convert fahrenheit to kelvin
                    value = (temp.Value + 459.67) * (5 / 9);
                }
                else if (unit == TemperatureUnit.Rankine)
                {
                    // convert fahrenheit to rankine
                    value = temp.Value + 459.67;
                }
            }
            else if (temp.Unit == TemperatureUnit.Rankine)
            {
                if (unit == TemperatureUnit.Celsius)
                {
                    // convert rankine to celsius
                    value = (temp.Value - 491.67) * (5 / 9);
                }
                else if (unit == TemperatureUnit.Kelvin)
                {
                    // convert rankine to kelvin
                    value = temp.Value * (5 / 9);
                }
                else if (unit == TemperatureUnit.Fahrenheit)
                {
                    // convert rankine to fahrenheit
                    value = temp.Value - 459.67;
                }
            }
            return value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Temperature"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="unit">Temperature unit.</param>
        public Temperature(double value, TemperatureUnit unit = TemperatureUnit.Celsius)
        {
            Value = value;
            this.unit = unit;
        }

        /// <summary>
        /// Gets or sets the value of this temperature.
        /// </summary>
        /// <value>The value.</value>
        public double Value { get; set; }

        TemperatureUnit unit;

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>The unit.</value>
        public TemperatureUnit Unit
        {
            get
            {
                return unit;
            }
            set
            {
                Value = ValueAs(this, value);
                unit = value;
            }
        }

        /// <summary>
        /// Return the value of this temperature converted into Celsius.
        /// </summary>
        /// <returns>The celsius value.</returns>
        public double AsCelsius()
        {
            return ValueAs(this, TemperatureUnit.Celsius);
        }

        /// <summary>
        /// Return the value of this temperature converted into Kelvin.
        /// </summary>
        /// <returns>The Kelvin value.</returns>
        public double AsKelvin()
        {
            return ValueAs(this, TemperatureUnit.Kelvin);
        }

        /// <summary>
        /// Return the value of this temperature converted into Fahrenheit.
        /// </summary>
        /// <returns>The Fahrenheit value.</returns>
        public double AsFahrenheit()
        {
            return ValueAs(this, TemperatureUnit.Fahrenheit);
        }

        /// <summary>
        /// Return the value of this temperature converted into Rankine.
        /// </summary>
        /// <returns>The Rankine value.</returns>
        public double AsRankine()
        {
            return ValueAs(this, TemperatureUnit.Rankine);
        }
    }
}
