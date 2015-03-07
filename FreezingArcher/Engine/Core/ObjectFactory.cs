//
//  ObjectFactory.cs
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
using System.Collections.Generic;
using FreezingArcher.Output;
using FreezingArcher.Reflection;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Class which automates object creation and recycling
    /// </summary>
    public class ObjectManager
    {
        /// <summary>
        /// Object factory deleage for FAObjects
        /// </summary>
        public delegate FAObject ObjectFactory();
        private class ObjectTypeManager
        {
            private uint lastIndex = 0;
            private Dictionary<uint, FAObject> objects;
            private Queue<FAObject> objectsToRecylce;
            private ObjectFactory factory;
            private ObjectManager mgr;
            public ObjectTypeManager(ObjectManager mgr, ObjectFactory factory, int objectCount = 1000)
            {
                this.mgr = mgr;
                this.factory = factory;
                this.objectsToRecylce = new Queue<FAObject>();
                this.objects = new Dictionary<uint, FAObject>(objectCount);
            }
            object locker = new object();
            public FAObject CreateNewOrRecycle()
            {
                if (objectsToRecylce.Count == 0)
                {
                    // create new
                    var fAObject = factory();
                    lock (locker)
                    {
                        uint index = this.lastIndex++;
                        fAObject.Init(this.mgr, index);
                        objects[index] = fAObject;
                    }
                    return fAObject;
                }
                else
                {
                    FAObject obj;
                    lock (objectsToRecylce)
                        obj = objectsToRecylce.Dequeue();
                    obj.Recycle();
                    return obj;
                }
            }

            internal void PrepareForRecycling(FAObject fAObject)
            {
                lock (objectsToRecylce)
                    objectsToRecylce.Enqueue(fAObject);
            }
        }
        private Dictionary<ushort, ObjectTypeManager> objectTypes;
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.ObjectManager"/> class.
        /// </summary>
        public ObjectManager()
        {
            Logger.Log.AddLogEntry(LogLevel.Info, "ObjectManager", "Initializing object manager...");
            objectTypes = new Dictionary<ushort, ObjectTypeManager>();
            var derived = ReflectionHelper.GetDerivedTypes(typeof(FAObject));
            foreach (var type in derived)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                TypeIdentifierAttribute tia = type.GetAttribute<TypeIdentifierAttribute>(false);
                objectTypes.Add(tia.TypeID, new ObjectTypeManager(this, () => Activator.CreateInstance(type) as FAObject));
            }
        }

        internal void PrepareForRecycling(FAObject fAObject)
        {
            objectTypes[(ushort)((fAObject.ID & 0xFFFF000000000000) << 48)].PrepareForRecycling(fAObject);
        }

        /// <summary>
        /// Creates a new object or recycles a destroyed object
        /// </summary>
        /// <returns>new or recycled object</returns>
        /// <param name="typeId">TypeId of the object to recycle</param>
        /// <typeparam name="T">Return Type</typeparam>
        public T CreateOrRecycle<T>(ushort typeId) where T: FAObject
        {
            return objectTypes[typeId].CreateNewOrRecycle() as T;
        }

        /// <summary>
        /// Creates a new object or recycles a destroyed object
        /// </summary>
        /// <returns>new or recycled object</returns>
        /// <param name="type">Type of object to recycle</param>
        /// <typeparam name="T">Return Type</typeparam>
        public T CreateOrRecycle<T>(Type type) where T: FAObject
        {
            return objectTypes[type.GetAttribute<TypeIdentifierAttribute>(false).TypeID].CreateNewOrRecycle() as T;
        }

    }
}

