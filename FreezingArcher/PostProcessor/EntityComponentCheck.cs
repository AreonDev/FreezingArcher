//
//  EntityComponentCheck.cs
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
using System.Reflection;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PostProcessor
{
    public sealed class EntityComponentCheck : IPostProcessingStep
    {
        readonly Type baseCompnent;

        public EntityComponentCheck ()
        {
            var asm = Assembly.LoadFile (Path.Combine (MainClass.workingDirectory.FullName, "lib", "FreezingArcher.dll"));
            baseCompnent = asm.GetType ("FreezingArcher.Content.EntityComponent");
        }


        #region IPostProcessingStep implementation

        public int DoPostProcessing (Assembly asm)
        {
	    PostProcessingError error = PostProcessingError.NoError;
            var bc = asm.GetTypes ().Where (j => j.BaseType == baseCompnent);
            foreach (var item in bc)
            {
                Console.WriteLine ("Processing type: {0}", item.FullName);
                if (!item.IsSealed)
                {
                    Console.WriteLine ("Error: Type {0} inherits from EntityComponent but is not sealed!", item.FullName);
		    error |= PostProcessingError.NotSealed;
                }

                var methods = item.GetMethods (
                                  BindingFlags.NonPublic |
                                  BindingFlags.Public |
                                  BindingFlags.Static |
                                  BindingFlags.Instance |
                                  BindingFlags.DeclaredOnly);
                
		var methods2 = methods.Where (k => !k.IsSpecialName && k.GetBaseDefinition() == k).ToArray ();

                if (methods2.Length > 0)
                {
                    Console.WriteLine ("Error: Type {0} has {1} methods (listed below)", item.FullName, methods2.Length);
                    foreach (var method in methods2)
                    {
                        Console.WriteLine (getStringSignature (method));
                    }
		    error |= PostProcessingError.MethodError;
                }

		var fields = item.GetFields(
		    BindingFlags.NonPublic |
		    BindingFlags.Public |
		    BindingFlags.Static |
		    BindingFlags.Instance |
		    BindingFlags.DeclaredOnly).ToList();

		var fields2 = fields.Where(k => (k.IsPrivate || k.IsFamily || k.IsFamilyOrAssembly)
		    && !k.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
		    !k.Name.StartsWith("Default", StringComparison.InvariantCulture)).ToList();

		if (fields2.Count > 0)
		{
		    Console.WriteLine("Error: Type {0} has {1} fields with disallowed access modifiers", item.Name,
			fields2.Count);
		    foreach (var field in fields2)
		    {
			string access_modifier = null;

			if (field.IsPrivate)
			    access_modifier = "private";
			if (field.IsFamily)
			    access_modifier = "protected";
			if (field.IsFamilyOrAssembly)
			    access_modifier = "protected internal";

			Console.WriteLine("Error: Field {0} is {1} but should be at least internal!", field.Name,
			    access_modifier);
		    }
		    error |= PostProcessingError.FieldError;
		}

		var fields3 = fields.Where(k => (!k.IsPublic || !k.IsStatic || !k.IsInitOnly) &&
		    k.Name.StartsWith("Default", StringComparison.InvariantCulture)).ToList();

		if (fields3.Count > 0)
		{
		    Console.WriteLine("Error: {0} default fields have some invalid modifiers!", fields3.Count);
		    foreach (var field in fields3)
		    {
			string access_modifier = null;

			if (field.IsPrivate)
			    access_modifier = "private";
			if (field.IsFamily)
			    access_modifier = "protected";
			if (field.IsFamilyOrAssembly)
			    access_modifier = "protected internal";
			if (field.IsAssembly)
			    access_modifier = "internal";

			if (!field.IsPublic)
			{
			    Console.WriteLine("Error: Default field {0} is {1} but should be public!", field.Name,
				access_modifier);
			    error |= PostProcessingError.FieldError;
			}

			if (!field.IsStatic)
			{
			    Console.WriteLine("Error: Default field {0} is not static but should be!", field.Name);
			    error |= PostProcessingError.FieldError;
			}

			if (!field.IsInitOnly)
			{
			    Console.WriteLine("Error: Default field {0} is not readonly but should be!", field.Name);
			    error |= PostProcessingError.FieldError;
			}
		    }
		}

		foreach (var field in fields)
		{
		    if (!field.IsStatic && !field.IsDefined(typeof(CompilerGeneratedAttribute), false))
		    {
			if (fields.FindIndex(f => f.Name == "Default" + field.Name) < 0)
			{
			    Console.WriteLine("Error: Field {0} has no default value specified via a static field!",
				field.Name);
			    error |= PostProcessingError.FieldError;
			}
		    }
		}
                    
                var props = item.GetProperties (
                                BindingFlags.NonPublic |
                                BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.Instance |
                                BindingFlags.DeclaredOnly);
                foreach (var prop in props) 
                {
                    if(prop.CanRead)
                    {
                        if((int)getAmFromMethod (prop.GetGetMethod (true)) < 4)
                        {
                            Console.WriteLine ("Error: getter of property {0} is {1}, should be at least internal", prop.Name, getAmFromMethod (prop.GetGetMethod (true)).ToString ().Replace ('_', ' '));
			    error |= PostProcessingError.PropertyError;
                        }
                    }
                    if(prop.CanWrite)
                    {
                        if((int)getAmFromMethod (prop.GetSetMethod (true)) < 4)
                        {
                            Console.WriteLine ("Error: setter of property {0} is {1}, should be at least internal", prop.Name, getAmFromMethod (prop.GetSetMethod (true)).ToString ().Replace ('_', ' '));
			    error |= PostProcessingError.PropertyError;
                        }
                    }    

		    if (fields.FindIndex(f => f.Name == "Default" + prop.Name) < 0)
		    {
			Console.WriteLine("Error: Field {0} has no default value specified via a static field!",
			    prop.Name);
			error |= PostProcessingError.PropertyError;
		    }
                }

                var constr = item.GetConstructors (
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    //BindingFlags.Static | nonstatic only (exclude .cctor)
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly);
                foreach (var @const in constr) {
                    var am = getAmFromMethod (@const);
                    if((int)am < 4)
                    {
                        Console.WriteLine ("Error: constructor is {0}, should be at least internal (listed below)", am.ToString ().Replace ('_', ' '));
                        Console.WriteLine (getStringSignature (@const));
			error |= PostProcessingError.ConstructorError;
                    }
		    if (@const.GetParameters().Length > 0)
		    {
			Console.WriteLine("Error: constructor needs to be default constructor");
			error |= PostProcessingError.ConstructorError;
		    }
                }
            }   
	    return (int) error;
        }

        #endregion

        static AccessModifier getAmFromMethod (MethodBase mi)
        {
            if (mi.IsPublic)
                return AccessModifier.Public;
            if (mi.IsPrivate)
                return AccessModifier.Private;
            if (mi.IsFamilyOrAssembly)
                return AccessModifier.ProtectedInternal;
            if (mi.IsFamily)
                return AccessModifier.Protected;
            return AccessModifier.Internal;
        }

        static string getStringSignature (MethodBase mi)
        {
            var am = getAmFromMethod (mi);

            string res = am.ToString ().Replace ('_', ' ') + (mi.IsStatic ? " static " :" ");
            var methodInfo = mi as MethodInfo;
            if (methodInfo != null)
                res += methodInfo.ReturnType.Name + " ";
            
            res += ((mi.Name == ".ctor" || mi.Name == ".cctor") ? mi.DeclaringType.Name : mi.Name);
            res += " (";
            var @params = mi.GetParameters ();

            for (int i = 0; i < @params.Length; i++)
            {
                res += @params [i].ParameterType.Name + " " + @params [i].Name + (i < @params.Length - 1 ? ", " : "");
            }
            res += ");";
            return res;
        }
    }
}
