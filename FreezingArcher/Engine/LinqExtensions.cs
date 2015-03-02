//
//  LinqExtensions.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
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
using System.Collections.Generic;

namespace FreezingArcher
{
    /// <summary>
    /// Linq extensions.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Joins the sequence.
        /// </summary>
        /// <returns>The sequence.</returns>
        /// <param name="src">Source.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> JoinSequence<T> (this IEnumerable<IEnumerable<T>> src)
        {
            foreach (var item in src)
            {
                foreach (var i in item)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Joins the sequence.
        /// </summary>
        /// <returns>The sequence.</returns>
        /// <param name="src">Source.</param>
        /// <param name="separator">Separator.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> JoinSequence<T> (this IEnumerable<IEnumerable<T>> src, T separator)
        {
            foreach (var item in src)
            {
                foreach (var i in item)
                {
                    yield return i;
                }
                yield return separator;
            }
        }

        /// <summary>
        /// Join the specified enumerable and use the separator as the separator.
        /// </summary>
        /// <param name="joiner">Enumerable.</param>
        /// <param name="separator">Separator.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string Join<T> (this IEnumerable<T> joiner, string separator = ", ")
        {
            string res = "";
            foreach (var item in joiner)
            {
                res += item + separator;
            }
            return res.Substring (0, res.Length - separator.Length);
        }

        /// <summary>
        /// Clamp the specified val between min and max.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static float Clamp (this float val, float min, float max)
        {
            return val < min ? min : val > max ? max : val;
        }
    }
}
