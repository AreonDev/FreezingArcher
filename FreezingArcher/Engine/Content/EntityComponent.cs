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
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Abstract entity component. Classes of this type are used to hold the data for entities. Those classes may hold
    /// only data (no methods).
    /// Inheritance of this class must be sealed, contain only properties and fields and all
    /// access modifiers must be internal or higher. If any of the constraints listed above is not met the build will
    /// fail on post processing.
    /// </summary>
    public abstract class EntityComponent : FAObject, IMessageCreator
    {
        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion

        public Entity Entity { get; private set; }

        /// <summary>
        /// Initialize this component. Within this method all properties may be reseted and reloaded from the attribute
        /// manager.
        /// </summary>
        /// <param name="entity">The entity this component is bounded to.</param>
        /// <param name="messageProvider">The message provider instance for the component.</param>
        public virtual void Init(Entity entity, MessageProvider messageProvider)
        {
            Entity = entity;
            messageProvider += this;
            // set type based default parameters for fields and properties
            Type t = GetType();
            var fields = t.GetFields().Where(f => !f.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
                !f.Name.StartsWith("Default", StringComparison.InvariantCulture)).ToList();
            var props = t.GetProperties().Where(p => p.CanWrite);

            foreach(var field in fields)
            {
                field.SetValue(this, t.GetField("Default" + field.Name).GetValue(this));
            }

            foreach (var prop in props)
            {
                if (prop.Name != "Entity")
                    prop.SetValue(this, t.GetField("Default" + prop.Name).GetValue(this));
            }

            // set blueprint based default parameters for fields and properties
            // TODO

            // set instance based default parameters for fields and properties
            // TODO
        }

        /// <summary>
        /// Switchs the message provider.
        /// </summary>
        /// <param name="providerFrom">Provider from.</param>
        /// <param name="providerTo">Provider to.</param>
        public void SwitchMessageProvider(MessageProvider providerFrom, MessageProvider providerTo)
        {
            providerFrom -= this;
            providerTo += this;
        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="msg">Message.</param>
        protected void CreateMessage(IMessage msg)
        {
            if (MessageCreated != null)
                MessageCreated(msg);
        }
    }
}
