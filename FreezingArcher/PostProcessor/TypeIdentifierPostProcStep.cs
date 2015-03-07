//
//  TypeIdentifierPostProcStep.cs
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
using System.Collections.Generic;

namespace PostProcessor
{
    public class TypeIdentifierPostProcStep : IPostProcessingStep
    {
        private readonly List<ushort> knownIdentifiers = new List<ushort>();
        private Type faObjectType, attribType;
        public TypeIdentifierPostProcStep (string directory)
        {
            Assembly asm = Assembly.LoadFrom (Path.Combine (directory, "FreezingArcher.dll"));
            faObjectType = asm.GetType ("FreezingArcher.Core.FAObject");
            attribType = asm.GetType ("FreezingArcher.Core.TypeIdentifierAttribute");
        }


        #region IPostProcessingStep implementation
        public int DoPostProcessing (Assembly asm)
        {
            var types = asm.GetTypes ();
            foreach (var type in types)
            {
                if(type.IsClass)
                {
                    var attrib = type.GetCustomAttribute (attribType, false);
                    if(attrib != null)
                    {
                        if(type.IsAbstract || !faObjectType.IsAssignableFrom(type))
                        {
                            Console.WriteLine ("Error: Type {0} has TypeIdentifier and is abstract or no FAObject", type.Name);
                            return 4;
                        }

                        Console.WriteLine ("--> Processing FAObject Type {0}", type.FullName);

                        var id = (ushort)attribType.GetProperty ("TypeID").GetValue (attrib);
                        if (knownIdentifiers.Contains (id))
                        {
                            Console.WriteLine ("Error: Type {0} has a duplicate TypeIdentifier of {1}", type.Name, id);
                            return 3;
                        }
                        knownIdentifiers.Add (id);
                    }
                    else if(faObjectType.IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        Console.WriteLine ("Error: Type {0} has no TypeIdentifier", type);
                        return 2;
                    }
                }
            }
            return 0;
        }
        #endregion
    }
}

