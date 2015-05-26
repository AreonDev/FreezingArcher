﻿//
//  EntitySystem.cs
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
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Abstract entity system. Classes of this type are used modify the data of entities stored in components.
    /// Those classes may hold be a message consumer only and may not hold any additional data or have any additional
    /// methods.
    /// Inheritance of this class must be sealed, only overriden fields and methods may be contained in this class.
    /// If any of the constraints listed above is not met the build will fail on post processing.
    /// </summary>
    public abstract class EntitySystem : FAObject, IMessageConsumer
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        public abstract void Init();

        #region IMessageConsumer implementation
        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public abstract void ConsumeMessage(IMessage msg);

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public abstract int[] ValidMessages { get; }
        #endregion
    }
}
