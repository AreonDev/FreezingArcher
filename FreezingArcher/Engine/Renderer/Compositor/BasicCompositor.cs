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

using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Core;
using FreezingArcher.Output;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorEdgeDescription : IComparable
    {
        public int InputSlotIndex;
        public int OutputSlotIndex;

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
        object graphLock;

        public BasicCompositor(ObjectManager objm)
        {
            _CompositorGraph = new DirectedWeightedGraph<CompositorNode, CompositorEdgeDescription>();
            _CompositorGraph.Init();


        }

        public void AddNode(CompositorNode node)
        {
            lock (graphLock)
            {
                node.Active = true;
                _CompositorGraph.AddNode(node);
            }
        }

        public void DeleteNode(CompositorNode node)
        {
            lock (graphLock)
            {
                //_CompositorGraph.RemoveNode
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

                if (begin == null || end == null)
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "BasicCompositor", Status.Meh);
                    return;
                }
                    
                //_CompositorGraph.AddEdge(begin, end, desc);
            }
        }

        public void DeleteConnection()
        {
            
        }
    }
}

