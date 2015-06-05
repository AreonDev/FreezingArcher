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

namespace FreezingArcher.Game
{
    public static class Extensions
    {
        public static IEnumerable<WeightedNode<TData, TWeight>> GetNeighbours<TData, TWeight>(this WeightedNode<TData, TWeight> node)
            where TWeight : IComparable
        {
            foreach (var e in node.Edges)
                yield return e.FirstNode != node ? e.FirstNode : e.SecondNode;
        }
    }

    /// <summary>
    /// Map node.
    /// </summary>
    public class MapNode
    {
        /// <summary>
        /// Initializes a new instance of the MapNode class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="preview">If set to <c>true</c> preview.</param>
        public MapNode(string name, int weight, bool preview = false)
        {
            Weight = weight;
            Preview = preview;
            Name = name;
            Final = false;
        }

        /// <summary>
        /// The type of the labyrinth item.
        /// </summary>
        public LabyrinthItemType LabyrinthType;

        /// <summary>
        /// The weight.
        /// </summary>
        public int Weight;

        /// <summary>
        /// The preview flag.
        /// </summary>
        public bool Preview;

        /// <summary>
        /// The final flag.
        /// </summary>
        public bool Final;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The position.
        /// </summary>
        public Vector2i Position;
    }

    /// <summary>
    /// Labyrinth item type.
    /// </summary>
    public enum LabyrinthItemType
    {
        /// <summary>
        /// The undefined item.
        /// </summary>
        Undefined,
        /// <summary>
        /// The ground item.
        /// </summary>
        Ground,
        /// <summary>
        /// The wall item.
        /// </summary>
        Wall
    }

    /// <summary>
    /// Labyrinth generator.
    /// </summary>
    public class LabyrinthGenerator : IMessageConsumer
    {
        #region IMessageConsumer implementation

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            var im = msg as InputMessage;
            if (im != null && generationStep != null)
            {
                if (im.IsActionPressed("jump"))
                    generationStep();
            }
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.LabyrinthGenerator"/> class.
        /// </summary>
        /// <param name="objmnr">Object manager.</param>
        /// <param name="scene">Scene.</param>
        /// <param name="msgmnr">Message manager.</param>
        /// <param name="seed">Seed.</param>
        public LabyrinthGenerator (ObjectManager objmnr, CoreScene scene, MessageManager msgmnr, int seed)
        {
            objectManager = objmnr;
            ValidMessages = new int[]{ (int) MessageId.Input };
            this.scene = scene;
            msgmnr += this;
            rand = new Random(seed);
            const uint size = 100;
            GenerateMap(size, size);

            Logger.Log.AddLogEntry(LogLevel.Debug, "LabGen", "Seed: {0}", seed);

            /*var file = new StreamWriter("graph.txt");
            file.Write(PrintMap(size));
            file.Close();*/
        }

        ObjectManager objectManager;

        CoreScene scene;

        WeightedGraph<MapNode, bool> graph;

        Random rand;

        RectangleSceneObject[,] rectangles;

        /// <summary>
        /// Draws the labyrinth.
        /// </summary>
        /// <param name="maxX">Max x.</param>
        /// <param name="maxY">Max y.</param>
        public void DrawLabyrinth(uint maxX, uint maxY)
        {
            rectangles = new RectangleSceneObject[maxX, maxY];
            int x = 0;
            int y = 0;

            Vector3 scale = new Vector3(4, 4, 0);

            foreach(var node in (IEnumerable<MapNode>) graph)
            {
                rectangles[x, y] = new RectangleSceneObject ();

                rectangles[x, y].Position = new Vector3 (x * scale.X, y * scale.Y, 0.0f);
                rectangles[x, y].Scaling = scale;
                rectangles[x, y].Color = Color4.Fuchsia;

                scene.Objects.Add (rectangles[x, y]);

                if (++x >= maxX)
                {
                    x = 0;
                    y++;
                }
            }
        }

