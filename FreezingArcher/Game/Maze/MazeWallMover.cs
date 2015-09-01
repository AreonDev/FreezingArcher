//
//  MazeWallMover.cs
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
using System.Collections.Generic;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Content;
using FreezingArcher.Renderer.Scene.SceneObjects;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using FreezingArcher.Core;
using FreezingArcher.Math;

namespace FreezingArcher.Game.Maze
{
    public sealed class MazeWallMover : IMessageConsumer, IMessageCreator
    {
        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion

        public MazeWallMover (Maze maze, Maze secondMaze, MessageProvider messageProvider, GameState state)
        {
            Maze = maze;
            SecondMaze = secondMaze;
            this.state = state;
            rand = new Random(maze.Seed);

            this.messageProvider = messageProvider;

            ValidMessages = new[] { (int) MessageId.Update };
            messageProvider += this;
        }

        public Maze Maze { get; private set; }

        public Maze SecondMaze { get; private set; }

        readonly GameState state;

        readonly MessageProvider messageProvider;

        readonly Random rand;

        void Step ()
        {
            foreach (var deadEnd in GetDeadEndGrounds ())
            {
                if (rand.Next() % 10000 != 0)
                    continue;

                WeightedNode<MazeCell, MazeCellEdgeWeight> connection = null;
                var new_ground_edge = deadEnd.Edges.FirstOrDefault(e => {
                    var n = e.FirstNode != deadEnd ? e.FirstNode : e.SecondNode;
                    var wall_component = Maze.entities[n.Data.Position.X, n.Data.Position.Y].GetComponent<WallComponent>();
                    if (n.Data.MazeCellType == MazeCellType.Wall && e.Weight.Direction != Direction.Diagonal &&
                        wall_component.IsMoveable && !wall_component.IsMoving)
                    {
                        foreach (var e2 in n.Edges)
                        {
                            var n2 = e2.FirstNode != n ? e2.FirstNode : e2.SecondNode;
                            if (e2.Weight.Direction != Direction.Diagonal &&
                                n2.Data.MazeCellType == MazeCellType.Ground && !n2.Data.IsDeadEnd && !n2.Data.IsPortal)
                            {
                                connection = n2;
                                break;
                            }
                        }

                        return connection != null;
                    }
                    return false;
                });

                if (connection == null)
                    continue;
                
                var grounds = connection.Edges.Where (e => {
                    var n = e.FirstNode != connection ? e.FirstNode : e.SecondNode;
                    return n.Data.MazeCellType == MazeCellType.Ground;
                });

                if (grounds.Any (g => g.Weight.Direction != grounds.ElementAt (0).Weight.Direction) ||
                    grounds.Count () > 2)
                    continue;

                if (new_ground_edge != null)
                {
                    var tmp2 = connection.Edges.FirstOrDefault(e => e.Weight.IsNextGenerationStep);
                    var tmp = connection.Edges.FirstOrDefault(e => e.Weight.Direction == tmp2.Weight.Direction && e != tmp2);
                    if (tmp != null)
                    {
                        var next_gen_ground = tmp.FirstNode != connection ? tmp.FirstNode : tmp.SecondNode;
                        next_gen_ground.Data.MazeCellType = MazeCellType.Wall; // here
                        var new_ground = new_ground_edge.FirstNode != deadEnd ?
                            new_ground_edge.FirstNode : new_ground_edge.SecondNode;
                        new_ground.Data.MazeCellType = MazeCellType.Ground; // here
                        deadEnd.Data.IsDeadEnd = false;
                        new_ground_edge.Weight.IsNextGenerationStep = true;

                        foreach (var e in new_ground.Edges)
                        {
                            var n = e.FirstNode != new_ground ? e.FirstNode : e.SecondNode;
                            if (n.Data.MazeCellType == MazeCellType.Ground)
                            {
                                n.Data.IsDeadEnd = true;
                                e.Weight.IsNextGenerationStep = false;
                                break;
                            }
                        }

                        AnimateMovement(next_gen_ground, new_ground, connection);
                    }
                }
            }
        }

        int temp_counter = 0;

