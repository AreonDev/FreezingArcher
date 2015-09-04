//
//  CaligoAI.cs
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
using System.Linq;
using FreezingArcher.Content;
using System.Collections.Generic;
using FreezingArcher.Math;
using FreezingArcher.Game.Maze;
using FreezingArcher.Output;
using Jitter.Dynamics;
using FreezingArcher.Core;
using Jitter.LinearMath;
using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Audio;
using FreezingArcher.Audio.Filters;
using FreezingArcher.Messaging;

namespace FreezingArcher.Game.AI
{
    public sealed class CaligoAI : ArtificialIntelligence, Messaging.Interfaces.IMessageCreator
    {
        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion

        private MessageProvider Provider;

        public CaligoAI (Entity entity, GameState state, CompositorWarpingNode warpingNode)
        {
            this.entity = entity;
            this.state = state;
            this.warpingNode = warpingNode;
            this.AIcomp = entity.GetComponent<ArtificialIntelligenceComponent>();

            Provider = state.MessageProxy;
            this.Provider += this;
        }

        const float acceleration = 0.2f;

        const float speed = 5f;

        const int resolution = 2;

        const float max_distance = 6;

        const float height = 2f;

        JVector direction;

        JVector fallback;

        readonly Entity entity;
        readonly GameState state;
        readonly ArtificialIntelligenceComponent AIcomp;
        readonly CompositorWarpingNode warpingNode;

        bool do_reset= false;

        Entity temp_player;

        DateTime lastDamage;

        public override void Think (PhysicsComponent ownPhysics, HealthComponent ownHealth, object map,
            List<Entity> entitiesNearby)
        {
            Maze.Maze maze = map as Maze.Maze;
            if (maze != null && maze.HasFinished)
            {
                RigidBody rigidBody;
                JVector normal, temp_direction, old_direction = direction;
                float fraction;
                const int res_over_two = resolution / 2;
                direction = JVector.Zero;
                for (int i = -res_over_two; i <= res_over_two; i++)
                {
                    temp_direction = JVector.Transform (old_direction, JMatrix.CreateFromAxisAngle (JVector.Up,
                        i / res_over_two * MathHelper.PiOver4));
                    ownPhysics.World.CollisionSystem.Raycast (
                        ownPhysics.RigidBody.Position,
                        temp_direction,
                        new Jitter.Collision.RaycastCallback((rb, n, f) => {
                            var e = rb.Tag as Entity;
                            return f < max_distance && e != null && (e.HasComponent<WallComponent>() || e.Name.Contains("exit"));
                        }),
                        out rigidBody, out normal, out fraction);
                    
                    if (rigidBody != null)
                    {
                        var diff = ownPhysics.RigidBody.Position - rigidBody.Position;
                        diff = new JVector (diff.X, 0, diff.Z);
                        diff.Normalize();
                        direction += diff * (max_distance - fraction);
                    }
                    else
                    {
                        direction += temp_direction * max_distance;
                    }
                }

                if (direction.Length() > 0.1)
                    direction.Normalize();
                else
                {
                    fallback = JVector.Transform (fallback, JMatrix.CreateFromAxisAngle (JVector.Up, MathHelper.PiOver6));
                    direction = fallback;
                }

                if (ownPhysics.RigidBody.Position.X > maze.Size.X * 8)
                    direction = new JVector ((maze.Size.X * 8) - ownPhysics.RigidBody.Position.X, 0, direction.Z);
                else if (ownPhysics.RigidBody.Position.X < 0)
                    direction = new JVector (0 - ownPhysics.RigidBody.Position.X, 0, direction.Z);

                if (ownPhysics.RigidBody.Position.Z > maze.Size.Y * 8)
                    direction = new JVector (direction.X, 0, (maze.Size.Y * 8) - ownPhysics.RigidBody.Position.Z);
                else if (ownPhysics.RigidBody.Position.Z < 0)
                    direction = new JVector (direction.X, 0, 0 - ownPhysics.RigidBody.Position.Z);

                if (ownPhysics.RigidBody.LinearVelocity.Length() < speed)
                    ownPhysics.RigidBody.LinearVelocity += (direction * acceleration);

                ownPhysics.RigidBody.Position = new JVector (ownPhysics.RigidBody.Position.X, height,
                    ownPhysics.RigidBody.Position.Z);

                var player = entitiesNearby.FirstOrDefault (e => e.Name == "player");

                if (player != null)
                {
                    do_reset = true;
                    var player_pos = player.GetComponent<TransformComponent>().Position;
                    var ghost_pos = ownPhysics.RigidBody.Position.ToFreezingArcherVector();
                    float distance;
                    Vector3.Distance(ref player_pos, ref ghost_pos, out distance);
                    float fac = ((AIcomp.MaximumEntityDistance - distance) / AIcomp.MaximumEntityDistance);
                    warpingNode.WarpFactor = fac / 10;

                    var playerPhysics = player.GetComponent<PhysicsComponent>();
                    if (playerPhysics != null)
                        playerPhysics.SpeedMultiplier = 1 - fac;

                    if ((DateTime.Now - lastDamage).TotalSeconds > 0.5f)
                    {
                        player.GetComponent<HealthComponent>().Health -= fac * 40;
                        lastDamage = DateTime.Now;
                    }

                    var lowpass = state.AudioContext.GlobalFilter as LowpassFilter;
                    if (lowpass == null)
                    {
                        lowpass = new LowpassFilter ();
                        state.AudioContext.GlobalFilter = lowpass;
                    }
                    lowpass.GainHF = (1 - fac) / 10;
                    temp_player = player;

                    //Here send Caligo Attack Message
                    if (MessageCreated != null)
                        MessageCreated (new AIAttackMessage (entity));
                    
                }
                else if (do_reset)
                {
                    do_reset = false;
                    warpingNode.WarpFactor = 0;
                    if (temp_player != null)
                        temp_player.GetComponent<PhysicsComponent>().SpeedMultiplier = 1;

                    var lowpass = state.AudioContext.GlobalFilter as LowpassFilter;
                    if (lowpass != null)
                        lowpass.GainHF = 1;
                    state.AudioContext.GlobalFilter = null;
                }
            }
        }

