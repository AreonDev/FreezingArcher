//
//  ComponentRegistry.cs
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
using System.Collections.Generic;
using FreezingArcher.Core;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Component registry. This class holds and serves entity components. All registered components can be used in an
    /// entity.
    /// </summary>
    public sealed class ComponentRegistry
    {
        /// <summary>
        /// The name of this module.
        /// </summary>
        public static readonly string ModuleName = "ComponentRegistry";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.ComponentRegistry"/> class.
        /// </summary>
        public ComponentRegistry(ObjectManager objectManager)
        {
            m_Registry = new Dictionary<int, Type>();
            this.objectManager = objectManager;
        }

        readonly Dictionary<int, Type> m_Registry;

        readonly ObjectManager objectManager;

        /// <summary>
        /// Register the specified entity component type in the component registry for further instantiation in
        /// entities. This method returns status codes, where Status.Success is given on success and Status.Rejected is
        /// given if the component type is already registered in the component registry.
        /// </summary>
        /// <typeparam name="T">The type of the EntityComponent to register.</typeparam>
        public Status Register<T>() where T : EntityComponent
        {
            return Register(typeof(T));
        }

        /// <summary>
        /// Register the specified entity component type in the component registry for further instantiation in
        /// entities. This method returns status codes, where Status.Success is given on success,
        /// Status.YouShallNotPassNull is given if the type is null and Status.Rejected is given if the component type
        /// is already registered in the component registry.
        /// </summary>
        /// <param name="type">The type of an EntityComponent.</param>
        public Status Register(Type type)
        {
            if (type == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName, Status.YouShallNotPassNull,
                    "You shall not pass null as type parameter when registering a new entity component!");
                return Status.YouShallNotPassNull;
            }

            try
            {
                m_Registry.Add(type.GetHashCode(), type);
            }
            catch
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName, Status.Rejected,
                    "Type {0} already registered in ComponentRegistry!", type.GetFriendlyName());
                return Status.Rejected;
            }

            return Status.Success;
        }

        /// <summary>
        /// Unregister the specified entity component type from this component registry. This method return a status
        /// code where Status.Removed indicates a successful remove of the given component and Status.FailedToRemove
        /// indicates a failure on removing the given component.
        /// </summary>
        /// <typeparam name="T">The type of the entity component to remove.</typeparam>
        public Status Unregister<T>() where T : EntityComponent
        {
            return Unregister(typeof(T));
        }

        /// <summary>
        /// Unregister the specified entity component type from this component registry. This method return a status
        /// code where Status.Removed indicates a successful remove of the given component, Status.FailedToRemove
        /// indicates a failure on removing the given component and Status.YouShallNotPassNull is given if the type
        /// parameter is null.
        /// </summary>
        /// <param name="type">The type of the entity component to remove.</param>
        public Status Unregister(Type type)
        {
            if (type == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName, Status.YouShallNotPassNull,
                    "You shall not pass null as type parameter when unregistering an entity component!");
                return Status.YouShallNotPassNull;
            }

            return m_Registry.Remove(type.GetHashCode()) ? Status.Removed : Status.FailedToRemove;
        }

        /// <summary>
        /// Instantiate a given entity component type for use in an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity component to instantiate.</typeparam>
        public T Instantiate<T>() where T : EntityComponent
        {
            return Instantiate(typeof(T)) as T;
        }

        /// <summary>
        /// Instantiate a given entity component type for use in an entity.
        /// </summary>
        /// <param name="type">The type of the entity component to instantiate.</param>
        public EntityComponent Instantiate(Type type)
        {
            return objectManager.CreateOrRecycle(type) as EntityComponent;
        }

        /// <summary>
        /// Determines whether the given entity component type is registered or not.
        /// </summary>
        /// <returns><c>true</c> if the given entity component type is registered; otherwise, <c>false</c>.</returns>
        /// <typeparam name="T">The type of the entity component to check for.</typeparam>
        public bool IsRegistered<T>() where T : EntityComponent
        {
            return IsRegistered(typeof(T));
        }

        /// <summary>
        /// Determines whether the given entity component type is registered or not.
        /// </summary>
        /// <returns><c>true</c> if the given entity component type is registered; otherwise, <c>false</c>.</returns>
        /// <param name="type">The type of the entity component to check for.</param>
        public bool IsRegistered(Type type)
        {
            return m_Registry.ContainsKey(type.GetHashCode());
        }
    }
}
