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
using System.Diagnostics.Tracing;
using System.Threading;
using Pencil.Gaming;
using System.Linq.Expressions;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;

namespace FreezingArcher.Game
{
    public class LabyrinthGenerator : IMessageConsumer
    {
        public enum LabyrinthItemType
        {
            Undefined,
	    Ground,
            Wall
        }

        public class MapNode
        {
            public MapNode(string name, int weight, bool preview = false)
            {
                Weight = weight;
                Preview = preview;
                Name = name;
                Final = false;
            }

            public LabyrinthItemType LabyrinthType;
            public int Weight;
            public bool Preview;
            public bool Final;
            public string Name;
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            var im = msg as InputMessage;
            if (im != null && generationStep != null)
            {
                if (im.IsActionPressed("jump"))
                    generationStep();
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion

        public LabyrinthGenerator (ObjectManager objmnr, MessageManager msgmnr, int seed)
        {
            objectManager = objmnr;
            ValidMessages = new int[]{ (int) MessageId.Input };
            msgmnr += this;
            rand = new Random(seed);
            const uint size = 50;
            GenerateMap(size, size);

            var file = new StreamWriter("graph.txt");
            file.Write(PrintMap(size));
            file.Close();
        }

        ObjectManager objectManager;

        WeightedGraph<MapNode, bool> graph;

        Random rand;

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

        public void GenerateMap(uint x, uint y)
        {
            CreateMap(x, y);

            WeightedNode<MapNode, bool> node = null;
            WeightedEdge<MapNode, bool> edge = graph.Edges[rand.Next(0, graph.Edges.Count)];

            generationStep = () => {
                // get non-visited minimal edge and extract destination from it
                node = edge.FirstNode != node ? edge.FirstNode : edge.SecondNode;
                node.Data.LabyrinthType = LabyrinthItemType.Ground;
                node.Data.Preview = false;

                bool loop;

                do
                {
                    edge = node.Edges.Where (e => {
                        var n = e.FirstNode != node ? e.FirstNode : e.SecondNode;

                        var p = n.Data.Preview;

                        n.Data.Preview = false;

                        if (!e.Weight)
                            return false;
                        
                        var ret = n.Data.LabyrinthType == LabyrinthItemType.Undefined ||
                            (e.Weight && p) || (e.Weight && n.Edges.Any(k => {
                                var n2 = k.FirstNode != n ? k.FirstNode : k.SecondNode;
                                return n2.Data.LabyrinthType == LabyrinthItemType.Undefined;
                            }));//FIXME
                        return ret;
                    }).MinElem (e => e.FirstNode != node ? e.FirstNode.Data.Weight : e.SecondNode.Data.Weight);

                    loop = false;
                    if (edge == null)
                    {
                        foreach(var e in node.Edges)
                        {
                            var n = e.FirstNode != node ? e.FirstNode : e.SecondNode;
                            node.Data.Final = true;
                            if (n.Data.LabyrinthType == LabyrinthItemType.Ground && !n.Data.Final)
                            {
                                node = n;
                                loop = true;
                                break;
                            }
                        }
                    }
                }
                while (loop);
                node.Edges.Except (edge.ToCollection ()).ForEach (e => 
                {
                    var n = e.FirstNode != node ? e.FirstNode : e.SecondNode;
                    if (n.Data.LabyrinthType == LabyrinthItemType.Undefined)
                    {
                        n.Data.LabyrinthType = LabyrinthItemType.Wall;
                        n.Data.Preview |= !e.Weight;
                    }
                });
            };
        }

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
                }

                nodesLast = nodes;
            }
        }

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