        /// <summary>
        /// Prints the map.
        /// </summary>
        /// <returns>The map.</returns>
        /// <param name="x">The x coordinate.</param>
        public string PrintMap(uint x)
        {
            int cnt = 0;
            string s = "";
            foreach(var node in (IEnumerable<MapNode>) graph)
            {
                switch (node.LabyrinthType)
                {
                case LabyrinthItemType.Ground:
                    s += ' ';
                    break;
                case LabyrinthItemType.Wall:
                    s += '#';
                    break;
                default:
                    s += '?';
                    break;
                }
                if (++cnt >= x)
                {
                    s += '\n';
                    cnt = 0;
                }
            }
            return s;
        }

        delegate void GenerationStep();
        GenerationStep generationStep;

        /// <summary>
        /// Generates the map.
        /// </summary>
        /// <param name="maxX">The x coordinate.</param>
        /// <param name="maxY">The y coordinate.</param>
        public void GenerateMap(uint maxX, uint maxY)
        {
            CreateMap(maxX, maxY);

            DrawLabyrinth(maxX, maxY);

            WeightedNode<MapNode, bool> node = null;
            WeightedNode<MapNode, bool> lastNode = null;
            WeightedEdge<MapNode, bool> edge = graph.Edges[rand.Next(0, graph.Edges.Count)];

            //generationStep = () => {
            do
            {
                lastNode = node;

                if (edge != null)
                {
                    node = edge.FirstNode != node ? edge.FirstNode : edge.SecondNode;

                    if (node.Data.LabyrinthType != LabyrinthItemType.Ground)
                    {
                        node.Data.LabyrinthType = LabyrinthItemType.Ground;
                        node.Data.Preview = false;
                        rectangles[node.Data.Position.X, node.Data.Position.Y].Color = Color4.WhiteSmoke;
                    }

                    if (lastNode != null)
                    {
                        lastNode.Edges.ForEach(e => {
                            var n = e.FirstNode != lastNode ? e.FirstNode : e.SecondNode;

                            if (n.Data.LabyrinthType == LabyrinthItemType.Wall && !node.Edges.Any(e2 => {
                                var n2 = e2.FirstNode != node ? e2.FirstNode : e2.SecondNode;
                                return n == n2 && e2.Weight;
                            }))
                            {
                                n.Data.Preview = false;
                                rectangles[n.Data.Position.X, n.Data.Position.Y].Color = Color4.Chocolate;
                            }
                        });
                    }
                }

                node.Edges.ForEach (e => 
                {
                    var n = e.FirstNode != node ? e.FirstNode : e.SecondNode;
                    if (n.Data.LabyrinthType == LabyrinthItemType.Undefined)
                    {
                        n.Data.LabyrinthType = LabyrinthItemType.Wall;
                        n.Data.Preview = true;
                    }
                    if (n.Data.LabyrinthType == LabyrinthItemType.Wall)
                    {   
                        if (n.Data.Preview)
                            rectangles[n.Data.Position.X, n.Data.Position.Y].Color = Color4.DarkGoldenrod;
                        else
                            rectangles[n.Data.Position.X, n.Data.Position.Y].Color = Color4.Chocolate;
                    }
                });

                bool loop;

                do
                {
                    edge = node.Edges.Where (e => {
                        var n = e.FirstNode != node ? e.FirstNode : e.SecondNode;

                        if (!e.Weight || n.Data.Final)
                            return false;

                        return n.Data.LabyrinthType == LabyrinthItemType.Undefined || n.Data.Preview;
                    }).MinElem (e => e.FirstNode != node ? e.FirstNode.Data.Weight : e.SecondNode.Data.Weight);

                    loop = false;
                    if (edge == null)
                    {
                        foreach(var e in node.Edges)
                        {
                            var n = e.FirstNode != node ? e.FirstNode : e.SecondNode;
                            node.Data.Final = true;
                            if (n.Data.LabyrinthType == LabyrinthItemType.Ground && !n.Data.Final && e.Weight)
                            {
                                node.Edges.ForEach(e_old => {
                                    var n_old = e_old.FirstNode != node ? e_old.FirstNode : e_old.SecondNode;
                                    n_old.Edges.ForEach(e2_old => {
                                        var n2_old = e2_old.FirstNode != n_old ? e2_old.FirstNode : e2_old.SecondNode;
                                        if (n2_old.Data.LabyrinthType == LabyrinthItemType.Wall && n2_old.Data.Preview)
                                        {
                                            n2_old.Data.Preview = false;
                                            rectangles[n2_old.Data.Position.X, n2_old.Data.Position.Y].Color = Color4.Chocolate;
                                        }
                                    });
                                });
                                node = n;
                                node.Edges.FirstOrDefault(e2 => {
                                    var n2 = e2.FirstNode != node ? e2.FirstNode : e2.SecondNode;

                                    if (n2.Data.LabyrinthType == LabyrinthItemType.Wall && e2.Weight &&
                                        n2.Edges.Any(e3 => {
                                            var n3 = e3.FirstNode != n2 ? e3.FirstNode : e3.SecondNode;
                                            bool tmp = n3.Edges.Count (e4 => 
                                            {
                                                var n4 = e4.FirstNode != n3 ? e4.FirstNode : e4.SecondNode;
                                                return n4.Data.LabyrinthType == LabyrinthItemType.Ground;
                                            }) <= 2;
                                            return n3.Data.LabyrinthType == LabyrinthItemType.Undefined && e3.Weight && tmp;
                                        }))
                                    {
                                        n2.Data.Preview = true;
                                        return true;
                                    }
                                    return false;
                                });
                                loop = true;
                                break;
                            }
                        }
                    }
                }
                while (loop);
            }
            while (graph.Nodes.Count(n => n.Data.LabyrinthType == LabyrinthItemType.Ground && !n.Data.Final) != 0);

            graph.Nodes.ForEach(n => {
                if (n.Data.LabyrinthType == LabyrinthItemType.Undefined)
                {
                    n.Data.LabyrinthType = LabyrinthItemType.Wall;
                    rectangles[n.Data.Position.X, n.Data.Position.Y].Color = Color4.Chocolate;
                }
            });
        }

