//
//  ParserUtils.cs
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
using System.Text.RegularExpressions;
using FreezingArcher.Output;
using FreezingArcher.Math;
using FreezingArcher.Core;

namespace FreezingArcher.Configuration
{
    /// <summary>
    /// Utilities for parsing strings.
    /// </summary>
    public static class ParserUtils
    {
        /// <summary>
        /// Parses a vector from a string like 1024x576.
        /// </summary>
        /// <returns>The vector.</returns>
        /// <param name="val">Value.</param>
        public static Vector2i ParseVector (string val)
        {
            Regex r = new Regex ("(\\d+)x(\\d+)");
            Match m = r.Match (val);

            if (m.Success)
            {
                int x,y;
                int.TryParse (m.Groups[1].Value, out x);
                int.TryParse (m.Groups[2].Value, out y);
                return new Vector2i (x, y);
            }

            Logger.Log.AddLogEntry (LogLevel.Error, "ConfigManager#ParseVector", Status.BadData,
                "Could not parse a vector from string! Have you messed up your resolution config?");
            return new Vector2i ();
        }
    }
}
