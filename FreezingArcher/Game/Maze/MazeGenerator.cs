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
//#define DO_STEP
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using FreezingArcher.Core;
using FreezingArcher.DataStructures.Graphs;
using System.Collections.Generic;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;
using FreezingArcher.Output;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the neighbour nodes.
        /// </summary>
        /// <returns>The neighbour nodes.</returns>
        /// <param name="node">The node.</param>
        /// <typeparam name="TData">Data type.</typeparam>
        /// <typeparam name="TWeight">Weight type.</typeparam>
        public static IEnumerable<Tuple<WeightedNode<TData, TWeight>, WeightedEdge<TData, TWeight>>>
        GetNeighbours<TData, TWeight> (this WeightedNode<TData, TWeight> node)
            where TWeight : IComparable
        {
            foreach (var e in node.Edges)
                yield return e.FirstNode != node ?
                    new Tuple<WeightedNode<TData, TWeight>, WeightedEdge<TData, TWeight>> (e.FirstNode, e) :
                    new Tuple<WeightedNode<TData, TWeight>, WeightedEdge<TData, TWeight>> (e.SecondNode, e);
        }
    }

    /// <summary>
    /// Maze cell edge weight.
    /// </summary>
    public class MazeCellEdgeWeight : IComparable<MazeCellEdgeWeight>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.Maze.MazeCellEdgeWeight"/> class.
        /// </summary>
        /// <param name="even">If set to <c>true</c> even.</param>
        /// <param name="direction">Direction.</param>
        public MazeCellEdgeWeight(bool even, Direction direction)
        {
            Even = even;
            Direction = direction;
        }

        /// <summary>
        /// The even flag.
        /// </summary>
        public bool Even;

        /// <summary>
        /// The direction.
        /// </summary>
        public Direction Direction;

        #region IComparable implementation

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <returns>The compare result.</returns>
        /// <param name="other">The value to compare to.</param>
        public int CompareTo (MazeCellEdgeWeight other)
        {
            return other != null ? Even.CompareTo (other.Even) : -1;
        }

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <returns>The compare result.</returns>
        /// <param name="obj">The value to compare to.</param>
        public int CompareTo (object obj)
        {
            return CompareTo (obj as MazeCellEdgeWeight);
        }

        #endregion
    }

    /// <summary>
    /// Direction.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Unknown direction.
        /// </summary>
        Unknown,
        /// <summary>
        /// Horizontal direction.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical direction.
        /// </summary>
        Vertical,
        /// <summary>
        /// Diagonal direction.
        /// </summary>
        Diagonal
    }

    /// <summary>
    /// Labyrinth generator.
    /// </summary>
    public sealed class MazeGenerator : IMessageConsumer
    {
        static readonly uint size = 100;
        static readonly uint scaling = 5;

        #region IMessageConsumer implementation

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            var im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressed("jump"))
                    GenerateMap(size, size);
            }
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.Maze.MazeGenerator"/> class.
        /// </summary>
        /// <param name="objmnr">Object manager.</param>
        /// <param name="scene">Scene.</param>
        /// <param name="msgmnr">Message manager.</param>
        /// <param name="seed">Seed.</param>
        /// <param name="turbulence">Turbulence factor. The higher the more straight will the maze be.</param>
        /// <param name="portalSpawnFactor"> Portal spawn factor. The lower the more portals will be spawned.</param>
        public MazeGenerator (ObjectManager objmnr, CoreScene scene, MessageManager msgmnr, int seed,
            double turbulence = 2, int portalSpawnFactor = 3)
        {
            objectManager = objmnr;
            ValidMessages = new int[]{ (int)MessageId.Input };
            this.scene = scene;
            Turbulence = turbulence;
            PortalSpawnFactor = portalSpawnFactor;
            msgmnr += this;
            rand = new Random (seed);
            Logger.Log.AddLogEntry (LogLevel.Debug, "LabGen", "Seed: {0}", seed);
            CreateMap (size, size);
            DrawMaze (size, size);
        }

        ObjectManager objectManager;

        CoreScene scene;

        WeightedGraph<MazeCell, MazeCellEdgeWeight> graph;

        Random rand;

        RectangleSceneObject[,] rectangles;

        public double Turbulence { get; private set; }

        public int PortalSpawnFactor { get; private set; }

        /// <summary>
        /// Generates the map.
        /// </summary>
        /// <param name="maxX">The x coordinate.</param>
        /// <param name="maxY">The y coordinate.</param>
        public void GenerateMap (uint maxX, uint maxY)
        {
            WeightedNode<MazeCell, MazeCellEdgeWeight> node = null;
            WeightedNode<MazeCell, MazeCellEdgeWeight> lastNode = null;
            Direction lastDirection = Direction.Unknown;
            // set initial edge
            WeightedEdge<MazeCell, MazeCellEdgeWeight> edge = graph.Edges [rand.Next (0, graph.Edges.Count)];

            do
            {
                // save last node
                lastNode = node;

                if (edge != null)
                {
                    // get new node from edge
                    node = edge.FirstNode != node ? edge.FirstNode : edge.SecondNode;

                    // if new node is not a ground set it to ground
                    if (node.Data.MazeType != MazeCellType.Ground)
                    {
                        node.Data.MazeType = MazeCellType.Ground;
                        node.Data.Preview = false;
                    }

                    // set walls surrounding last node to non-preview
                    if (lastNode != null)
                    {
                        lastNode.GetNeighbours ().ForEach ((n, e) =>
                        {
                            if (n.Data.MazeType == MazeCellType.Wall &&
                                !node.GetNeighbours ().Any ((n2, e2) => n == n2 && e2.Weight.Even))
                                n.Data.Preview = false;
                        });
                    }
                    else
                        node.Data.IsSpawn = true;
                }

                // build walls around new node and set correct preview state on those
                node.GetNeighbours ().ForEach ((n, e) =>
                {
                    if (n.Data.MazeType == MazeCellType.Undefined)
                    {
                        n.Data.MazeType = MazeCellType.Wall;
                        n.Data.Preview = true;
                    }
                });

                // bool indicating wether to do backtracking or not
                bool backtrack = false;

                do
                {
                    // get next edge
                    edge = node.GetNeighbours ().Where ((n, e) =>
                        (n.Data.MazeType == MazeCellType.Undefined || n.Data.Preview)
                        && !n.Data.Final && e.Weight.Even).MinElem ((n, e) =>
                            n.Data.Weight * (e.Weight.Direction == lastDirection ? Turbulence : 1 / Turbulence)).Item2;

                    // are we at an dead end? if true set IsPortal randomly
                    node.Data.IsPortal |= !backtrack && edge == null && rand.Next() % PortalSpawnFactor == 0;

                    // backtracking
                    backtrack = false;
                    if (edge == null)
                    {
                        // iterate over all surrounding nodes
                        node.GetNeighbours ().ForEach ((n, e) =>
                        {
                            node.Data.Final = true;
                            // can we go back?
                            if (n.Data.MazeType == MazeCellType.Ground && !n.Data.Final && e.Weight.Even)
                            {
                                // set walls of current node to non-preview to avoid artifacts
                                node.GetNeighbours ().ForEach ((n_old, e_old) => 
                                    n_old.GetNeighbours ().ForEach ((n2_old, e2_old) =>
                                {
                                    if (n2_old.Data.MazeType == MazeCellType.Wall && n2_old.Data.Preview)
                                        n2_old.Data.Preview = false;
                                }));
                                // go to next node
                                node = n;

                                // search for a wall which has undefined cells around it
                                node.GetNeighbours ().FirstOrDefault ((n2, e2) =>
                                {
                                    if (n2.Data.MazeType == MazeCellType.Wall && e2.Weight.Even &&
                                            n2.GetNeighbours ().Any ((n3, e3) =>
                                            n3.Data.MazeType == MazeCellType.Undefined && e3.Weight.Even &&
                                            n3.GetNeighbours ().Count ((n4, e4) =>
                                                n4.Data.MazeType == MazeCellType.Ground) <= 2))
                                    {
                                        n2.Data.Preview = true;
                                        return true;
                                    }
                                    return false;
                                });
                                // if we have not returned yet continue backtracking
                                backtrack = true;
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
            } while (graph.Nodes.Count (n => n.Data.MazeType == MazeCellType.Ground && !n.Data.Final) != 0);

            // set undefined cells to walls
            graph.Nodes.ForEach (n =>
            {
                if (n.Data.MazeType == MazeCellType.Undefined)
                    n.Data.MazeType = MazeCellType.Wall;
            });

            // set exit node
            var borderNodes = graph.Nodes.Where (n => n.Edges.Count < 8 && n.GetNeighbours ().Any (
                (n2, e2) => n2.Data.MazeType == MazeCellType.Ground && e2.Weight.Even)).ToList();
            var exit = borderNodes[rand.Next(0, borderNodes.Count)].Data;
            exit.MazeType = MazeCellType.Ground;
            exit.IsExit = true;
        }

        /// <summary>
        /// Creates the map.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void CreateMap (uint x, uint y)
        {
            rectangles = new RectangleSceneObject[x, y];
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
                    mapnode = new MazeCell (k + "." + i, new Vector2i (i, k), rand.Next (), rectangles);

                    edges.Clear ();

                    if (k > 0)
                        edges.Add (new Pair<WeightedNode<MazeCell, MazeCellEdgeWeight>,
                            MazeCellEdgeWeight> (nodesLast [i], new MazeCellEdgeWeight(true, Direction.Vertical)));

                    if (i > 0)
                        edges.Add (new Pair<WeightedNode<MazeCell, MazeCellEdgeWeight>,
                            MazeCellEdgeWeight> (nodes [i - 1], new MazeCellEdgeWeight (true, Direction.Horizontal)));

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
        }

        /// <summary>
        /// Draws the maze.
        /// </summary>
        /// <param name="maxX">Max x.</param>
        /// <param name="maxY">Max y.</param>
        public void DrawMaze (uint maxX, uint maxY)
        {
            int x = 0;
            int y = 0;

            Vector3 scale = new Vector3 (scaling, scaling, 0);

            foreach (var node in (IEnumerable<WeightedNode<MazeCell, MazeCellEdgeWeight>>) graph)
            {
                rectangles [x, y] = new RectangleSceneObject ();

                rectangles [x, y].Position = new Vector3 (x * scale.X, y * scale.Y, 0.0f);
                rectangles [x, y].Scaling = scale;
                node.Data.Init ();


                if (node.Edges.Count < 8)
                {
                    node.Data.MazeType = MazeCellType.Wall;
                    node.Data.Preview = false;
                    node.Data.Final = true;
                }

                scene.Objects.Add (rectangles [x, y]);

                if (++x >= maxX)
                {
                    x = 0;
                    y++;
                }
            }
        }

        /// <summary>
        /// Writes graph as SVG.
        /// </summary>
        public void WriteAsSVG ()
        {
            var file = new StreamWriter ("graph.dot");

            file.WriteLine ("graph G {");
            foreach (var edge in graph.Edges)
            {
                file.WriteLine ("{0} -- {1}", edge.FirstNode.Data.Name, edge.SecondNode.Data.Name);
            }
            file.WriteLine ("}");

            file.Close ();

            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "/usr/bin/dot",
                    Arguments = "-Tsvg " + Environment.CurrentDirectory + "/graph.dot -o " +
                    Environment.CurrentDirectory + "/graph.svg",
                    CreateNoWindow = true
                }
            };

            proc.Start ();
            proc.WaitForExit ();
            new FileInfo (Environment.CurrentDirectory + "/graph.dot").Delete ();
        }
    }
}
