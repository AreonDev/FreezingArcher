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
        DirectedWeightedGraph<CompositorNode, CompositorEdgeDescription> _CompositorGraph;
        List<DirectedWeightedNode<CompositorNode, CompositorEdgeDescription>> _Nodes;
        List<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescription>> _Edges;

        object graphLock;

        public BasicCompositor(ObjectManager objm)
        {
            _CompositorGraph = new DirectedWeightedGraph<CompositorNode, CompositorEdgeDescription>();
            _CompositorGraph.Init();

            _Nodes = new List<DirectedWeightedNode<CompositorNode, CompositorEdgeDescription>>();
            _Edges = new List<DirectedWeightedEdge<CompositorNode, CompositorEdgeDescription>>();
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
                DirectedWeightedNode<CompositorNode, CompositorEdgeDescription> gnode = null;

                _Nodes.ForEach(x => {if(x.Data.ID == node.ID) gnode = x;});

                _Nodes.Remove(gnode);
                _CompositorGraph.RemoveNode(gnode);
            }
        }

        public void AddConnection(CompositorNode begin, CompositorNode end, int begin_slot = 0, int end_slot = 0)
        {
            lock (graphLock)
            {
                CompositorEdgeDescription desc = new CompositorEdgeDescription();
                desc.InputSlotIndex = begin_slot;
                desc.OutputSlotIndex = end_slot;
                desc.Active = true;
                desc.Input = begin;
                desc.Output = end;

                if (begin == null || end == null)
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "BasicCompositor", Status.Meh);
                    return;
                }
                    
                DirectedWeightedNode<CompositorNode, CompositorEdgeDescription> gnodestart = null;
                _Nodes.ForEach(x => {if (x.Data.ID == begin.ID) gnodestart = x;});

                DirectedWeightedNode<CompositorNode, CompositorEdgeDescription> gnodeend = null;
                _Nodes.ForEach(x => {if (x.Data.ID == end.ID) gnodeend = x;});

                _Edges.Add(_CompositorGraph.AddEdge(gnodestart, gnodeend, desc));
            }
        }

        public DirectedWeightedNode<CompositorNode, CompositorEdgeDescription>[] GetNodes()
        {
            return _Nodes.ToArray();
        }

        public DirectedWeightedEdge<CompositorNode, CompositorEdgeDescription>[] GetEdges()
        {
            return _Edges.ToArray();
        }

        public void DeleteConnection(CompositorEdgeDescription edge)
        {
            lock (graphLock)
            {
                DirectedWeightedEdge<CompositorNode, CompositorEdgeDescription> gedge = null;

                _Edges.ForEach(x =>
                    {
                        if (x.Weight.ID == edge.ID)
                            gedge = x;
                    });

                _Edges.Remove(gedge);
                _CompositorGraph.RemoveEdge(gedge);
            }
        }
    }
}