        /// <summary>
        /// Creates the map.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void CreateMap(uint x, uint y)
        {
            graph = objectManager.CreateOrRecycle<WeightedGraph<MapNode, bool>>();
            graph.Init();

            var nodesLast = new WeightedNode<MapNode, bool>[x];
            MapNode mapnode;
            var edges = new List<Pair<WeightedNode<MapNode, bool>, bool>>();

            for (int k = 0; k < y; k++)
            {
                var nodes = new WeightedNode<MapNode, bool>[x];

                for (int i = 0; i < x; i++)
                {
                    mapnode = new MapNode(k + "." + i, rand.Next());

                    edges.Clear();

                    if (k > 0) edges.Add(new Pair<WeightedNode<MapNode, bool>, bool>(nodesLast[i], true));

                    if (i > 0) edges.Add(new Pair<WeightedNode<MapNode, bool>, bool>(nodes[i - 1], true));

                    if (i > 0 && k > 0)
                    {
                        edges.Add(new Pair<WeightedNode<MapNode, bool>, bool>(nodesLast[i - 1], false));
                        graph.AddEdge(nodesLast[i], nodes[i - 1], false);
                    }

                    nodes [i] = graph.AddNode (mapnode, edges);
                    nodes [i].Data.Position = new Vector2i(i, k);
                }

                nodesLast = nodes;
            }
        }

        /// <summary>
        /// Writes as SV.
        /// </summary>
        public void WriteAsSVG()
        {
            var file = new StreamWriter("graph.dot");

            file.WriteLine("graph G {");
            foreach (var edge in graph.Edges)
            {
                file.WriteLine("{0} -- {1}", edge.FirstNode.Data.Name, edge.SecondNode.Data.Name);
            }
            file.WriteLine("}");

            file.Close();

            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "/usr/bin/dot",
                    Arguments = "-Tsvg " + Environment.CurrentDirectory + "/graph.dot -o " +
                        Environment.CurrentDirectory + "/graph.svg",
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();
            new FileInfo(Environment.CurrentDirectory + "/graph.dot").Delete();
        }
    }
}
