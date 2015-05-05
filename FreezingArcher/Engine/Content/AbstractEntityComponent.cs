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
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Abstract entity component.
    /// </summary>
    public abstract class AbstractEntityComponent : FAObject, IMessageConsumer
    {
        /// <summary>
        /// This field may contain a list of type id's describing the components this
        /// component depends on. The components type id may be received with
        /// Component.GetType().GetHashCode()
        /// </summary>
        public readonly int[] DependingOn = {};

        public abstract void ConsumeMessage(IMessage msg);

        public abstract int[] ValidMessages { get; }
    }
}
