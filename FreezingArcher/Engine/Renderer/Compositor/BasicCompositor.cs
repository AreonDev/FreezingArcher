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

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorEdgeDescription : IComparable
    {
        public CompositorEdgeDescription()
        {
            ID = (int)DateTime.Now.Ticks;
        }

        public int ID {get; private set;}

        public int InputSlotIndex;
        public int OutputSlotIndex;

        public CompositorNode Input;
        public CompositorNode Output;

        public bool Active;

        #region IComparable implementation
        public int CompareTo(object obj)
        {
            return 0;
        }
        #endregion
    }

    public class BasicCompositor
    {
        DirectedWeightedGraph<CompositorNode, List<CompositorEdgeDescription>> _CompositorGraph;
        List<DirectedWeightedNode<CompositorNode, List<CompositorEdgeDescription>>> _Nodes;
        List<DirectedWeightedEdge<CompositorNode, List<CompositorEdgeDescription>>> _Edges;

        object graphLock;

        RendererContext RendererContext;

        public BasicCompositor(ObjectManager objm, RendererContext rc)
        {
            _CompositorGraph = new DirectedWeightedGraph<CompositorNode, List<CompositorEdgeDescription>>();
            _CompositorGraph.Init();

            _Nodes = new List<DirectedWeightedNode<CompositorNode, List<CompositorEdgeDescription>>>();
            _Edges = new List<DirectedWeightedEdge<CompositorNode, List<CompositorEdgeDescription>>>();

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
                DirectedWeightedNode<CompositorNode, List<CompositorEdgeDescription>> gnode = null;

                _Nodes.ForEach(x =>
                    {
                        if (x.Data.ID == node.ID)
                            gnode = x;
                    });

                _CompositorGraph.RemoveNode(x);
            }
        }

        public void AddConnection(CompositorNode begin, CompositorNode end, int begin_slot = 0, int end_slot = 0)
        {
            lock (graphLock)
            {
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
                    
                DirectedWeightedEdge<CompositorNode, List<CompositorEdgeDescription>> gedge = null;

                DirectedWeightedNode<CompositorNode, List<CompositorEdgeDescription>> gnode_begin = null;
                DirectedWeightedNode<CompositorNode, List<CompositorEdgeDescription>> gnode_end = null;

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

                if (gnode_begin.Data.OutputSlots.Length <= begin_slot || gnode_end.Data.InputSlots.Length <= end_slot)
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
                    _Edges.Add(_CompositorGraph.AddEdge(gnode_begin, gnode_end, new List<CompositorEdgeDescription>().Add(desc)));
                else
                    gedge.Weight.Add(desc);
            }
        }

        public DirectedWeightedNode<CompositorNode, List<CompositorEdgeDescription>>[] GetNodes()
        {
            return _Nodes.ToArray();
        }

        public DirectedWeightedEdge<CompositorNode, List<CompositorEdgeDescription>>[] GetEdges()
        {
            return _Edges.ToArray();
        }

        public void DeleteConnection(CompositorEdgeDescription edge)
        {
            lock (graphLock)
            {
                DirectedWeightedEdge<CompositorNode, List<CompositorEdgeDescription>> gedge = null;

                throw new NotImplementedException();
            }
        }

        public void StartCompositing()
        {
            lock (graphLock)
            {
                //Find begin node
                /*
                DirectedWeightedGraph<CompositorNode, List<CompositorEdgeDescription>> gbegin = null;
                _Nodes.ForEach(x =>
                    {
                        if (x.Data.Name == "NodeStart")
                            gbegin = x;
                    });

                if (gbegin == null)
                    return;

                _CompositorGraph.BreadthFirstSearch(*/

                //Simply just go through the list.. and do stuff in order
                Logger.Log.AddLogEntry(LogLevel.Warning, "BasicCompositor", "Graph Iteration is not implemented yet!");
                SimpleGothrough();
            }
        }

        private void SimpleGothrough()
        {
            //Go through edges, hopefully, everything is in the right order
            foreach (DirectedWeightedEdge<CompositorNode, List<CompositorEdgeDescription>> edge in _Edges)
            {
                //Call start node
                edge.SourceNode.Data.Begin(RendererContext);
                edge.SourceNode.Data.Draw(RendererContext);
                edge.SourceNode.Data.End(RendererContext);

                //Change textures
                foreach (CompositorEdgeDescription desc in edge.Weight)
                {
                    edge.DestinationNode.Data.InputSlots[desc.InputSlotIndex].SlotTexture = edge.SourceNode.Data.OutputSlots[desc.OutputSlotIndex];
                }
            }
        }
    }
}

