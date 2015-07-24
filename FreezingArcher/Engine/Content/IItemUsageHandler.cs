//
//  ItemUsageHandler.cs
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
using Jitter.Dynamics;
using FreezingArcher.Math;

namespace FreezingArcher.Content
{
    /// <summary>
    /// This interface contains item usage handlers for an ItemSystem.
    /// </summary>
    public interface IItemUsageHandler
    {
        /// <summary>
        /// Eat the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        void Eat(ItemComponent item);

        /// <summary>
        /// Throw the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        void Throw(ItemComponent item);

        /// <summary>
        /// Hit the specified item, rigidBody, normal and fraction.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="rigidBody">Rigid body.</param>
        /// <param name="normal">Normal.</param>
        /// <param name="fraction">Fraction.</param>
        void Hit(ItemComponent item, RigidBody rigidBody, Vector3 normal, float fraction);

        /// <summary>
        /// Determines whether this instance is hit the specified rigidBody normal fraction.
        /// </summary>
        /// <returns><c>true</c> if this instance is hit the specified rigidBody normal fraction; otherwise, <c>false</c>.</returns>
        /// <param name="rigidBody">Rigid body.</param>
        /// <param name="normal">Normal.</param>
        /// <param name="fraction">Fraction.</param>
        bool IsHit(RigidBody rigidBody, Vector3 normal, float fraction);
    }
}
