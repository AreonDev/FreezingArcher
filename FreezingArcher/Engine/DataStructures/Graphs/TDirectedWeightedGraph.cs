//
//  TDirectedWeightedGraph.cs
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
using FreezingArcher.Core;
using System.Collections.Generic;
using FreezingArcher.Output;

namespace FreezingArcher.DataStructures.Graphs
{
    /// <summary>
    /// Directed weighted graph.
    /// </summary>
    [TypeIdentifier (2)]
    public class DirectedWeightedGraph<TData, TWeight> : FAObject where TWeight : IComparable
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        public const string ModuleName = "DirectedWeightedGraph";

        /// <summary>
        /// Initialize the graph.
        /// </summary>
        public void Init ()
        {
            InternalEdges = new List<Edge<TWeight>>();
            InternalNodes = new List<Node<TData>>();
        }

        /// <summary>
        /// The real edges are stored here for internal use.
        /// </summary>
        protected List<Edge<TWeight>> InternalEdges;

        /// <summary>
        /// The real nodes are stored here for internal use.
        /// </summary>
        protected List<Node<TData>> InternalNodes;

        /// <summary>
        /// Get a read only collection of all registered edges.
        /// </summary>
        /// <value>The edges.</value>
        public IReadOnlyCollection<Edge<TWeight>> Edges
        {
            get
            {
                return InternalEdges;
            }
        }

        /// <summary>
        /// Get a read only collection of all registered nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public IReadOnlyCollection<Node<TData>> Nodes
        {
            get
            {
                return InternalNodes;
            }
        }

        /// <summary>
        /// Add a node to this graph.
        /// </summary>
        /// <returns><c>true</c>, if node was added, <c>false</c> otherwise.</returns>
        /// <param name="data">The data the node should hold.</param>
        /// <param name="outgoingEdgeNodes">Collection of outgoing edges to be created. The pair consists of a
        /// destination node identifier and an edge weight.</param>
        /// <param name="incomingEdgeNodes">Collection of incoming edges to be created. The pair consists of a
        /// source node identifier and an edge weight.</param>
        public virtual bool AddNode (TData data, ICollection<Pair<int, TWeight>> outgoingEdgeNodes,
            ICollection<Pair<int, TWeight>> incomingEdgeNodes)
        {
            // create new node with object recycler
            Node<TData> node = ObjectManager.CreateOrRecycle<Node<TData>> (0);

            // initialize new node with data
            node.Init (data, new List<int>());

            // do we have outgoing edges?
            if (outgoingEdgeNodes != null && outgoingEdgeNodes.Count > 0)
            {
                // list of edges the new node will be assosiated with
                List<Edge<TWeight>> edges = new List<Edge<TWeight>>();

                foreach (var edgeNode in outgoingEdgeNodes)
                {
                    // search for destination node
                    Node<TData> toNode = InternalNodes.Find (n => n.NodeIdentifier == edgeNode.A);

                    // does destination node exist? If not adding this node to the graph will fail
                    if (toNode != null)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                            "Failed to create edge to nonexistent node {0}", edgeNode.A);

                        // cleanup
                        edges.ForEach(e => e.Destroy());
                        edges.Clear();
                        node.Destroy();

                        // failure
                        return false;
                    }

                    // create new edge with object recycler
                    Edge<TWeight> edge = ObjectManager.CreateOrRecycle<Edge<TWeight>> (1);

                    // add created edge to edges list, later on we may add those to InternalEdges or
                    // clean them up on failure
                    edges.Add (edge);

                    // initialize created edge with data
                    edge.Init (edgeNode.B, node.NodeIdentifier, edgeNode.A, true);

                    // add created edge to source and destination node (only by identifier)
                    node.InternalEdges.Add (edge.EdgeIdentifier);
                    toNode.InternalEdges.Add (edge.EdgeIdentifier);
                }

                // add all created edges to graph
                InternalEdges.AddRange (edges);

