//
//  Entity.cs
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
using FreezingArcher.Core;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Messaging;
using System.Collections.Generic;
using FreezingArcher.Output;
using System;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Component based Entity.
    /// </summary>
    public sealed class Entity : FAObject, IManageable
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        public static readonly string ModuleName = "Entity";

        /// <summary>
        /// Initialize this entity with the specified name and message manager.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="messageManager">Message manager.</param>
        public void Init(string name, MessageManager messageManager)
        {
            Name = name;
            MessageManager = messageManager;

            if (Components == null)
                Components = new Dictionary<int, EntityComponent>();
            else
                Components.Clear();

            if (Systems == null)
                Systems = new Dictionary<int, EntitySystem>();
            else
                Systems.Clear();
        }

        /// <summary>
        /// The component storage.
        /// </summary>
        Dictionary<int, EntityComponent> Components;

        /// <summary>
        /// The system storage.
        /// </summary>
        Dictionary<int, EntitySystem> Systems;

        /// <summary>
        /// Gets the component by generic parameter.
        /// </summary>
        /// <returns>The component.</returns>
        /// <typeparam name="T">The type of the component you would like to have.</typeparam>
        public T GetComponent<T> () where T : EntityComponent
        {
            EntityComponent component;
            Components.TryGetValue (typeof(T).GetHashCode(), out component);
            return component as T;
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <returns><c>true</c>, if component was added, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">The type of the component.</typeparam>
        public bool AddComponent<T>() where T : EntityComponent
        {
            return AddComponent(typeof(T));
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <returns><c>true</c>, if component was added, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool AddComponent(Type type)
        {
            if (!typeof(EntityComponent).IsAssignableFrom(type))
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName,
                    "The specified component '{0}' is not of type EntityComponent!", type.Name);
                return false;
            }

            int typeid = type.GetHashCode();

            if (Components.ContainsKey (typeid))
            {
                return true;
            }

            var component = ObjectManager.CreateOrRecycle(type) as EntityComponent;
            component.Init(this, MessageManager);

            Components.Add(typeid, component);

            return true;
        }

        /// <summary>
        /// Removes the component.
        /// </summary>
        /// <returns><c>true</c>, if component was removed, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">The type of the component which should be removed.</typeparam>
        public bool RemoveComponent<T> () where T : EntityComponent
        {
            int typeid = typeof(T).GetHashCode();

            if (!Components.ContainsKey (typeid))
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName,
                    "Component {0} is not registered in this entity and cannot be removed!", typeof(T).Name);
                return false;
            }

            return Components.Remove(typeid);
        }

        /// <summary>
        /// Adds an entity system to this entity identified by the type parameter.
        /// </summary>
        /// <returns><c>true</c>, if system was added, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">The type of the system to add.</typeparam>
        public bool AddSystem<T> () where T : EntitySystem
        {
            return AddSystem(typeof(T));
        }

        /// <summary>
        /// Adds an entity system to this entity identified by the type parameter.
        /// </summary>
        /// <returns><c>true</c>, if system was added, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool AddSystem(Type type)
        {
            if (!typeof(EntitySystem).IsAssignableFrom(type))
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName,
                    "The specified system '{0}' is not of type EntitySystem!", type.Name);
                return false;
            }

            int typeid = type.GetHashCode();

            if (Systems.ContainsKey(typeid))
            {
                return true;
            }

            var system = ObjectManager.CreateOrRecycle(type) as EntitySystem;
            system.Init(MessageManager, this);

            Systems.Add(typeid, system);

            bool result = true;
            if (system.NeededComponents != null)
            {
                foreach (var comp in system.NeededComponents)
                {
                    if (!AddComponent(comp))
                    {
                        result = false;
                        Logger.Log.AddLogEntry(LogLevel.Error, ModuleName, "Failed to add component {0}!", comp.Name);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Removes a registered entity system from this entity identified by the type parameter.
        /// </summary>
        /// <returns><c>true</c>, if system was removed, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">The type of the system to remove.</typeparam>
        public bool RemoveSystem<T> () where T : EntitySystem
        {
            int typeid = typeof(T).GetHashCode();

            if (!Systems.ContainsKey(typeid))
            {
                Logger.Log.AddLogEntry(LogLevel.Error, ModuleName,
                    "System {0} is not registered in this entity and cannot be removed!", typeof(T).Name);
                return false;
            }

            return Systems.Remove(typeid);
        }

        /// <summary>
        /// The message manager.
        /// </summary>
        MessageManager MessageManager;

        #region IManageable implementation
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public override void Destroy()
        {
            Name = null;
            MessageManager = null;
            Components.Clear();
            base.Destroy();
        }
    }
}

