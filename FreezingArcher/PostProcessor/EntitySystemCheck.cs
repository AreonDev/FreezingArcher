//
//  EntitySystemCheck.cs
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
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace PostProcessor
{
    public class EntitySystemCheck : IPostProcessingStep
    {
	readonly Type baseCompnent;

	public EntitySystemCheck ()
	{
	    var asm = Assembly.LoadFile (Path.Combine (MainClass.workingDirectory.FullName, "lib", "FreezingArcher.dll"));
	    baseCompnent = asm.GetType ("FreezingArcher.Content.EntitySystem");
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
		    Console.WriteLine ("Error: Type {0} inherits from EntitySystem but is not sealed!", item.FullName);
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
		    BindingFlags.DeclaredOnly);

		var fields2 = fields.Where(k => !k.IsDefined(typeof(CompilerGeneratedAttribute), false)).ToArray();

		if (fields2.Length > 0)
		{
		    Console.WriteLine("Error: Type {0} has {1} fields which are disallowed!", item.Name,
			fields2.Length);
		    foreach (var field in fields2)
		    {
			Console.WriteLine("Error: Field {0} should not exist!", field.Name);
		    }
		    error |= PostProcessingError.FieldError;
		}

		var props = item.GetProperties (
		    BindingFlags.NonPublic |
		    BindingFlags.Public |
		    BindingFlags.Static |
		    BindingFlags.Instance |
		    BindingFlags.DeclaredOnly);

		var props2 = props.Where(k => k.GetBaseDefinition() == k).ToArray();

		Console.WriteLine("Error: Type {0} has {1} properties which are disallowed!", item.Name, props2.Length);

		foreach (var prop in props2) 
		{
		    Console.WriteLine ("Error: Property {0} should not exist!", prop.Name);
		    error |= PostProcessingError.PropertyError;
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
