//
//  PhysicsManager.cs
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
using Jitter;
using Jitter.Collision;

namespace FreezingArcher.Core
{
    public enum CollisionSystem
    {
        SweepAndPrune,
        PersistentSweepAndPrune,
        Brute
    }

    public sealed class PhysicsManager
    {
        public PhysicsManager(CollisionSystem collisionSystem = CollisionSystem.SweepAndPrune)
        {
            this.collisionSystem = collisionSystem;

            Jitter.Collision.CollisionSystem system;

            switch (collisionSystem)
            {
                case CollisionSystem.Brute:
                    system = new CollisionSystemBrute();
                    break;
                case CollisionSystem.PersistentSweepAndPrune:
                    system = new CollisionSystemPersistentSAP();
                    break;
                default:
                    system = new CollisionSystemSAP();
                    break;
            }

            World = new World(system);
        }

        CollisionSystem collisionSystem;

        public CollisionSystem CollisionSystem
        {
            get
            {
                return collisionSystem;
            }
            set
            {
                switch (value)
                {
                    case CollisionSystem.Brute:
                        World.CollisionSystem = new CollisionSystemBrute();
                        collisionSystem = CollisionSystem.Brute;
                        break;
                    case CollisionSystem.PersistentSweepAndPrune:
                        World.CollisionSystem = new CollisionSystemPersistentSAP();
                        collisionSystem = CollisionSystem.PersistentSweepAndPrune;
                        break;
                    default:
                        World.CollisionSystem = new CollisionSystemSAP();
                        collisionSystem = CollisionSystem.SweepAndPrune;
                        break;
                }
            }
        }

        public World World { get; private set; }

        public void Update(TimeSpan timeStamp)
        {
            World.Step((float) timeStamp.TotalSeconds, true);
        }

        public void Destroy ()
        {
            World.Clear();
        }
    }
}

