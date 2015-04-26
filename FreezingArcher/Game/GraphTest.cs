//
//  GraphTest.cs
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
using FreezingArcher.Core;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Output;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Graph tests.
    /// </summary>
    public static class GraphTest
    {
	/// <summary>
	/// The name of the module.
	/// </summary>
	public static readonly string ModuleName = "GraphTest";

	/// <summary>
	/// Test the graphs.
	/// </summary>
	public static void Test()
	{
	    var objectManager = new ObjectManager();
	    var graph = objectManager.CreateOrRecycle<DirectedWeightedGraph<string, uint>>();
	    graph.Init();

	    var node1 = graph.AddNode("Node 1");

	    var node2 = graph.AddNode("Node 2", null, new Pair<DirectedNode<string, uint>, uint>[] {
		new Pair<DirectedNode<string, uint>, uint>(node1, 19)
	    });

	    var node3 = graph.AddNode("Node 3", new Pair<DirectedNode<string, uint>, uint>[] {
		new Pair<DirectedNode<string, uint>, uint>(node2, 12)
	    }, new Pair<DirectedNode<string, uint>, uint>[] {
		new Pair<DirectedNode<string, uint>, uint>(node1, 1)
	    });

	    graph.AddNode("Node 4", new Pair<DirectedNode<string, uint>, uint>[] {
		new Pair<DirectedNode<string, uint>, uint>(node1, 42)
	    }, new Pair<DirectedNode<string, uint>, uint>[] {
		new Pair<DirectedNode<string, uint>, uint>(node3, 4)
	    });

	    foreach (var node in graph.Nodes)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "{0} - outgoing: {1}, incoming {2}", node.Data,
		    node.OutgoingEdges.Count, node.IncomingEdges.Count);

	    foreach (var edge in graph.Edges)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "Edge from {0} to {1} with weight {2}",
		    edge.SourceNode.Data, edge.DestinationNode.Data, edge.Weight);

	    graph.DepthFirstSearch(node1, n => {
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", n.Data);
		return false;
	    });
	}
    }
}
