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
            uint lastIndex = 0;
            Dictionary<uint, FAObject> objects;
            readonly Queue<FAObject> objectsToRecylce;
            ObjectFactory factory;
            ObjectManager mgr;

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
        private Dictionary<int, ObjectTypeManager> objectTypes;
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.ObjectManager"/> class.
        /// </summary>
        public ObjectManager()
        {
            Logger.Log.AddLogEntry(LogLevel.Info, "ObjectManager", "Initializing object manager...");
            objectTypes = new Dictionary<int, ObjectTypeManager>();
        }

        internal void PrepareForRecycling(FAObject fAObject)
        {
            objectTypes[fAObject.TypeId].PrepareForRecycling(fAObject);
        }

        /// <summary>
        /// Creates a new object or recycles a destroyed object
        /// </summary>
        /// <returns>new or recycled object</returns>
        /// <typeparam name="T">Return Type</typeparam>
        public T CreateOrRecycle<T>() where T: FAObject, new()
        {            
            var hc = typeof(T).GetHashCode();
            ObjectTypeManager mgr;
            if(!objectTypes.TryGetValue(hc, out mgr))
            {
                Logger.Log.AddLogEntry(LogLevel.Fine, "ObjectManager", "Creating new factory for type {0}", typeof(T).FullName);
                objectTypes.Add(hc, mgr = new ObjectTypeManager(this, () => new T()));
            }
            return mgr.CreateNewOrRecycle() as T;
        }

        /// <summary>
        /// Creates a new object or recycles a destroyed object
        /// </summary>
        /// <returns>new or recycled object</returns>
        /// <param name="type">Type of object to recycle</param>
        /// <typeparam name="T">Return Type</typeparam>
        public T CreateOrRecycle<T>(Type type) where T: FAObject, new()
        {
            var hc = type.GetHashCode();
            ObjectTypeManager mgr;
            if(!objectTypes.TryGetValue(hc, out mgr))
            {
                Logger.Log.AddLogEntry(LogLevel.Fine, "ObjectManager", "Creating new factory for type {0}", typeof(T).FullName);
                objectTypes.Add(hc, mgr = new ObjectTypeManager(this, () => new T()));
            }
            return mgr.CreateNewOrRecycle() as T;
        }

        /// <summary>
        /// Creates a new object or recycles a destroyed object
        /// </summary>
        /// <returns>new or recycled object</returns>
        /// <param name="hc">HashCode of the type of the object</param>
        /// <typeparam name="T">Return Type</typeparam>
        public T CreateOrRecycle<T>(int hc) where T : FAObject, new()
        {
            ObjectTypeManager mgr;
            if(!objectTypes.TryGetValue(hc, out mgr))
            {
                Logger.Log.AddLogEntry(LogLevel.Fine, "ObjectManager", "Creating new factory for type {0}", typeof(T).FullName);
                objectTypes.Add(hc, mgr = new ObjectTypeManager(this, () => new T()));
            }
            return mgr.CreateNewOrRecycle() as T;
        }
    }
}

