//
//  ValueType.cs
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
    /// Value type enum.
    /// </summary>
    public enum ValueType : byte
    {
        /// <summary>
        /// Boolean value.
        /// </summary>
        Boolean,
        /// <summary>
        /// Integer value.
        /// </summary>
        Integer,
        /// <summary>
        /// Double value.
        /// </summary>
        Double,
        /// <summary>
        /// String value.
        /// </summary>
        String,
        /// <summary>
        /// Byte array value.
        /// </summary>
        Bytes
    }
}
