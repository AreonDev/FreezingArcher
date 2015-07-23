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
using System;
using Jitter;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Physics applying enum.
    /// </summary>
    [Flags]
    public enum AffectedByPhysics : int
    {
        /// <summary>
        /// Physics does not affect data.
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// Physics affects position data in Transform Component
        /// </summary>
        Position = 1,
        /// <summary>
        /// Physics affects orientation data in Transform Component
        /// </summary>
        Orientation = 2
    }

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
        /// The default world.
        /// </summary>
        public static readonly World DefaultWorld = null;

        /// <summary>
        /// The default physics applying.
        /// </summary>
        public static readonly AffectedByPhysics DefaultPhysicsApplying = AffectedByPhysics.Nothing;

        /// <summary>
        /// Gets or sets the rigid body.
        /// </summary>
        /// <value>The rigid body.</value>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// Gets or sets the world.
        /// </summary>
        /// <value>The world.</value>
        public World World { get; set; }

        /// <summary>
        /// Gets or sets the physics applying.
        /// </summary>
        /// <value>The physics applying.</value>
        public AffectedByPhysics PhysicsApplying {get; set;}

        public override void Destroy()
        {
            World.RemoveBody(RigidBody);
            base.Destroy();
        }
    }
}
