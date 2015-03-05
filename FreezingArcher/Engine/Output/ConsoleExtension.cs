//
//  ConsoleExtension.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2015 Martin Koppehel
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

namespace FreezingArcher.Output
{
    /// <summary>
    /// static helper class providing some extension methods
    /// </summary>
    public static class ConsoleExtension
    {
        /// <summary>
        /// Writes a line with the specified color
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="color">The color.</param>
        public static void WriteLine (string line, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            if (Console.BufferWidth != 0 && line.Length > Console.BufferWidth - 4)
                line = line.Substring (0, Console.BufferWidth - 4) + " ...";
            Console.WriteLine (line);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
