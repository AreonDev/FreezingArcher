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
using System.Windows.Threading;
using Jitter.LinearMath;
using FreezingArcher.Math;

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
        /// Performs an action for each element in an IEnumerable.
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
        /// Performs an action for each element in an IEnumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="func">Function to execute on each element.</param>
        /// <typeparam name="T">Type of the enumerable.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            foreach (var item in enumerable)
                if (!func(item))
                    break;
        }

        /// <summary>
        /// Performs an action for each element in an IEnumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="func">Function to execute on each element.</param>
        /// <typeparam name="T1">Type of the first tuple item.</typeparam>
        /// <typeparam name="T2">Type of the second tuple item.</typeparam>
        public static void ForEach<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable, Action<T1, T2> func)
        {
            foreach (var tuple in enumerable)
                func(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// Performs an action for each element in an IEnumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="func">Function to execute on each element.</param>
        /// <typeparam name="T1">Type of the first tuple item.</typeparam>
        /// <typeparam name="T2">Type of the second tuple item.</typeparam>
        public static void ForEach<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable, Func<T1, T2, bool> func)
        {
            foreach (var tuple in enumerable)
                if (!func(tuple.Item1, tuple.Item2))
                    break;
        }

        /// <summary>
        /// Gets the index of the given item in the enumerable.
        /// </summary>
        /// <returns>The index. (-1 if item not found)</returns>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="item">Item.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
        {
            int i = 0;
            foreach (var e in enumerable)
            {
                if (e.Equals(item))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Returns if any of the elements in the given enumerable matches the given predicate.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static bool Any<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable, Func<T1, T2, bool> predicate)
        {
            return enumerable.Any(t => predicate(t.Item1, t.Item2));
        }

        /// <summary>
        /// Returns an enumerable containing each element which matches the predicate.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static IEnumerable<Tuple<T1, T2>>
        Where<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable, Func<T1, T2, bool> predicate)
        {
            return enumerable.Where(i => predicate(i.Item1, i.Item2));
        }

        /// <summary>
        /// Returns the first or default element matching the given predicate.
        /// </summary>
        /// <returns>The or default.</returns>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Tuple<T1, T2> FirstOrDefault<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable,
            Func<T1, T2, bool> predicate)
        {
            var res = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2));
            return res ?? new Tuple<T1, T2>(default(T1), default(T2));
        }

        /// <summary>
        /// Count the matches of the predicate in the given enumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static int Count<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable,
            Func<T1, T2, bool> predicate)
        {
            return enumerable.Count(i => predicate(i.Item1, i.Item2));
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
        /// <typeparam name="TElem">The type of the comparable part of the element.</typeparam>
        public static TSource MinElem<TSource, TElem>(this IEnumerable<TSource> source, Func<TSource, TElem> selector)
            where TElem : IComparable
        {
            TSource src = default(TSource);
            TElem comp = default(TElem);
            TElem tmp;
            foreach (var i in source)
            {
                tmp = selector(i);
                if (comp.Equals(default(TElem)) || comp.CompareTo(tmp) > 0)
                {
                    src = i;
                    comp = tmp;
                }
            }
            return src;
        }

        /// <summary>
        /// Get the maximum element of a collection by a selector which selects the maximum element by choosing
        /// properties from this element.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="source">Source.</param>
        /// <param name="selector">Selector.</param>
        /// <typeparam name="TSource">The element type.</typeparam>
        /// <typeparam name="TElem">The type of the comparable part of the element.</typeparam>
        public static TSource MaxElem<TSource, TElem>(this IEnumerable<TSource> source, Func<TSource, TElem> selector)
            where TElem : IComparable
        {
            TSource src = default(TSource);
            TElem comp = default(TElem);
            TElem tmp;
            foreach (var i in source)
            {
                tmp = selector(i);
                if (comp.Equals(default(TElem)) || comp.CompareTo(tmp) < 0)
                {
                    src = i;
                    comp = tmp;
                }
            }
            return src;
        }

        /// <summary>
        /// Minimum element from enumerable.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="source">Source.</param>
        /// <param name="selector">Selector.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="TElem">The 3rd type parameter.</typeparam>
        public static Tuple<T1, T2> MinElem<T1, T2, TElem>(this IEnumerable<Tuple<T1, T2>> source,
            Func<T1, T2, TElem> selector) where TElem : IComparable
        {
            var t = source.MinElem(i => selector(i.Item1, i.Item2));
            return t ?? new Tuple<T1, T2>(default(T1), default(T2));
        }

        /// <summary>
        /// Maximum element from enumerable.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="source">Source.</param>
        /// <param name="selector">Selector.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="TElem">The 3rd type parameter.</typeparam>
        public static Tuple<T1, T2> MaxElem<T1, T2, TElem>(this IEnumerable<Tuple<T1, T2>> source,
            Func<T1, T2, TElem> selector) where TElem : IComparable
        {
            var t = source.MaxElem(i => selector(i.Item1, i.Item2));
            return t ?? new Tuple<T1, T2>(default(T1), default(T2));
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

        /// <summary>
        /// Format an enumerable as a human readable string.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="conversion">Conversion.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string Format<T>(this IEnumerable<T> source, Func<T, string> conversion)
        {
            string s = "[";
            foreach (var i in source)
                s += conversion(i) + ", ";
            return s.Substring(0, s.Length - 2) + "]";
        }

        /// <summary>
        /// Run an action after a specific time span.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="span">Span.</param>
        public static void RunAfter(this Action action, TimeSpan span)
        {
            var dispatcherTimer = new DispatcherTimer { Interval = span };
            dispatcherTimer.Tick += (sender, args) =>
                {
                    var timer = sender as DispatcherTimer;
                    if (timer != null)
                    {
                        timer.Stop();
                    }

                    action();
                };
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Converts FreezingArcher.Math.Vector3 to Jitter.JVector
        /// </summary>
        public static JVector ToJitterVector(this Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts Jitter.JVector to FreezingArcher.Math.Vector3
        /// </summary>
        public static Vector3 ToFreezingArcherVector(this JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts FreezingArcher.Math.Matrix to Jitter.Matrix
        /// </summary>
        /// <returns>The jitter matrix.</returns>
        /// <param name="matrix">Matrix.</param>
        public static JMatrix ToJitterMatrix(this Matrix matrix)
        {
            return new JMatrix(matrix.M11, matrix.M12, matrix.M13, 
                matrix.M21, matrix.M22, matrix.M23,
                matrix.M31, matrix.M32, matrix.M33);
        }

        /// <summary>
        /// Converts FreezingArcher.Math.Matrix3 to Jitter.Matrix
        /// </summary>
        /// <returns>The jitter matrix.</returns>
        /// <param name="matrix">Matrix.</param>
        public static JMatrix ToJitterMatrix(this Matrix3 matrix)
        {
            return new JMatrix(matrix.M11, matrix.M12, matrix.M13, 
                matrix.M21, matrix.M22, matrix.M23,
                matrix.M31, matrix.M32, matrix.M33);
        }

        /// <summary>
        /// Converts Jitter.JMatrix to FreezingArcher.Math.Matrix3
        /// </summary>
        /// <returns>The freezing archer matrix.</returns>
        /// <param name="matrix">Matrix.</param>
        public static Matrix3 ToFreezingArcherMatrix3(this JMatrix matrix)
        {
            return new Matrix3(matrix.M11, matrix.M12, matrix.M13, 
                matrix.M21, matrix.M22, matrix.M23,
                matrix.M31, matrix.M32, matrix.M33);
        }

        /// <summary>
        /// Converts Jitter.JMatrix to FreezingArcher.Math.Matrix
        /// </summary>
        /// <returns>The freezing archer matrix.</returns>
        /// <param name="matrix">Matrix.</param>
        public static Matrix ToFreezingArcherMatrix(this JMatrix matrix)
        {
            return new Matrix(matrix.M11, matrix.M12, matrix.M13, 0.0f,
                matrix.M21, matrix.M22, matrix.M23, 0.0f,
                matrix.M31, matrix.M32, matrix.M33, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Converts FreezingArcher.Math.Quaternion to Jitter.JQuaternion
        /// </summary>
        /// <returns>The jitter quaternion.</returns>
        /// <param name="quat">Quat.</param>
        public static JQuaternion ToJitterQuaternion(this Quaternion quat)
        {
            return new JQuaternion(quat.X, quat.Y, quat.Z, quat.W);
        }

        /// <summary>
        /// Converts Jitter.JQuaternion to FreezingArcher.Math.Quaternion
        /// </summary>
        /// <returns>The freezing archer quaternion.</returns>
        /// <param name="quat">Quat.</param>
        public static Quaternion ToFreezingArcherQuaternion(this JQuaternion quat)
        {
            return new Quaternion(quat.X, quat.Y, quat.Z, quat.W);
        }

        static readonly FastRandom rng = new FastRandom();  

        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
    }
}
