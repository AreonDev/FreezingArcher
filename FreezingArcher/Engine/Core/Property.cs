//
//  Property.cs
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
using System;
using System.Linq;
using FreezingArcher.Output;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Property of types.
    /// Used for <see cref="DynamicClassBuilder"/> 
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Attribute[] Attributes{ get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Property"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        /// <param name="attribs">Attribs.</param>
        public Property (string name, Type type, params Attribute[] attribs)
        {
            this.Attributes = attribs;
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Property"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        /// <param name="attribs">Attribs.</param>
        public Property (string name, string type, params Attribute[] attribs)
        {
            this.Attributes = attribs;
            this.Name = name;
            var types = ReflectionHelper.GetTypesWhere (t => t.Name == type || t.FullName == type).ToList ();
            if (types.Count > 1 || types.Count == 0)
                throw new Exception ("Could not determine type, try to specify type explicitly");

            Type = types [0];
        }

        /// <summary>
        /// Read property specified by name from the specified type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The property type.</typeparam>
        public static T Read<T>(object type, string name)
        {
            try
            {
                return (T)type.GetType ().GetProperty (name).GetGetMethod ().Invoke (type, null);
            }
            catch(Exception e)
            {
                Logger.Log.AddLogEntry (LogLevel.Warning, "DynamicClass", "Error reading property: " + name + "\n" + e);
                return default(T);
            }
        }
    }
}
