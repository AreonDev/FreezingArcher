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
    /// Abstract entity component. Classes of this type are used to hold the data for entities. Those classes may hold
    /// only data (no methods).
    /// Inheritance of this class must be sealed, contain only properties and fields and all
    /// access modifiers must be internal or higher. If any of the constraints listed above is not met the build will
    /// fail on post processing.
    /// </summary>
    public abstract class EntityComponent : FAObject
    {
        /// <summary>
        /// Initialize this component. Within this method all properties may be reseted and reloaded from the attribute
        /// manager.
        /// </summary>
        /// <param name="entity">The entity this component is bounded to.</param>
        public abstract void Init(Entity entity);
    }
}
