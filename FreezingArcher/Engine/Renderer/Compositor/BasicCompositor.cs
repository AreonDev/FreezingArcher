//
//  BasicCompositor.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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
using System.Collections.Generic;

using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Core;
using FreezingArcher.Output;

using FreezingArcher.Output;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorEdgeDescriptionListWrapper : IComparable
    {
        public CompositorEdgeDescriptionListWrapper()
        {
            Descriptions = new List<CompositorEdgeDescription>();
        }

        public List<CompositorEdgeDescription> Descriptions;

        #region IComparable implementation
        public int CompareTo(object obj)
        {
            return 0;
        }
        #endregion
    }

    public class CompositorEdgeDescription
    {
        public CompositorEdgeDescription()
        {
            ID = DateTime.Now.Ticks;
        }

        public long ID {get; private set;}

        public int InputSlotIndex;
        public int OutputSlotIndex;

        public CompositorNode Input;
        public CompositorNode Output;

        public bool Active;
    }

    public class BasicCompositor
    {
        DirectedWeightedGraph<CompositorNode, CompositorEdgeDescriptionListWrapper> _CompositorGraph;
        List<DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper>> _Nodes;
        List<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper>> _Edges;
        DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper> DummyNode;

        object graphLock;

        RendererContext RendererContext;

        public BasicCompositor(ObjectManager objm, RendererContext rc)
        {
            graphLock = new object();

            _CompositorGraph = objm.CreateOrRecycle<DirectedWeightedGraph<CompositorNode, CompositorEdgeDescriptionListWrapper>>();
            _CompositorGraph.Init();

            _Nodes = new List<DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper>>();
            _Edges = new List<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper>>();

            DummyNode = _CompositorGraph.AddNode(null);

            RendererContext = rc;
        }

        public void AddNode(CompositorNode node)
        {
            lock (graphLock)
            {
                node.Active = true;
                _Nodes.Add(_CompositorGraph.AddNode(node));
            }
        }

        public void DeleteNode(CompositorNode node)
        {
            lock (graphLock)
            {
                DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper> gnode = null;

                _Nodes.ForEach(x =>
                    {
                        if (x.Data.ID == node.ID)
                            gnode = x;
                    });

                _CompositorGraph.RemoveNode(gnode);
            }
        }

        public void AddConnection(CompositorNode begin, CompositorNode end, int begin_slot = 0, int end_slot = 0)
        {
            lock (graphLock)
            {                
                //This fuck is fucking crazy.... 
                CompositorEdgeDescription desc = new CompositorEdgeDescription();
                desc.InputSlotIndex = end_slot;
                desc.OutputSlotIndex = begin_slot;
                desc.Active = true;
                desc.Input = begin;
                desc.Output = end;
               
                if (begin == null || end == null)
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "BasicCompositor", Status.Meh);
                    return;
                }
                    
                DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper> gedge = null;

                DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper> gnode_begin = null;
                DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper> gnode_end = null;

                //Find begin and end node
                _Nodes.ForEach(x => {if(x.Data.ID == begin.ID) gnode_begin = x;});
                _Nodes.ForEach(x =>
                    {
                        if (x.Data.ID == end.ID)
                            gnode_end = x;
                    });

                if (gnode_begin == null || gnode_end == null)
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "BasicCompositor", Status.BadArgument);
                    return;
                }



                if ((gnode_begin.Data.OutputSlots != null && gnode_begin.Data.OutputSlots.Length <= begin_slot) || 
                    (gnode_end.Data.InputSlots != null && gnode_end.Data.InputSlots.Length <= end_slot))
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "BasicCompositor", Status.BadArgument);
                    return;
                }
                    
                //Is there an edge with this nodes?
                _Edges.ForEach(x =>
                    {
                        if(x.SourceNode.InstId == gnode_begin.InstId && x.DestinationNode.InstId == gnode_end.InstId)
                            gedge = x;
                    });

                if (gedge == null)
                {
                    CompositorEdgeDescriptionListWrapper wrap = new CompositorEdgeDescriptionListWrapper();
                    wrap.Descriptions.Add(desc);
                    _Edges.Add(_CompositorGraph.AddEdge(gnode_begin, gnode_end, wrap));
                }
                else
                    gedge.Weight.Descriptions.Add(desc);
            }
        }

        public DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper>[] GetNodes()
        {
            return _Nodes.ToArray();
        }

        public DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper>[] GetEdges()
        {
            return _Edges.ToArray();
        }

        public void DeleteConnection(CompositorEdgeDescription edge)
        {
            lock (graphLock)
            {
                DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper> gedge = null;

                throw new NotImplementedException();
            }
        }

        public void StartCompositing()
        {
            lock (graphLock)
            {
                //Connect all StartNodes with dummy nodes
                List<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper>> addededges = new List<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper>>();
                foreach (DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper> node in _Nodes)
                {
                    if (node.Data.Name == "NodeStart")
                        addededges.Add(_CompositorGraph.AddEdge(DummyNode, node, null));
                }

                foreach (var edge in (IEnumerable<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescriptionListWrapper>>) _CompositorGraph.AsBreadthFirstEnumerable)
                {
                    if (edge.Weight != null)
                    {
                        Console.WriteLine("StartNode: " + edge.SourceNode.Data.Name + " EndNode: " + edge.DestinationNode.Data.Name);

                        if (!edge.SourceNode.Data.Rendered)
                        {
                            edge.SourceNode.Data.Begin();
                            edge.SourceNode.Data.Draw();
                            edge.SourceNode.Data.End();
                        }

                        foreach (CompositorEdgeDescription desc in edge.Weight.Descriptions)
                        {
                            desc.Output.InputSlots[desc.InputSlotIndex].SlotTexture = desc.Input.OutputSlots[desc.OutputSlotIndex].SlotTexture;
                        }
                    }
                }

                //Disconnect all StartNodes with dummy node
                addededges.ForEach(x => _CompositorGraph.RemoveEdge(x));

                //Are there some nodes, which are not rendered?
                foreach (DirectedWeightedNode<CompositorNode, CompositorEdgeDescriptionListWrapper> node in _Nodes)
                {
                    if (!node.Data.Rendered)
                    {
                        node.Data.Begin();
                        node.Data.Draw();
                        node.Data.End();
                    }
                }
                    
                //Reset all nodes
                _Nodes.ForEach(x => x.Data.Reset());
            }
        }
    }
}

