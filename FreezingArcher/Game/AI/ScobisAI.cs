//
//  ScobisAI.cs
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
using FreezingArcher.Content;
using System.Collections.Generic;
using FreezingArcher.Math;
using FreezingArcher.Game.Maze;
using FreezingArcher.Output;
using Jitter.Dynamics;
using FreezingArcher.Core;
using Jitter.LinearMath;

namespace FreezingArcher.Game.AI
{
    public sealed class ScobisAI : ArtificialIntelligence
    {
        public ScobisAI ()
        {
        }

        const float acceleration = 0.02f;

        const float speed = 3f;

        JVector direction;

        JVector fallback;

        public override void Think (PhysicsComponent ownPhysics, HealthComponent ownHealth, object map,
            List<Entity> entitiesNearby)
        {
            Maze.Maze maze = map as Maze.Maze;
            if (maze != null && maze.HasFinished)
            {
                RigidBody rigidBody;
                JVector normal;
                float fraction;
                ownPhysics.World.CollisionSystem.Raycast (
                    ownPhysics.RigidBody.Position,
                    direction,
                    new Jitter.Collision.RaycastCallback((rb, n, f) => {
                        var e = rb.Tag as Entity;
                        return f < 6 && e != null && e.HasComponent<WallComponent>();
                    }),
                    out rigidBody, out normal, out fraction);
                if (rigidBody != null)
                {
                    var diff = ownPhysics.RigidBody.Position - rigidBody.Position;
                    direction += new JVector (diff.X, 0, diff.Z);
                }

                if (direction.Length() > 0.1)
                    direction.Normalize();
                else
                    direction = fallback;

                if (ownPhysics.RigidBody.Position.X > maze.Size.X * 8 || ownPhysics.RigidBody.Position.X < 0 ||
                    ownPhysics.RigidBody.Position.Z > maze.Size.Y * 8 || ownPhysics.RigidBody.Position.Z < 0)
                    direction = direction * -1;

                if (ownPhysics.RigidBody.LinearVelocity.Length() < speed)
                    ownPhysics.RigidBody.LinearVelocity += (direction * acceleration);
            }
        }

        public override void SetSpawnPosition (PhysicsComponent ownPhysics, object map, Random rand)
        {
            Maze.Maze maze = map as Maze.Maze;
            if (maze != null)
            {
                int pos = rand.Next (0, maze.graph.Nodes.Count);
                bool gotit = false;
                for (int i = pos; i < maze.graph.Nodes.Count; i++)
                {
                    if (maze.graph.Nodes[i].Data.MazeCellType == MazeCellType.Ground)
                    {
                        gotit = true;
                        ownPhysics.RigidBody.Position = new JVector (maze.graph.Nodes[i].Data.WorldPosition.X, 0.32f,
                            maze.graph.Nodes[i].Data.WorldPosition.Z);
                        break;
                    }
                }

                if (!gotit)
                {
                    Logger.Log.AddLogEntry (LogLevel.Severe, "ScobisAI", "Failed to generate spawn position!");
                }

                fallback = JVector.Transform (JVector.Backward, JMatrix.CreateFromAxisAngle (JVector.Up,
                    (float) rand.NextDouble() * 2 * MathHelper.Pi));
                direction = fallback;
            }
        }
    }
}