        void AnimateMovement(WeightedNode<MazeCell, MazeCellEdgeWeight> groundNode,
            WeightedNode<MazeCell, MazeCellEdgeWeight> wallNode,
            WeightedNode<MazeCell, MazeCellEdgeWeight> connection)
        {
            var ground1 = Maze.entities[groundNode.Data.Position.X, groundNode.Data.Position.Y];
            var wall = Maze.entities[wallNode.Data.Position.X, wallNode.Data.Position.Y];

            var old_pos = ground1.GetComponent<TransformComponent>().Position;
            var wall_transform = wall.GetComponent<TransformComponent>();
            wall.GetComponent<WallComponent>().IsMoving = true;
            var new_ground_position = new Vector3 (wall_transform.Position.X, 0, wall_transform.Position.Z);

            var ground2 = EntityFactory.Instance.CreateWith ("ground_temp" + temp_counter++, messageProvider,
                systems: new[] { typeof (ModelSystem), typeof (PhysicsSystem) });

            var ground2_model = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/ground.xml");
            ground2.GetComponent<ModelComponent>().Model = ground2_model;
            var transform = ground2.GetComponent<TransformComponent>();
            transform.Position = old_pos;
            transform.Scale = ground1.GetComponent<TransformComponent>().Scale;
            ground2_model.Position = transform.Position;
            var body = new RigidBody(new BoxShape (2.0f * transform.Scale.X, 0.2f, 2.0f * transform.Scale.Y));
            body.Position = transform.Position.ToJitterVector ();
            body.Material.Restitution = -10;
            body.IsStatic = true;
            ground2.GetComponent<PhysicsComponent>().RigidBody = body;
            ground2.GetComponent<PhysicsComponent>().World = state.PhysicsManager.World;
            ground2.GetComponent<PhysicsComponent>().PhysicsApplying =
                AffectedByPhysics.Orientation | AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (body);
            state.Scene.AddObject(ground2_model);

            ground1.GetComponent<TransformComponent>().Position = new_ground_position;
            ground1.GetComponent<PhysicsComponent>().RigidBody.Position = new_ground_position.ToJitterVector();

            var position = old_pos;
            position = new Vector3 (position.X, -0.5f, position.Z);

            Maze.entities[wallNode.Data.Position.X, wallNode.Data.Position.Y] = ground1;
            Maze.entities[groundNode.Data.Position.X, groundNode.Data.Position.Y] = wall;

            var tmp_wall_position = Maze.entities[connection.Data.Position.X, connection.Data.Position.Y]
                .GetComponent<TransformComponent>().Position;
            tmp_wall_position = new Vector3 (tmp_wall_position.X, -0.5f, tmp_wall_position.Z);

            MoveEntityTo (wall, tmp_wall_position, position, () => {
                state.Scene.RemoveObject(ground2_model);
                wall.GetComponent<WallComponent>().IsMoving = false;
                ground2.Destroy(); if(MessageCreated != null) MessageCreated(new EndWallMovementMessage(wall));
            });
        }

        class EntityMover : IMessageConsumer
        {
            public EntityMover(MessageProvider messageProvider, int steps, Vector3 position_1, Vector3 position_2,
                TransformComponent transform, RigidBody rigidBody, Action finishedCallback)
            {
                this.steps = steps;
                this.position_1 = position_1;
                this.position_2 = position_2;
                this.transform = transform;
                this.rigidBody = rigidBody;
                step = (position_1 - transform.Position) / (float) steps;
                this.finishedCallback = finishedCallback;
                this.messageProvider = messageProvider;
                ValidMessages = new[] { (int) MessageId.Update };
                messageProvider += this;
            }

            readonly int steps;
            int count_1 = 0;
            int count_2 = 0;
            readonly Vector3 position_1;
            readonly Vector3 position_2;
            Vector3 step;
            readonly TransformComponent transform;
            readonly RigidBody rigidBody;
            readonly Action finishedCallback;
            MessageProvider messageProvider;

            #region IMessageConsumer implementation
            public void ConsumeMessage (IMessage msg)
            {
                if (msg.MessageId == (int) MessageId.Update)
                {
                    if (count_2 == steps)
                    {
                        finishedCallback();
                        messageProvider -= this;
                        count_2++;
                    }

                    if (count_1 == steps && count_2 == 0)
                    {
                        step = (position_2 - transform.Position) / (float) steps;
                    }

                    if (count_1 < steps)
                    {
                        transform.Position += step;
                        rigidBody.Position += step.ToJitterVector();
                        count_1++;
                    }
                    else if (count_2 < steps)
                    {
                        transform.Position += step;
                        rigidBody.Position += step.ToJitterVector();
                        count_2++;
                    }
                }
            }

            public int[] ValidMessages { get; private set; }
            #endregion
            
        }

        void MoveEntityTo (Entity entity, Vector3 position_1, Vector3 position_2, Action finishedCallback, int time_steps = 200)
        {
            if (MessageCreated != null)
                MessageCreated (new BeginWallMovementMessage (entity));

            var transform = entity.GetComponent<TransformComponent>();
            var rigidBody = entity.GetComponent<PhysicsComponent>().RigidBody;
            new EntityMover(messageProvider, time_steps, position_1, position_2, transform, rigidBody, finishedCallback);
        }

        IEnumerable<WeightedNode<MazeCell, MazeCellEdgeWeight>> GetDeadEndGrounds ()
        {
            return Maze.graph.Nodes.Where(n => n.Data.IsDeadEnd && !n.Data.IsPortal);
        }

        #region IMessageConsumer implementation
        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update && Maze.HasFinished && SecondMaze.HasFinished)
            {
                Step();
            }
        }

        public int[] ValidMessages { get; private set; }
        #endregion
    }
}
