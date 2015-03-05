//
//  Attribute.cs
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
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Class for attributes on properties or types
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type {get; private set;}
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public Dictionary<string, object> Parameters { get; private set; }
        private object[] constrParams;
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Attribute"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="parameters">Parameters.</param>
        public Attribute(string name, params Pair<string, object>[] parameters)
        {
            var types = ReflectionHelper.GetTypesWhere(type => type.Name == name || type.FullName == name).ToList();
            if(types.Count > 1 || types.Count == 0)
                throw new Exception("Could not determine type, try to specify type explicitly");

            Type = types[0];
            Parameters = new Dictionary<string, object>(parameters.Length);
            foreach (var item in parameters) {
                Parameters.Add(item.A, item.B);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Attribute"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        public Attribute(Type type)
        {
            Type = type;
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Calls the constructor of the attribute
        /// Will search for a matching overload
        /// </summary>
        /// <param name="parameter">Parameters for the constructor</param>
        public void CallConstructor(params object[] parameter)
        {
            constrParams = parameter;
        }
        /// <summary>
        /// Adds the named parameters.
        /// </summary>
        /// <param name="named">Named parameters</param>
        public void AddNamedParameters(params Pair<string, object>[] named)
        {
            foreach (var item in named)
            {
                Parameters.Add (item.A, item.B);
            }
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <returns>The builder.</returns>
        public CustomAttributeBuilder GetBuilder()
        {
            var constructors = Type.GetConstructors();
            // Analysis disable once UseMethodIsInstanceOfType
            var constructor = constructors.First (constr =>
            { 
                int paramIndex = 0; 
                return constr.GetParameters().Length == constrParams.Length && constr.GetParameters ().All (param => param.ParameterType.IsAssignableFrom (constrParams [paramIndex++].GetType ()));
            });

            List<PropertyInfo> properties = new List<PropertyInfo> ();
            List<object> propertyValues = new List<object> ();
            List<FieldInfo> fields = new List<FieldInfo> ();
            List<object> fieldValues = new List<object> ();

            foreach (var item in Parameters)
            {
                var property = Type.GetProperty (item.Key);
                // Analysis disable once UseMethodIsInstanceOfType
                if(property != null && property.PropertyType.IsAssignableFrom(item.Value.GetType()))
                {
                    properties.Add (property);
                    propertyValues.Add (item.Value);
                    continue;
                }

                var field = Type.GetField (item.Key);
                // Analysis disable once UseMethodIsInstanceOfType
                if(field != null && field.FieldType.IsAssignableFrom(item.Value.GetType()))
                {
                    fields.Add (field);
                    fieldValues.Add (item.Value);
                }
            }
            var cab = new CustomAttributeBuilder (constructor, constrParams, properties.ToArray(), propertyValues.ToArray(), fields.ToArray(), fieldValues.ToArray());
            return cab;
        }
    }
}

