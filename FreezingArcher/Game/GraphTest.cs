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
using System.Collections;
using System.Collections.Generic;

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
	    Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "Starting graph tests...");

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

	    graph.BreadthFirstSearch(node1, n => {
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "BFS PREDICATE# {0}", n.Data);
		return false;
	    });

	    graph.DepthFirstSearch(node1, n => {
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "DFS PREDICATE# {0}", n.Data);
		return false;
	    });

	    foreach (var n in (IEnumerable<DirectedNode<string, uint>>) graph)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "FOR#   Node: {0}", n.GetHashCode());

	    foreach (var s in (IEnumerable<string>) graph)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "FOR#   Data: {0}", s);

	    foreach (var e in (IEnumerable<DirectedEdge<string, uint>>) graph)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "FOR# Weight: {0}", e.Weight);

	    foreach (var n in (IEnumerable<DirectedNode<string, uint>>) graph.AsBreadthFirstEnumerable)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "BFS#   Node: {0}", n.GetHashCode());

	    foreach (var s in (IEnumerable<string>) graph.AsBreadthFirstEnumerable)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "BFS#   Data: {0}", s);

	    foreach (var e in (IEnumerable<DirectedEdge<string, uint>>) graph.AsBreadthFirstEnumerable)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "BFS# Weight: {0}", e.Weight);

	    foreach (var n in (IEnumerable<DirectedNode<string, uint>>) graph.AsDepthFirstEnumerable)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "DFS#   Node: {0}", n.GetHashCode());

	    foreach (var s in (IEnumerable<string>) graph.AsDepthFirstEnumerable)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "DFS#   Data: {0}", s);

	    foreach (var e in (IEnumerable<DirectedEdge<string, uint>>) graph.AsDepthFirstEnumerable)
		Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "DFS# Weight: {0}", e.Weight);

	    Logger.Log.AddLogEntry(LogLevel.Debug, "GraphTest", "Graph tests finished!");
	}
    }
}