        public override void SetSpawnPosition (Vector3 playerSpawn, PhysicsComponent ownPhysics, object map, FastRandom rand)
        {
            Maze.Maze maze = map as Maze.Maze;
            if (maze != null)
            {
                bool gotit = false;

                while (!gotit)
                {
                    int pos = rand.Next (0, maze.graph.Nodes.Count);
                    Vector3 spawn_pos;
                    float distance;
                    for (int i = pos; i < maze.graph.Nodes.Count; i++)
                    {
                        spawn_pos = maze.graph.Nodes[i].Data.WorldPosition;
                        Vector3.Distance(ref playerSpawn, ref spawn_pos, out distance);
                        if (maze.graph.Nodes[i].Data.MazeCellType == MazeCellType.Ground && distance > 50 &&
                            !maze.graph.Nodes[i].Data.IsExit)
                        {
                            gotit = true;
                            ownPhysics.RigidBody.Position = new JVector (maze.graph.Nodes[i].Data.WorldPosition.X, height,
                                maze.graph.Nodes[i].Data.WorldPosition.Z);
                            break;
                        }
                    }
                }

                if (!gotit)
                {
                    Logger.Log.AddLogEntry (LogLevel.Severe, "CaligoAI", "Failed to generate spawn position!");
                }

                fallback = JVector.Transform (JVector.Backward, JMatrix.CreateFromAxisAngle (JVector.Up,
                    (float) rand.NextDouble() * 2 * MathHelper.Pi));
                direction = fallback;

                Spawned = true;
            }
        }
    }
}
