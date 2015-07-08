//
//  LabyrinthGenerator.cs
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
using FreezingArcher.Core;
using FreezingArcher.DataStructures.Graphs;
using System.Collections.Generic;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;
using FreezingArcher.Content;
using Henge3D.Physics;
using Henge3D;
using FreezingArcher.Messaging;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Labyrinth generator.
    /// </summary>
    public sealed class MazeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.Maze.MazeGenerator"/> class.
        /// </summary>
        /// <param name="objmnr">Object manager.</param>
        public MazeGenerator (ObjectManager objmnr)
        {
            objectManager = objmnr;
        }

        Vector2i Offset = new Vector2i(0, 0);

        /// <summary>
        /// Generates the maze.
        /// </summary>
        /// <returns>The maze.</returns>
        /// <param name="seed">Seed.</param>
        /// <param name="messageProvider">The message provider for the maze scenes.</param>
        /// <param name="physics">Physics manager instance.</param>
        /// <param name="player">The player that should be positioned at the spawn.</param>
        /// <param name="sizeX">Size x.</param>
        /// <param name="sizeY">Size y.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="turbulence">Turbulence. The higher the more straight the maze will be.</param>
        /// <param name="maximumContinuousPathLength">Maximum continuous path length.</param>
        /// <param name="portalSpawnFactor">Portal spawn factor. The higher the less portals will appear.</param>
        public Maze CreateMaze(int seed, MessageProvider messageProvider, PhysicsManager physics, Entity player,
            int sizeX = 40, int sizeY = 40, float scale = 10, double turbulence = 2,
            int maximumContinuousPathLength = 20, uint portalSpawnFactor = 3)
        {
            Maze maze = new Maze (objectManager, seed, sizeX, sizeY, scale, physics, player, InitializeMaze, CreateMaze,
                AddMazeToScene, CalculatePathToExit, SpawnPortals, turbulence, maximumContinuousPathLength,
                portalSpawnFactor);
            maze.Offset = Offset;
            var offs = Offset;
            offs.X += (int) (sizeX * scale);
            //offs.Y += sizeY;
            Offset = offs;
            maze.Init();

            return maze;
        }

        ObjectManager objectManager;

        #region delegates

        static void CreateMaze (ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph, ref Random rand,
            int maximumContinuousPathLength, double turbulence)
        {
            WeightedNode<MazeCell, MazeCellEdgeWeight> node = null;
            WeightedNode<MazeCell, MazeCellEdgeWeight> lastNode = null;
            Direction lastDirection = Direction.Unknown;
            // set initial edge (exclude surounding wall)
            var edges = graph.Edges.Where(e => !e.FirstNode.Data.IsFinal && !e.SecondNode.Data.IsFinal);
            WeightedEdge<MazeCell, MazeCellEdgeWeight> edge = edges.ElementAt(rand.Next (0, edges.Count()));
            Vector2i spawnPos = edge.FirstNode.Data.Position;
            int pathLength = 0;
            int minimumBacktrack = 0;

            do
            {
                // save last node
                lastNode = node;

                if (edge != null)
                {
                    // get new node from edge
                    node = edge.FirstNode != node ? edge.FirstNode : edge.SecondNode;

                    // if new node is not a ground set it to ground
                    if (node.Data.MazeCellType != MazeCellType.Ground)
                    {
                        node.Data.MazeCellType = MazeCellType.Ground;
                        node.Data.IsPreview = false;
                        pathLength++;
                    }

                    // set walls surrounding last node to non-preview
                    if (lastNode != null)
                    {
                        lastNode.GetNeighbours ().ForEach ((n, e) =>
                        {
                            if (n.Data.MazeCellType == MazeCellType.Wall &&
                                !node.GetNeighbours ().Any ((n2, e2) => n == n2 && e2.Weight.Even))
                                n.Data.IsPreview = false;
                        });
                    }
                    else
                        node.Data.IsSpawn = true;
                }

                // build walls around new node and set correct preview state on those
                node.GetNeighbours ().ForEach ((n, e) =>
                {
                    if (n.Data.MazeCellType == MazeCellType.Undefined)
                    {
                        n.Data.MazeCellType = MazeCellType.Wall;
                        n.Data.IsPreview = true;
                    }
                });

                // bool indicating wether to do backtracking or not
                bool backtrack = false;

                do
                {
                    // get next edge
                    edge = null;
                    if (pathLength < maximumContinuousPathLength)
                    {
                        edge = node.GetNeighbours ().Where ((n, e) =>
                            (n.Data.MazeCellType == MazeCellType.Undefined || n.Data.IsPreview) && !n.Data.IsFinal && e.Weight.Even)
                            .MaxElem ((n, e) =>
                                n.Data.Weight * (e.Weight.Direction == lastDirection ? turbulence : 1 / turbulence)).Item2;
                    }
                    else
                        minimumBacktrack = maximumContinuousPathLength / 3;

                    // are we at an dead end?
                    node.Data.IsDeadEnd |= !backtrack && edge == null;

                    // backtracking
                    backtrack = false;
                    if (edge == null)
                    {
                        // iterate over all surrounding nodes
                        node.GetNeighbours ().ForEach ((n, e) =>
                        {
                            node.Data.IsFinal = true;
                            // can we go back?
                            if (n.Data.MazeCellType == MazeCellType.Ground && !n.Data.IsFinal && e.Weight.Even)
                            {
                                // set walls of current node to non-preview to avoid artifacts
                                node.GetNeighbours ().ForEach ((n_old, e_old) => 
                                    n_old.GetNeighbours ().ForEach ((n2_old, e2_old) =>
                                {
                                    if (n2_old.Data.MazeCellType == MazeCellType.Wall && n2_old.Data.IsPreview)
                                        n2_old.Data.IsPreview = false;
                                }));
                                // go to next node
                                node = n;

                                if (minimumBacktrack > 0)
                                {
                                    backtrack = true;
                                    pathLength = 0;
                                    minimumBacktrack--;
                                    return false;
                                }

                                // search for a wall which has undefined cells around it
                                node.GetNeighbours ().FirstOrDefault ((n2, e2) =>
                                {
                                    if (n2.Data.MazeCellType == MazeCellType.Wall && e2.Weight.Even &&
                                            n2.GetNeighbours ().Any ((n3, e3) =>
                                            n3.Data.MazeCellType == MazeCellType.Undefined && e3.Weight.Even &&
                                            n3.GetNeighbours ().Count ((n4, e4) =>
                                                n4.Data.MazeCellType == MazeCellType.Ground) <= 2))
                                    {
                                        n2.Data.IsPreview = true;
                                        return true;
                                    }
                                    return false;
                                });
                                // if we have not returned yet continue backtracking
                                backtrack = true;
                                pathLength = 0;
                                // stop this loop
                                return false;
                            }
                            // continue this loop
                            return true;
                        });
                    }
                    else
                        lastDirection = edge.Weight.Direction;
                } while (backtrack);
            } while (graph.Nodes.Count (n => n.Data.MazeCellType == MazeCellType.Ground && !n.Data.IsFinal) != 0);

            // set undefined cells to walls
            graph.Nodes.ForEach (n =>
            {
                if (n.Data.MazeCellType == MazeCellType.Undefined)
                    n.Data.MazeCellType = MazeCellType.Wall;
            });

            // set exit node
            var exit = graph.Nodes.Where (n => n.Edges.Count < 8 && n.GetNeighbours ().Any (
                (n2, e2) => n2.Data.MazeCellType == MazeCellType.Ground && e2.Weight.Even && !n2.Data.IsDeadEnd))
                .MaxElem(n => (spawnPos - n.Data.Position).Length).Data;
            exit.MazeCellType = MazeCellType.Ground;
            exit.IsExit = true;
        }

        static void InitializeMaze (ref ObjectManager objectManager,
            ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph, ref Entity[,] entities,
            ref Random rand, uint x, uint y)
        {
            entities = new Entity[x, y];
            graph = objectManager.CreateOrRecycle<WeightedGraph<MazeCell, MazeCellEdgeWeight>> ();
            graph.Init ();

            var nodesLast = new WeightedNode<MazeCell, MazeCellEdgeWeight>[x];
            MazeCell mapnode;
            var edges = new List<Pair<WeightedNode<MazeCell, MazeCellEdgeWeight>, MazeCellEdgeWeight>> ();

            for (int k = 0; k < y; k++)
            {
                var nodes = new WeightedNode<MazeCell, MazeCellEdgeWeight>[x];

                for (int i = 0; i < x; i++)
                {
                    mapnode = new MazeCell (k + "." + i, new Vector2i (i, k),
                        Quaternion.FromAxisAngle(Vector3.UnitY, rand.Next(0, 4) * MathHelper.PiOver2), rand.Next ());

                    edges.Clear ();

                    if (k > 0)
                        edges.Add (new Pair<WeightedNode<MazeCell, MazeCellEdgeWeight>,
                            MazeCellEdgeWeight> (nodesLast [i], new MazeCellEdgeWeight(true, Direction.Vertical)));

                    if (i > 0)
                        edges.Add (new Pair<WeightedNode<MazeCell, MazeCellEdgeWeight>,
                            MazeCellEdgeWeight> (nodes [i - 1], new MazeCellEdgeWeight(true, Direction.Horizontal)));

                    if (i > 0 && k > 0)
                    {
                        edges.Add (new Pair<WeightedNode<MazeCell, MazeCellEdgeWeight>,
                            MazeCellEdgeWeight> (nodesLast [i - 1], new MazeCellEdgeWeight (false, Direction.Diagonal)));
                        graph.AddEdge (nodesLast [i], nodes [i - 1], new MazeCellEdgeWeight (false, Direction.Diagonal));
                    }

                    nodes [i] = graph.AddNode (mapnode, edges);
                }

                nodesLast = nodes;
            }

            foreach (var node in (IEnumerable<WeightedNode<MazeCell, MazeCellEdgeWeight>>) graph)
            {
                node.Data.Init ();

                if (node.Edges.Count < 8)
                {
                    node.Data.MazeCellType = MazeCellType.Wall;
                    node.Data.IsPreview = false;
                    node.Data.IsFinal = true;
                }
            }
        }

        static void AddMazeToScene (WeightedGraph<MazeCell, MazeCellEdgeWeight> graph, MessageProvider messageProvider,
            Entity[,] entities, Entity player, CoreScene scene, PhysicsManager physics,
            float scaling, uint maxX, int xOffs, int yOffs)
        {
            int x = 0;
            int y = 0;

            SceneObjectArray scnobjarr_wall = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Wall/wall.xml");
            scnobjarr_wall.LayoutLocationOffset = 10;
            scene.AddObject (scnobjarr_wall);

            SceneObjectArray scnobjarr_ground = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Ground/ground.xml");
            scnobjarr_ground.LayoutLocationOffset = 10;
            scene.AddObject (scnobjarr_ground);

            var systems = new[] { typeof (ModelSystem), typeof (PhysicsSystem) };

            Vector3 scale = new Vector3 (4, 4, 4);

            var startNode = graph.Nodes.FirstOrDefault (n => n.Data.IsSpawn);
            if (startNode != null)
            {
                var pos = player.GetComponent<TransformComponent>().Position;
                player.GetComponent<TransformComponent>().Position =
                    new Vector3 (
                        startNode.Data.Position.X * scale.X * 2 + xOffs, pos.Y,
                        startNode.Data.Position.Y * scale.Y * 2 + yOffs);
            }

            ModelSceneObject model;
            TransformComponent transform;

            foreach (var node in (IEnumerable<WeightedNode<MazeCell, MazeCellEdgeWeight>>) graph)
            {
                if (node.Data.MazeCellType == MazeCellType.Ground)
                {
                    entities [x, y] = EntityFactory.Instance.CreateWith("ground" + x + "." + y, messageProvider, systems: systems);
                    model = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/ground.xml");
                    entities [x, y].GetComponent<ModelComponent>().Model = model;
                    scnobjarr_ground.AddObject (model);

                    // FIXME creates massive input lag and is glitchy as hell
                    //var groundRigidBody = new RigidBody();
                    //var groundPhysics = entities [x, y].GetComponent<PhysicsComponent>();
                    //groundPhysics.RigidBody = groundRigidBody;
                    //groundRigidBody.MassProperties = new MassProperties(float.PositiveInfinity, Matrix.Identity);
                    //groundRigidBody.Skin.DefaultMaterial = new Material(1f, 0.5f);
                    //groundRigidBody.Skin.Add(new PlanePart(Vector3.UnitZ, Vector3.UnitY));
                    //physics.Add(groundRigidBody);

                    transform = entities [x, y].GetComponent<TransformComponent>();
                    transform.Position = new Vector3 (x * scale.X * 2 + xOffs, -0.0f, y * scale.Y * 2 + yOffs);
                    transform.Scale = scale;
                }
                else
                {
                    entities [x, y] = EntityFactory.Instance.CreateWith("wall" + x + "." + y, messageProvider, systems: systems);
                    model = new ModelSceneObject("lib/Renderer/TestGraphics/Wall/wall.xml");
                    entities [x, y].GetComponent<ModelComponent>().Model = model;
                    scnobjarr_wall.AddObject(model);

                    // FIXME creates massive input lag and is glitchy as hell
                    //var wallRigidBody = new RigidBody();
                    //var wallPhysics = entities [x, y].GetComponent<PhysicsComponent>();
                    //wallPhysics.RigidBody = wallRigidBody;
                    //wallRigidBody.MassProperties = new MassProperties(float.PositiveInfinity, Matrix.Identity);
                    //Vector3 p1 = new Vector3(0, 0, 1), p2 = new Vector3(0, 0, -1);
                    //wallRigidBody.Skin.Add(new CapsulePart(new Capsule(p1, p2, 0.5f)), new Material(1f, 0.5f));
                    //physics.Add(wallRigidBody);

                    transform = entities [x, y].GetComponent<TransformComponent>();
                    transform.Position = new Vector3 (x * scale.X * 2 + xOffs, -0.5f, y * scale.Y * 2 + yOffs);
                    transform.Rotation = node.Data.Rotation;
                    transform.Scale = scale;
                }

                if (++x >= maxX)
                {
                    x = 0;
                    y++;
                }
            }
        }

        static void CalculatePathToExit(ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph)
        {
            WeightedNode<MazeCell, MazeCellEdgeWeight> node = null;
            WeightedNode<MazeCell, MazeCellEdgeWeight> lastNode; 
            graph.Nodes.ForEach(n => {
                if (n.Data.MazeCellType == MazeCellType.Ground)
                {
                    n.Data.IsFinal = false;
                    n.Data.IsPath = false;

                    if (n.Data.IsSpawn)
                        node = n;
                }
            });

            do
            {
                node.Data.IsPath = true;
                lastNode = node;
                node = lastNode.GetNeighbours().FirstOrDefault((n, e) =>
                    n.Data.MazeCellType == MazeCellType.Ground && !n.Data.IsPath && !n.Data.IsFinal && e.Weight.Even).Item1;
                
                while (node == null)
                {
                    lastNode.Data.IsFinal = true;
                    lastNode.Data.IsPath = false;
                    lastNode = lastNode.GetNeighbours ().FirstOrDefault ((n, e) =>
                        n.Data.MazeCellType == MazeCellType.Ground && n.Data.IsPath && e.Weight.Even).Item1;
                    node = lastNode.GetNeighbours ().FirstOrDefault ((n, e) =>
                        n.Data.MazeCellType == MazeCellType.Ground && !n.Data.IsPath && !n.Data.IsFinal && e.Weight.Even).Item1;
                }
            } while (!node.Data.IsExit);

            graph.Nodes.ForEach(n => {
                if (n.Data.MazeCellType == MazeCellType.Ground)
                {
                    n.Data.IsFinal = true;
                    n.Data.IsPreview = false;
                }
            });
        }

        static void SpawnPortals(WeightedGraph<MazeCell, MazeCellEdgeWeight> previous,
            WeightedGraph<MazeCell, MazeCellEdgeWeight> current,
            WeightedGraph<MazeCell, MazeCellEdgeWeight> next, Random rand, uint portalSpawnFactor)
        {
            if (previous == null)
            {
                for (int i = 0; i < current.Nodes.Count; i++)
                {
                    if (current.Nodes[i].Data.IsDeadEnd &&
                        next.Nodes[i].Data.MazeCellType == MazeCellType.Ground &&
                        rand.Next() % portalSpawnFactor == 0)
                        current.Nodes[i].Data.IsPortal = true;
                }
            }
            else if (next == null)
            {
                for (int i = 0; i < current.Nodes.Count; i++)
                {
                    if (previous.Nodes[i].Data.MazeCellType == MazeCellType.Ground &&
                        current.Nodes[i].Data.IsDeadEnd &&
                        rand.Next() % portalSpawnFactor == 0)
                        current.Nodes[i].Data.IsPortal = true;
                }                
            }
            else
            {
                for (int i = 0; i < current.Nodes.Count; i++)
                {
                    if (previous.Nodes[i].Data.MazeCellType == MazeCellType.Ground &&
                        current.Nodes[i].Data.IsDeadEnd &&
                        next.Nodes[i].Data.MazeCellType == MazeCellType.Ground &&
                        rand.Next() % portalSpawnFactor == 0)
                        current.Nodes[i].Data.IsPortal = true;
                }
            }
        }
        #endregion
    }
}
