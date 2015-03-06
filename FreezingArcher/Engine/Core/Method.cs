//
//  Method.cs
//
//  Author:
//       martin <>
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

namespace FreezingArcher.Core
{
    /// <summary>
    /// Method for dynamicClassBuilder
    /// </summary>
    public class Method
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set;}
        /// <summary>
        /// Gets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Delegate Implementation { get; private set;}
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Attribute[] Attributes {get;private set;}
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Method"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="implementation">Implementation.</param>
        /// <param name="attributes">Attributes.</param>
        public Method (string name, Delegate implementation, params Attribute[] attributes)
        {
            Name = name;
            Implementation = implementation;
            Attributes = attributes;
        }
    }
}

