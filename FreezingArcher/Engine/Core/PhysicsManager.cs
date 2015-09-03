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
using System.Threading;
using FreezingArcher.Output;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Core
{
    /// <summary>
    /// This enum describes the collision system used by the physics engine.
    /// </summary>
    public enum CollisionSystem
    {
        /// <summary>
        /// The sweep and prune collision system.
        /// </summary>
        SweepAndPrune,
        /// <summary>
        /// The persistent sweep and prune collision system.
        /// </summary>
        PersistentSweepAndPrune,
        /// <summary>
        /// The brute collision system.
        /// </summary>
        Brute
    }

    /// <summary>
    /// This class manages a physics instance of one scene.
    /// </summary>
    public sealed class PhysicsManager : IMessageCreator
    {
        readonly MessageProvider MessageProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.PhysicsManager"/> class.
        /// </summary>
        /// <param name="collisionSystem">Collision system.</param>
        public PhysicsManager(MessageProvider prov, CollisionSystem collisionSystem = CollisionSystem.SweepAndPrune)
        {
            this.collisionSystem = collisionSystem;

            MessageProvider = prov;

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

            MessageProvider += this;

            World = new World(system);

            World.CollisionSystem.CollisionDetected += (Jitter.Dynamics.RigidBody body1, Jitter.Dynamics.RigidBody body2,
                Jitter.LinearMath.JVector point1, Jitter.LinearMath.JVector point2, Jitter.LinearMath.JVector normal, float penetration) => 
            {
                if(MessageCreated != null)
                    MessageCreated(new CollisionDetectedMessage(body1, body2));
            };

            Start();
        }

        CollisionSystem collisionSystem;

        /// <summary>
        /// Gets or sets the collision system.
        /// </summary>
        /// <value>The collision system.</value>
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

        Thread physicsThread;
        readonly AutoResetEvent updateEvent = new AutoResetEvent(false);
        TimeSpan timeStamp;
        bool running;
        bool physicsUpdateRunning = false;

        void run()
        {
            running = true;
            while(running)
            {
                updateEvent.WaitOne();
                physicsUpdateRunning = true;
                World.Step((float) timeStamp.TotalSeconds, true);
                physicsUpdateRunning = false;
            }
        }

        public void Start()
        {
            physicsThread = new Thread(run);
            physicsThread.Start();
        }

        public void Stop ()
        {
            running = false;
        }

        /// <summary>
        /// Gets the physics world.
        /// </summary>
        /// <value>The world.</value>
        public World World { get; private set; }

        /// <summary>
        /// Update physics with the specified time stamp.
        /// </summary>
        /// <param name="timeStamp">Time stamp.</param>
        public void Update(TimeSpan timeStamp)
        {
            if (physicsUpdateRunning)
            {
                Logger.Log.AddLogEntry(LogLevel.Warning, "PhysicsManager",
                    "A physics update is already running - ignoring update request...\n" +
                    "A possible reason could be too many objects in the scene, the update cycle is too fast or your " +
                    "computer is too slow."
                );   
            }
            else
            {
                try
                {
                this.timeStamp = timeStamp;
                updateEvent.Set();
                }catch {
                }
            }
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy ()
        {
            running = false;
            World.Clear();

            updateEvent.Set();

            updateEvent.Close();
            updateEvent.Dispose();
            physicsThread.Abort();
        }

        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion
    }
}
