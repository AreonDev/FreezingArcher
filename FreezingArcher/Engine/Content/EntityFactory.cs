//
//  EntityFactory.cs
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
using FreezingArcher.Core;
using FreezingArcher.Messaging;
using FreezingArcher.Content;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Entity factory. This class is used to create entities from given parameters or descriptive xml files.
    /// </summary>
    public class EntityFactory
    {
        /// <summary>
        /// The global instance of the entity factory.
        /// </summary>
        public static EntityFactory Instance;

        /// <summary>
        /// The name of the module.
        /// </summary>
        public static readonly string ModuleName = "EntityFactory";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.EntityFactory"/> class.
        /// </summary>
        /// <param name="objectManager">Object manager.</param>
        /// <param name="messageManager">Message manager.</param>
        public EntityFactory(ObjectManager objectManager, MessageManager messageManager)
        {
            ObjectManager = objectManager;
            MessageManager = messageManager;
        }

        readonly ObjectManager ObjectManager;
        readonly MessageManager MessageManager;

        /// <summary>
        /// Creates an entity with the specified parameters.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="name">Name.</param>
        /// <param name="components">Components.</param>
        /// <param name="systems">Systems.</param>
        public Entity CreateWith(string name, Type[] components = null, Type[] systems = null)
        {
            Entity e = ObjectManager.CreateOrRecycle<Entity>();
            e.Init(name, MessageManager);

            if (components != null)
            {
                foreach (var c in components)
                {
                    if (!e.AddComponent(c))
                    {
                        Logger.Log.AddLogEntry(LogLevel.Error, ModuleName, "Failed to add component {0}!", c.Name);
                    }
                }
            }

            if (systems != null)
            {
                foreach (var s in systems)
                {
                    if (!e.AddSystem(s))
                    {
                        Logger.Log.AddLogEntry(LogLevel.Error, ModuleName, "Failed to add system {0}!", s.Name);
                    }
                }
            }

            return e;
        }

        /// <summary>
        /// Creates entity from the given xml.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="pathToXML">Path to XML.</param>
        public Entity CreateFrom(string pathToXML)
        {
            throw new NotImplementedException();
        }
    }
}
