//
//  PhysicsComponent.cs
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
using Jitter.Dynamics;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Physics component.
    /// </summary>
    public sealed class PhysicsComponent : EntityComponent
    {
        /// <summary>
        /// The default rigid body.
        /// </summary>
        public static readonly RigidBody DefaultRigidBody = null;

        /// <summary>
        /// Gets or sets the rigid body.
        /// </summary>
        /// <value>The rigid body.</value>
        public RigidBody RigidBody { get; set; }
    }
}
