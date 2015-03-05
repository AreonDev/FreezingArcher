//
//  DynamicClassBuilder.cs
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
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Helper class for building dynamic classes
    /// <remarks>Default values for properties are not supported.</remarks>
    /// </summary>
    public class DynamicClassBuilder
    {
        private readonly string className;
        private readonly List<Property> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.DynamicClassBuilder"/> class.
        /// </summary>
        /// <param name="className">Class name to generate</param>
        public DynamicClassBuilder (string className)
        {
            this.className = className;
            properties = new List<Property> ();
        }

        /// <summary>
        /// Adds the property to the current class
        /// </summary>
        /// <param name="p">Property to add</param>
        public void AddProperty (Property p)
        {
            properties.Add (p);
        }
        /// <summary>
        /// Removes the property from the current class
        /// </summary>
        /// <param name="p">property</param>
        public void RemoveProperty(Property p)
        {
            properties.Remove (p);
        }

        /// <summary>
        /// Creates the type.
        /// </summary>
        /// <returns>The type.</returns>
        public Type CreateType ()
        {
            AssemblyName asmName = new AssemblyName (Guid.NewGuid ().ToString ());
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule (className);
            TypeBuilder tb = modBuilder.DefineType (className);


            foreach (var property in properties)
            {
                FieldBuilder fb = tb.DefineField ("_" + property.Name, property.Type, FieldAttributes.Private);

                PropertyBuilder propertyBuilder = tb.DefineProperty (property.Name, PropertyAttributes.HasDefault, property.Type, null);
                //fb.SetConstant (property.DefaultValue);
                //doesnt work with mono - should be fixed sometimes

                foreach (var attrib in property.Attributes)
                    propertyBuilder.SetCustomAttribute (attrib.GetBuilder ());
                //Get Method
                MethodBuilder getPropMthdBldr = tb.DefineMethod ("get_" + property.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, property.Type, Type.EmptyTypes);
                ILGenerator getIl = getPropMthdBldr.GetILGenerator ();
                getIl.Emit (OpCodes.Ldarg_0);
                getIl.Emit (OpCodes.Ldfld, fb);
                getIl.Emit (OpCodes.Ret);

                //End Get Method

                MethodBuilder setPropMthdBldr = tb.DefineMethod ("set_" + property.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { property.Type });
                ILGenerator setIl = setPropMthdBldr.GetILGenerator ();
                Label modifyProperty = setIl.DefineLabel ();
                Label exitSet = setIl.DefineLabel ();

                setIl.MarkLabel (modifyProperty);
                setIl.Emit (OpCodes.Ldarg_0);
                setIl.Emit (OpCodes.Ldarg_1);
                setIl.Emit (OpCodes.Stfld, fb);

                setIl.Emit (OpCodes.Nop);
                setIl.MarkLabel (exitSet);
                setIl.Emit (OpCodes.Ret);
                propertyBuilder.SetGetMethod (getPropMthdBldr);
                propertyBuilder.SetSetMethod (setPropMthdBldr);
            }

            return tb.CreateType ();
        }       
    }
}