                // cleanup (no really necessary)
                edges.Clear();
            }

            // do we have incoming edges?
            if (incomingEdgeNodes != null && incomingEdgeNodes.Count > 0)
            {
                // list of edges the new node will be assosiated with
                List<Edge<TWeight>> edges = new List<Edge<TWeight>>();

                foreach (var edgeNode in incomingEdgeNodes)
                {
                    // search for source node
                    Node<TData> fromNode = InternalNodes.Find (n => n.NodeIdentifier == edgeNode.A);

                    // does source node exist? If not adding this node to the graph will fail
                    if (fromNode != null)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                            "Failed to create edge from nonexistent node {0}", edgeNode.A);

                        // cleanup
                        edges.ForEach(e => e.Destroy());
                        edges.Clear();
                        node.Destroy();

                        // failure
                        return false;
                    }

                    // create new edge with object recycler
                    Edge<TWeight> edge = ObjectManager.CreateOrRecycle<Edge<TWeight>> (1);

                    // add created edge to edges list, later on we may add those to InternalEdges or
                    // clean them up on failure
                    edges.Add (edge);

                    // initialize created edge with data
                    edge.Init (edgeNode.B, edgeNode.A, node.NodeIdentifier, true);

                    // add created edge to source and destination node (only by identifier)
                    node.InternalEdges.Add (edge.EdgeIdentifier);
                    fromNode.InternalEdges.Add (edge.EdgeIdentifier);
                }

                // add all created edges to graph
                InternalEdges.AddRange (edges);

                // cleanup (no really necessary)
                edges.Clear();
            }

            // add new node to internal node list
            InternalNodes.Add (node);

            return true;
        }

        /// <summary>
        /// Remove the given node by its identifier.
        /// </summary>
        /// <returns><c>true</c>, if node was removed, <c>false</c> otherwise.</returns>
        /// <param name="node">Node identifier.</param>
        public virtual bool RemoveNode (int node)
        {
            // remove the node itself
            int removedNodes = InternalNodes.RemoveAll (n => {
                if (n.NodeIdentifier == node)
                {
                    n.Destroy();
                    return true;
                }
                return false;
            });

            // fail if we have not removed any node
            if (removedNodes < 1)
            {
                Logger.Log.AddLogEntry (LogLevel.Warning, ModuleName, "The given Node {0} does not exist!", node);
                return false;
            }

            // remove all edges associated with this node
            InternalEdges.RemoveAll (e => {
                // if we found the start node of this edge we need to remove this edge from the end node
                if (e.StartNode == node)
                {
                    // get end node of this edge
                    Node<TData> endNode = InternalNodes.Find(n => n.NodeIdentifier == e.EndNode);

                    // remove this edge from the end node as this edge will be removed from the graph
                    endNode.InternalEdges.RemoveAll (endEdge => endEdge == e.EdgeIdentifier);

                    // now remove this edge
                    e.Destroy();
                    return true;
                }

                // if we found the end node of this edge we need to remove this edge from the start node
                if (e.EndNode == node)
                {
                    // get start node of this edge
                    Node<TData> startNode = InternalNodes.Find(n => n.NodeIdentifier == e.StartNode);

                    // remove this edge from the start node as this edge will be removed from the graph
                    startNode.InternalEdges.RemoveAll (startEdge => startEdge == e.EdgeIdentifier);

                    // now remove this edge
                    e.Destroy();
                    return true;
                }

                // this edge does not belong to the given node and do not need to be removed
                return false;
            });

            // everything ok
            return true;
        }

        /// <summary>
        /// Adds an edge from a given source node to a given destination node with a given edge weight.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="fromNodeIdentifier">Identifier identifying the source node.</param>
        /// <param name="toNodeIdentifier">Identifier identifying the destination node.</param>
        /// <param name="weight">The edge weight.</param>
        public bool AddEdge (int fromNodeIdentifier, int toNodeIdentifier, TWeight weight)
        {
            // search for real nodes by identifiers
            Node<TData> fromNode = InternalNodes.Find (n => n.NodeIdentifier == fromNodeIdentifier);
            Node<TData> toNode = InternalNodes.Find (n => n.NodeIdentifier == toNodeIdentifier);

            // fail if one of the nodes does not exist
            if (fromNode == null || toNode == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName,
                    "Failed to create edge as one the given nodes [{0}, {1}] does not exist!",
                    fromNodeIdentifier, toNodeIdentifier);
                return false;
            }

            // create new edge with object recycler
            Edge<TWeight> edge = ObjectManager.CreateOrRecycle<Edge<TWeight>>(1);

            // initialize edge with data
            edge.Init(weight, fromNode.NodeIdentifier, toNode.NodeIdentifier, true);

            // add created edge to source and destination nodes
            fromNode.InternalEdges.Add (edge.EdgeIdentifier);
            toNode.InternalEdges.Add (edge.EdgeIdentifier);

            // add created node to graph
            InternalEdges.Add (edge);

            // everything ok
            return true;
        }

        /// <summary>
        /// Removes an edge identified by a given edge identifier.
        /// </summary>
        /// <returns><c>true</c>, if edge was removed, <c>false</c> otherwise.</returns>
        /// <param name="edgeIdentifier">The edge identifier.</param>
        public bool RemoveEdge (int edgeIdentifier)
        {
            // search for real edge from identifier
            Edge<TWeight> edge = InternalEdges.Find (e => e.EdgeIdentifier == edgeIdentifier);

            // fail if edge does not exist
            if (edge == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName,
                    "Failed to remove edge as the given edge {0} does not exist!", edgeIdentifier);
                return false;
            }

            Node<TData> startNode = InternalNodes.Find (n => n.NodeIdentifier == edge.StartNode);
            Node<TData> endNode = InternalNodes.Find (n => n.NodeIdentifier == edge.EndNode);

            // if start or end node does not exist we do really have a problem
            if (startNode == null || endNode == null)
            {
                Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                    "Detected an edge with referenced nodes that do not exist!" +
                    "This is a severe bug in the graph implementation.");
                return false;
            }

            // remove edge from start and end nodes
            startNode.InternalEdges.RemoveAll (e => e == edgeIdentifier);
            endNode.InternalEdges.RemoveAll (e => e == edgeIdentifier);

            // remove edge from graph
            InternalEdges.RemoveAll (e => e.EdgeIdentifier == edgeIdentifier);

            // destroy the edge
            edge.Destroy();
            return true;
        }
    }
}
