//
//  Extensions.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2014 Fin Christensen, Martin Koppehel
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
using System;
using System.Linq;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Linq extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the attribute with type T from the given object, inheritance is used
        /// </summary>
        /// <returns>The attribute if found, otherwise null</returns>
        /// <param name="o">Object to read attributes from</param>
        /// <param name="inherit">If set to <c>true</c> inherited attributes are scanned</param>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        public static T GetAttribute<T>(this object o, bool inherit) where T: Attribute
        {
            var attribs = o.GetType().GetCustomAttributes(typeof(T), inherit);
            if (attribs.Length > 0)
                return attribs[0] as T;
            return null;
        }

        /// <summary>
        /// Gets the attribute with type T from the given type, inheritance is used
        /// </summary>
        /// <returns>The attribute if found, otherwise null</returns>
        /// <param name="t">Type to read attributes from</param>
        /// <param name="inherit">If set to <c>true</c> inherited attributes are scanned</param>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        public static T GetAttribute<T>(this Type t, bool inherit) where T: Attribute
        {
            var attribs = t.GetCustomAttributes(typeof(T), inherit);
            if (attribs.Length > 0)
                return attribs[0] as T;
            return null;
        }

        /// <summary>
        /// Performs an action for each element in an IEnumerable
        /// </summary>
        /// <typeparam name="T">type of the enumerable</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="func">The function.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func)
        {
            foreach (var item in enumerable)
                func(item);
        }

        /// <summary>
        /// Get minimum and maximum out of a list of doubles.
        /// </summary>
        /// <param name="enumerable">Enumerable (list).</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static void MinMax(this IEnumerable<double> enumerable, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            foreach (var item in enumerable)
            {
                if (item < min)
                    min = item;
                else if (item > max)
                    max = item;
            }
        }

        /// <summary>
        /// Get minimum and maximum out of a list of integers.
        /// </summary>
        /// <param name="enumerable">Enumerable (list).</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static void MinMax(this IEnumerable<int> enumerable, out int min, out int max)
        {
            min = int.MaxValue;
            max = int.MinValue;
            foreach (var item in enumerable)
            {
                if (item < min)
                    min = item;
                else if (item > max)
                    max = item;
            }
        }

        /// <summary>
        /// Is double between range of min and max.
        /// </summary>
        /// <returns><c>true</c>, if it is between, <c>false</c> otherwise.</returns>
        /// <param name="d">Double.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static bool IsInside(this double d, double min, double max)
        {
            return d > min && d < max;
        }

        /// <summary>
        /// Get absolute value of a double.
        /// </summary>
        /// <param name="d">Double.</param>
        public static double Abs(this double d)
        {
            return d < 0 ? -d : d;
        }


        /// <summary>
        /// Joins the sequence.
        /// </summary>
        /// <returns>The sequence.</returns>
        /// <param name="src">Source.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> JoinSequence<T>(this IEnumerable<IEnumerable<T>> src)
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
        public static IEnumerable<T> JoinSequence<T>(this IEnumerable<IEnumerable<T>> src, T separator)
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
        public static string Join<T>(this IEnumerable<T> joiner, string separator = ", ")
        {
            string res = "";
            foreach (var item in joiner)
            {
                res += item.ToString() + separator;
            }
            return res.Substring(0, res.Length - separator.Length);
        }

        /// <summary>
        /// Convert IEnumerable to a collection.
        /// </summary>
        /// <returns>The collection.</returns>
        /// <param name="t">T.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> ToCollection<T>(this T t)
        {
            yield return t;
        }

        /// <summary>
        /// Clamp the specified val between min and max.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static double Clamp(this double val, double min, double max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Clamp the specified val between min and max.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static float Clamp(this float val, float min, float max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Clamp the specified val between min and max.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        public static int Clamp(this int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Gets the friendly name of the given type.
        /// </summary>
        /// <returns>The friendly name.</returns>
        /// <param name="type">Type.</param>
        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].Name;
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        /// <summary>
        /// Get the minimal element of a collection by a selector which selects the minimal element by choosing
        /// properties from this element.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="source">Source.</param>
        /// <param name="selector">Selector.</param>
        /// <typeparam name="TSource">The element type.</typeparam>
        /// <typeparam name="TComp">The type of the comparable part of the element.</typeparam>
        public static TSource MinElem<TSource, TComp>(this IEnumerable<TSource> source, Func<TSource, TComp> selector)
            where TComp : IComparable
        {
            TSource src = default(TSource);
            TComp comp = default(TComp);
            TComp tmp;
            foreach (var i in source)
            {
                tmp = selector(i);
                if (comp.Equals(default(TComp)) || comp.CompareTo(tmp) < 0)
                {
                    src = i;
                    comp = tmp;
                }
            }
            return src;
        }

        /// <summary>
        /// Select even elements of the enumerable.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <typeparam name="TSource">The source type of the enumerable.</typeparam>
        public static IEnumerable<TSource> Even<TSource>(this IEnumerable<TSource> source)
        {
            return source.Where((c, i) => i % 2 == 0);
        }

        /// <summary>
        /// Select odd elements of the enumerable.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <typeparam name="TSource">The source type of the enumerable.</typeparam>
        public static IEnumerable<TSource> Odd<TSource>(this IEnumerable<TSource> source)
        {
            return source.Where((c, i) => i % 2 != 0);
        }
    }
}
