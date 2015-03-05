//
//  ReflectionHelper.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2015 martin
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using FreezingArcher.Output;


namespace FreezingArcher.Core
{
    /// <summary>
    /// Helper class for Reflections and Inheritance Trees
    /// </summary>
    public static class ReflectionHelper
    {
        static List<Type> loadedTypesPlugins = new List<Type>();
        static List<Type> loadedAssembly = new List<Type>();
        static object o = new object();
        const string mn = "ReflectionHelper";

        static ReflectionHelper()
        {
            Refresh0 ();
            AppDomain.CurrentDomain.AssemblyLoad += 
                (sender, args) => {
                lock(o)
                    loadedAssembly.AddRange(args.LoadedAssembly.GetTypes());
            };
        }

        private static void Refresh0()
        {
            var appDomain = AppDomain.CurrentDomain;
            var assemblies = appDomain.GetAssemblies ();
            loadedAssembly.Clear ();
            foreach (var asm in assemblies)
            {
                lock (o)
                {

                    loadedAssembly.AddRange (asm.GetTypes ());
                }
            }
            Logger.Log.AddLogEntry (LogLevel.Info, mn, "Loaded {0} types in main assembly", loadedAssembly.Count);
        }
        /// <summary>
        /// Releases all resource used by the <see cref="FreezingArcher.Core.ReflectionHelper"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="FreezingArcher.Core.ReflectionHelper"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="FreezingArcher.Core.ReflectionHelper"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="FreezingArcher.Core.ReflectionHelper"/> so the garbage collector can reclaim the memory that the
        /// <see cref="FreezingArcher.Core.ReflectionHelper"/> was occupying.</remarks>
        public static void Dispose()
        {
            lock (o)
            {
                loadedAssembly = null;
                loadedTypesPlugins = null;
            }
        }

        /// <summary>
        /// Gets the loaded types.
        /// </summary>
        /// <returns>The loaded types.</returns>
        public static IEnumerable<Type> getLoadedTypes()
        {
            lock (o)
            {
                return loadedAssembly.Concat(loadedTypesPlugins);
            }
        }

        /// <summary>
        /// Gets the derived types of t
        /// </summary>
        /// <returns>The derived types.</returns>
        /// <param name="t">Type to find derived types</param>
        public static IEnumerable<Type> GetDerivedTypes(Type t)
        {
            lock (o)
            {
                return loadedAssembly.Concat (loadedTypesPlugins).Where (t.IsAssignableFrom);
            }
        }

        /// <summary>
        /// Gets the types where the predicate matches
        /// </summary>
        /// <returns>The types where the predicate matches</returns>
        /// <param name="pred">Predicate to filter</param>
        public static IEnumerable<Type> GetTypesWhere(Predicate<Type> pred)
        {
            return loadedAssembly.Concat(loadedTypesPlugins).Where(item => pred(item));
        }
    }
}

