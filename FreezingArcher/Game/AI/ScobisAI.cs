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

        const float acceleration = .5f;

        const float speed = 0.75f;

        JVector direction = JVector.Backward;

        public override void Think (PhysicsComponent ownPhysics, HealthComponent ownHealth, object map,
            List<Entity> entitiesNearby)
        {
            Maze.Maze maze = map as Maze.Maze;
            if (maze != null && maze.HasFinished)
            {
                if (ownPhysics.RigidBody.Arbiters.Count > 0)
                {
                    try
                    {
                        foreach (Arbiter arbiter in ownPhysics.RigidBody.Arbiters)
                        {
                            var body = arbiter.Body1 == ownPhysics.RigidBody ? arbiter.Body2 : arbiter.Body1;
                            var diff = ownPhysics.RigidBody.Position - body.Position;
                            direction += new JVector (diff.X, 0, diff.Z);
                        }

                        if (direction.Length() > 0)
                            direction.Normalize ();
                        else
                            direction = JVector.Backward;
                    }
                    catch {}
                }
                /*else
                {
                    direction = ownPhysics.RigidBody.Force;
                    if (direction.Length() > 0)
                        direction.Normalize ();
                    else
                        direction = JVector.Backward;
                }*/

                if (ownPhysics.RigidBody.Force.Length() < speed)
                    ownPhysics.RigidBody.AddForce (direction * acceleration);
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
            }
        }
    }
}
