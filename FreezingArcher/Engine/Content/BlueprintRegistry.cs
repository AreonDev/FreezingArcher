//
//  BlueprintRegistry.cs
//
//  Author:
//       wfailla <>
//
//  Copyright (c) 2015 wfailla
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
using System.Collections.Generic;
using FreezingArcher.Core;
using System.Xml.Linq;

namespace FreezingArcher.Content
{
    public class BlueprintRegistry
    {
        string Name;
        List<Pair<string, XElement>> Documents = new List<Pair<string, XElement>>();

        public BlueprintRegistry (string name)
        {
            Name = name;
        }

        public void AddDocument(string doc)
        {
            Documents.Add (new Pair<string, XElement>(doc, XElement.Load (doc)));
        }

        public object GetValueForComponent(string entityName, Type type, string fieldName)
        {
            //componenten name 
            string typename = type.GetFriendlyName();


            foreach (var x in Documents)
            {
                if (x.B.Attribute("name").Value == entityName)
                {
                    foreach(var y in x.B.Elements("component"))
                    {
                        if(y.Attribute("name").Value == typename)
                        {
                            foreach (var node in y.Elements())
                            {
                                //stack aufbauen von elements lege immer alle unterknoten drauf und arbeite dann den stack ab 
                                Type tmpType = Type.GetType("Namespace.MyClass, MyAssembly");
                                var obj = Activator.CreateInstance(tmpType);
                                foreach (var parameter in node.Elements())
                                {
                                    tmpType.GetField(parameter.Name).SetValue(obj, parameter.Value);
                                }
                                return obj;
                            }
                        }
                    }
                }
            }
        }

        public object GetValueForComponent<T>(string fieldName)
            where T : EntityComponent
        {
            this.GetValueForComponent (typeof(T), fieldName);
        }
    }
}

