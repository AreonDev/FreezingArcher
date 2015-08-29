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
using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Math;
using FreezingArcher.Content;
using FreezingArcher.Messaging;
using FreezingArcher.Output;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Labyrinth generator.
    /// </summary>
    public sealed class MazeGenerator
    {
        #region templates

        public static readonly ItemTemplate[] ItemTemplates = new[] {
            new ItemTemplate {
                Name = "choco_milk",
                ImageLocation = "Content/ChocoMilk/thumb.png",
                Description = "choco_milk_description",
                ModelPath = "Content/ChocoMilk/choco_milk.xml",
                Size = new Vector2i(1, 1),
                PositionOffset = new Vector3(-.4f, -.25f, .5f),
                Rotation = ItemComponent.DefaultRotation,
                Shape = new BoxShape(0.1f, 0.14f, 0.1f),
                AttackClasses = AttackClass.Object,
                ItemUsages =  ItemUsage.Eatable | ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = .5f,
                HealthDelta = 20,
                UsageDeltaPerUsage = .2f,
                AttackStrength = 5,
                ThrowPower = 0.2f,
                Usage = .2f
            },
            new ItemTemplate {
                Name = "flashlight",
                ImageLocation = "Content/Flashlight/thumb.png",
                Description = "flashlight_description",
                ModelPath = "Content/Flashlight/flashlight.xml",
                Size = new Vector2i(2, 1),
                PositionOffset = new Vector3(-0.45f, -0.33f, 0.5f),
                Rotation = Quaternion.FromAxisAngle (Vector3.UnitX, MathHelper.PiOver2),
                Shape = new CylinderShape (0.552666f, 0.080992f),
                AttackClasses = AttackClass.Object,
                ItemUsages =  ItemUsage.Throwable | ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = 1,
                HealthDelta = 0,
                UsageDeltaPerUsage = .0001f,
                AttackStrength = 10,
                ThrowPower = 5,
                Usage = 0
            },
            new ItemTemplate {
                Name = "pickaxe",
                ImageLocation = "Content/Pickaxe/thumb.png",
                Description = "pickaxe_description",
                ModelPath = "Content/Pickaxe/pickaxe.xml",
                Size = new Vector2i(2, 4),
                PositionOffset = new Vector3(-0.4f, -0.3f, 0.5f),
                Rotation = Quaternion.FromAxisAngle (Vector3.UnitZ, MathHelper.PiOver2),
                AttackClasses = AttackClass.Object,
                ItemUsages =  ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = 2,
                HealthDelta = 0,
                UsageDeltaPerUsage = .25f,
                AttackStrength = 25,
                ThrowPower = 5,
                Usage = 0
            },
            new ItemTemplate {
                Name = "soda_can",
                ImageLocation = "Content/SodaCan/thumb.png",
                Description = "soda_can_description",
                ModelPath = "Content/SodaCan/soda_can.xml",
                Size = new Vector2i(1, 1),
                PositionOffset = new Vector3(-0.4f, -0.25f, 0.5f),
                Rotation = ItemComponent.DefaultRotation,
                Shape = new CylinderShape (0.13f, 0.032f),
                AttackClasses = AttackClass.Object,
                ItemUsages =  ItemUsage.Eatable | ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = .5f,
                HealthDelta = 20,
                UsageDeltaPerUsage = .2f,
                AttackStrength = 5,
                ThrowPower = .2f,
                Usage = 0
            },
            new ItemTemplate {
                Name = "apple",
                ImageLocation = "Content/Apple/thumb.png",
                Description = "apple_description",
                ModelPath = "Content/Apple/apple.xml",
                Size = new Vector2i (1, 1),
                PositionOffset = new Vector3 (-.4f, -.25f, .5f),
                Rotation = ItemComponent.DefaultRotation,
                Shape = new SphereShape (.08f),
                AttackClasses = AttackClass.Object,
                ItemUsages = ItemUsage.Eatable | ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = .5f,
                HealthDelta = 20,
                UsageDeltaPerUsage = .25f,
                AttackStrength = 5,
                ThrowPower = .2f,
                Usage = 0
            },
            new ItemTemplate {
                Name = "mate",
                ImageLocation = "Content/Mate/thumb.png",
                Description = "mate_description",
                ModelPath = "Content/Mate/mate.xml",
                Size = new Vector2i (1, 2),
                PositionOffset = new Vector3 (-.4f, -.25f, .5f),
                Rotation = ItemComponent.DefaultRotation,
                Shape = new CylinderShape (.342884f, .043448f),
                AttackClasses = AttackClass.Object,
                ItemUsages = ItemUsage.Eatable | ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = .5f,
                HealthDelta = 25,
                UsageDeltaPerUsage = .20f,
                AttackStrength = 5,
                ThrowPower = .2f,
                Usage = 0
            },
            new ItemTemplate {
                Name = "toast",
                ImageLocation = "Content/Toast/thumb.png",
                Description = "toast_description",
                ModelPath = "Content/Toast/toast.xml",
                Size = new Vector2i (1, 1),
                PositionOffset = new Vector3 (-.4f, -.25f, .5f),
                Rotation = Quaternion.FromAxisAngle (Vector3.UnitX, MathHelper.PiOver2),
                Shape = new BoxShape (0.278638f, 0.045314f, 0.230326f),
                AttackClasses = AttackClass.Object,
                ItemUsages = ItemUsage.Eatable | ItemUsage.Hitable,
                Protection = ItemComponent.DefaultProtection,
                PhysicsMaterial = new Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                Mass = .5f,
                HealthDelta = 25,
                UsageDeltaPerUsage = .20f,
                AttackStrength = 5,
                ThrowPower = .2f,
                Usage = 0
            }
        };

        #endregion

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
        /// <param name="sizeX">Size x.</param>
        /// <param name="sizeY">Size y.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="turbulence">Turbulence. The higher the more straight the maze will be.</param>
        /// <param name="maximumContinuousPathLength">Maximum continuous path length.</param>
        /// <param name="portalSpawnFactor">Portal spawn factor. The higher the less portals will appear.</param>
        public Maze CreateMaze(int seed, MessageProvider messageProvider, PhysicsManager physics,
            int sizeX = 40, int sizeY = 40, float scale = 10, double turbulence = 2,
            int maximumContinuousPathLength = 20, uint portalSpawnFactor = 3)
        {
            Maze maze = new Maze (objectManager, messageProvider, seed, sizeX, sizeY, scale, physics, InitializeMaze,
                CreateMaze, AddMazeToGameState, CalculatePathToExit, SpawnPortals, turbulence,
                maximumContinuousPathLength, portalSpawnFactor);
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

                edge.Weight.IsNextGenerationStep = true;

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
                if (node.Edges.Count < 8)
                {
                    node.Data.MazeCellType = MazeCellType.Wall;
                    node.Data.IsPreview = false;
                    node.Data.IsFinal = true;
                    node.Data.IsEdge = true;
                }
            }
        }

        static int lightCount = 0;
        static int choco_milk_idx = 0;
        static int flashlight_idx = 0;
        static int pickaxe_idx = 0;
        static int soda_can_idx = 0;
        static int apple_idx = 0;
        static int mate_idx = 0;
        static int toast_idx = 0;

        static void AddMazeToGameState (WeightedGraph<MazeCell, MazeCellEdgeWeight> graph, MessageProvider messageProvider,
            Entity[,] entities, ref Vector3 playerPosition, GameState state, Random rand,
            float scaling, uint maxX, int xOffs, int yOffs)
        {
            int x = 0;
            int y = 0;

            SceneObjectArray scnobjarr_wall = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Wall/wall.xml");
            scnobjarr_wall.LayoutLocationOffset = 10;
            state.Scene.AddObject (scnobjarr_wall);

            SceneObjectArray scnobjarr_ground = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Ground/ground.xml");
            scnobjarr_ground.LayoutLocationOffset = 10;
            state.Scene.AddObject (scnobjarr_ground);

            scnobjarr_wall.BeginPrepare ();
            scnobjarr_ground.BeginPrepare ();

            var systems = new[] { typeof (ModelSystem), typeof (PhysicsSystem) };

            Vector3 scale = new Vector3 (4, 4, 4);

            var startNode = graph.Nodes.FirstOrDefault (n => n.Data.IsSpawn);
            if (startNode != null)
            {
                playerPosition = new Vector3 (
                    startNode.Data.Position.X * scale.X * 2 + xOffs, playerPosition.Y,
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

                    transform = entities [x, y].GetComponent<TransformComponent>();
                    transform.Position = new Vector3 (x * scale.X * 2 + xOffs, 0, y * scale.Y * 2 + yOffs);
                    node.Data.WorldPosition = transform.Position;
                    transform.Scale = scale;

                    var body = new RigidBody (new BoxShape (2.0f * scale.X, 0.2f, 2.0f * scale.Y));
                    body.Position = new JVector(transform.Position.X, transform.Position.Y - 0.1f, transform.Position.Z);
                    body.Material.Restitution = -10;
                    body.IsStatic = true;
                    body.Tag = node.Data;

                    entities [x, y].GetComponent<PhysicsComponent> ().RigidBody = body;
                    entities [x, y].GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
                    entities [x, y].GetComponent<PhysicsComponent> ().PhysicsApplying =
                        AffectedByPhysics.Orientation | AffectedByPhysics.Position;

                    state.PhysicsManager.World.AddBody (body);

                    // TODO add items here
                    var r = rand.Next(0, 200);

                    string name = string.Empty;
                    int idx;
                    // pickaxe
                    if (r == 0)
                    {
                        idx = 2;
                        name = ItemTemplates[idx].Name + pickaxe_idx++;
                    }
                    // flashlight
                    else if (r > 0 && r <= 4)
                    {
                        if (lightCount++ < CompositorNodeScene.MaximumLightCount)
                        {
                            idx = 1;
                            name = ItemTemplates[idx].Name + flashlight_idx++;
                        }
                        else
                        {
                            idx = -1;
                        }
                    }
                    // choco_milk
                    else if (r > 4 && r <= 20)
                    {
                        idx = 0;
                        name = ItemTemplates[idx].Name + choco_milk_idx++;
                    }
                    // soda_can
                    else if (r > 20 && r <= 28)
                    {
                        idx = 3;
                        name = ItemTemplates[idx].Name + soda_can_idx++;
                    }
                    else if (r > 28 && r <= 36)
                    {
                        idx = 4;
                        name = ItemTemplates[idx].Name + apple_idx++;
                    }
                    else if (r > 36 && r <= 44)
                    {
                        idx = 5;
                        name = ItemTemplates[idx].Name + mate_idx++;
                    }
                    else if (r > 44 && r <= 52)
                    {
                        idx = 6;
                        name = ItemTemplates[idx].Name + toast_idx++;
                    }
                    else
                    {
                        idx = -1;
                    }

                    if (idx >= 0)
                    {
                        var item = Inventory.CreateNewItem(messageProvider, state,
                            name,
                            ItemTemplates[idx].ImageLocation,
                            ItemTemplates[idx].Description,
                            ItemTemplates[idx].ModelPath,
                            ItemTemplates[idx].Size,
                            ItemTemplates[idx].PositionOffset,
                            ItemTemplates[idx].Rotation,
                            ItemTemplates[idx].Shape,
                            ItemLocation.Ground,
                            ItemTemplates[idx].AttackClasses,
                            ItemTemplates[idx].ItemUsages,
                            ItemTemplates[idx].Protection,
                            ItemTemplates[idx].PhysicsMaterial,
                            ItemTemplates[idx].Mass,
                            ItemTemplates[idx].HealthDelta,
                            ItemTemplates[idx].UsageDeltaPerUsage,
                            ItemTemplates[idx].AttackStrength,
                            ItemTemplates[idx].ThrowPower,
                            ItemTemplates[idx].Usage
                        );
                        var item_body = item.Entity.GetComponent<PhysicsComponent>();
                        var item_model = item.Entity.GetComponent<ModelComponent>();
                        var pos = transform.Position;
                        float y_rot = (float) rand.NextDouble();
                        pos.X += (float) rand.NextDouble() * 3.8f - 2f;

                        if (idx != 1)
                            pos.Y -= item_body.RigidBody.Shape.BoundingBox.Min.Y;
                        else
                            pos.Y -= item_body.RigidBody.Shape.BoundingBox.Min.X;

                        pos.Z += (float) rand.NextDouble() * 3.8f - 2f;
                        item_body.RigidBody.Position = pos.ToJitterVector();
                        item_body.RigidBody.Orientation = JMatrix.CreateFromAxisAngle(JVector.Up, y_rot);
                        item_model.Model.Position = pos;
                        item_model.Model.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, y_rot);

                        if (idx == 1)
                        {
                                item.Entity.AddSystem<LightSystem> ();
                                var light = item.Entity.GetComponent<LightComponent> ().Light;
                                light = new Light (LightType.SpotLight);
                                light.Color = new Color4 (0.1f, 0.1f, 0.1f, 1.0f);
                                light.PointLightLinearAttenuation = 0.01f;
                                light.SpotLightConeAngle = MathHelper.ToRadians (30f);
                                light.On = false;

                                item.Entity.GetComponent<LightComponent> ().Light = light;

                                state.Scene.AddLight(light);

                                item_model.Model.Rotation = Quaternion.FromAxisAngle (Vector3.UnitX, MathHelper.PiOver2) * item_model.Model.Rotation;
                                item_body.RigidBody.Orientation = JMatrix.CreateFromQuaternion (item_model.Model.Rotation.ToJitterQuaternion ());

                            //Update position
                            item_body.RigidBody.Position = pos.ToJitterVector();
                            item_body.RigidBody.Orientation = JMatrix.CreateFromAxisAngle(JVector.Up, y_rot);
                        }
                    }
                }
                else
                {
                    entities [x, y] = EntityFactory.Instance.CreateWith("wall" + x + "." + y, messageProvider,
                        new[] { typeof (HealthComponent), typeof(WallComponent) }, systems);
                    model = new ModelSceneObject("lib/Renderer/TestGraphics/Wall/wall.xml");
                    entities [x, y].GetComponent<ModelComponent>().Model = model;
                    scnobjarr_wall.AddObject(model);

                    transform = entities [x, y].GetComponent<TransformComponent>();
                    transform.Position = new Vector3 (x * scale.X * 2 + xOffs, -0.5f, y * scale.Y * 2 + yOffs);
                    node.Data.WorldPosition = transform.Position;
                    transform.Rotation = node.Data.Rotation;
                    transform.Scale = scale;

                    var body = new RigidBody (new BoxShape (scale.X * 2, scale.Y * 4, scale.Z * 2));
                    body.Position = transform.Position.ToJitterVector () + JVector.Up * (scale.Y * 4 * 0.5f);
                    body.Material.Restitution = -10;
                    body.IsStatic = true;
                    body.Tag = entities [x, y];

                    entities [x, y].GetComponent<PhysicsComponent> ().RigidBody = body;
                    entities [x, y].GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
                    entities [x, y].GetComponent<PhysicsComponent> ().PhysicsApplying =
                        AffectedByPhysics.Orientation | AffectedByPhysics.Position;

                    entities [x, y].GetComponent<WallComponent>().IsEdge = node.Data.IsEdge;

                    state.PhysicsManager.World.AddBody (body);
                }

                if (++x >= maxX)
                {
                    x = 0;
                    y++;
                }
            }

            scnobjarr_wall.EndPrepare ();
            scnobjarr_ground.EndPrepare ();
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
