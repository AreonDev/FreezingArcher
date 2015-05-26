//
//  TypeHelper.cs
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

namespace PostProcessor
{
    public enum AccessModifier
    {
	Private = 1,
	Protected = 2,
	ProtectedInternal = 3,
	Internal = 4,
	Public = 5,
    }

    [Flags]
    public enum PostProcessingError
    {
	NoError = 0,
	NotSealed = 1,
	MethodError = 2,
	FieldError = 4,
	PropertyError = 8,
	ConstructorError = 16
    }

    public static class TypeHelper
    {
	/// <summary>
	/// When overridden in a derived class, returns the <see cref="propertyInfo"/> object for the 
	/// method on the direct or indirect base class in which the property represented 
	/// by this instance was first declared. 
	/// </summary>
	/// <returns>A object for the first implementation of this property.</returns>
	public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo)
	{
	    var method = propertyInfo.GetAccessors(true)[0];
	    if (method == null)
		return null;

	    var baseMethod = method.GetBaseDefinition();

	    if (baseMethod == method)
		return propertyInfo;

	    var allProperties = BindingFlags.Instance | BindingFlags.Public 
		| BindingFlags.NonPublic | BindingFlags.Static;

	    var arguments = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();

	    return baseMethod.DeclaringType.GetProperty(propertyInfo.Name, allProperties, 
		null, propertyInfo.PropertyType, arguments, null);
	}
    }
}