//
//  Value.cs
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

namespace FreezingArcher.Configuration
{
    /// <summary>
    /// Value class for configuration files.
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.Value"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Value (bool value)
        {
            Boolean = value;
            Type = ValueType.Boolean;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.Value"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Value (int value)
        {
            Integer = value;
            Type = ValueType.Integer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.Value"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Value (double value)
        {
            Double = value;
            Type = ValueType.Double;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.Value"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Value (string value)
        {
            String = value;
            Type = ValueType.String;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.Value"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Value (byte[] value)
        {
            Bytes = value;
            Type = ValueType.Bytes;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ValueType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the byte values.
        /// </summary>
        /// <value>The bytes.</value>
        public byte[] Bytes { get; protected set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>The string.</value>
        public string String { get; protected set; }

        /// <summary>
        /// Gets or sets the integer value.
        /// </summary>
        /// <value>The integer.</value>
        public int Integer { get; protected set; }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>The double.</value>
        public double Double { get; protected set; }

        /// <summary>
        /// Gets or sets the boolean value.
        /// </summary>
        /// <value>The boolean.</value>
        public bool Boolean { get; protected set; }
    }
}
