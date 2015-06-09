//
//  TWeightedGraph.cs
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
using System.Collections;
using System.Linq;

namespace FreezingArcher.DataStructures.Graphs
{
    /// <summary>
    /// Weighted graph.
    /// </summary>
    public sealed class WeightedGraph<TData, TWeight> : FAObject, IEnumerable<WeightedNode<TData, TWeight>>,
    IEnumerable<WeightedEdge<TData, TWeight>>, IEnumerable<TData> where TWeight : IComparable
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        public const string ModuleName = "WeightedGraph";

        /// <summary>
        /// Initialize the graph.
        /// </summary>
        public void Init ()
        {
            if (InternalEdges == null)
                InternalEdges = new List<WeightedEdge<TData, TWeight>>();
            else
                InternalEdges.Clear();

            if (InternalNodes == null)
                InternalNodes = new List<WeightedNode<TData, TWeight>>();
            else
                InternalEdges.Clear();

            if (AsBreadthFirstEnumerable == null)
                AsBreadthFirstEnumerable = new BreadthFirstEnumerable(this);

            if (AsDepthFirstEnumerable == null)
                AsDepthFirstEnumerable = new DepthFirstEnumerable(this);
        }

        /// <summary>
        /// The real edges are stored here for internal use.
        /// </summary>
        List<WeightedEdge<TData, TWeight>> InternalEdges;

        /// <summary>
        /// The real nodes are stored here for internal use.
        /// </summary>
        List<WeightedNode<TData, TWeight>> InternalNodes;

        /// <summary>
        /// Get a read only collection of all registered edges.
        /// </summary>
        /// <value>The edges.</value>
        public ReadOnlyList<WeightedEdge<TData, TWeight>> Edges
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
        public ReadOnlyList<WeightedNode<TData, TWeight>> Nodes
        {
            get
            {
                return InternalNodes;
            }
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public override void Destroy()
        {
            InternalEdges.ForEach (e => e.Destroy());
            InternalNodes.ForEach (n => n.Destroy());
            InternalEdges.Clear();
            InternalNodes.Clear();
            base.Destroy();
        }

        /// <summary>
        /// Add a node to this graph.
        /// </summary>
        /// <returns><c>true</c>, if node was added, <c>false</c> otherwise.</returns>
        /// <param name="data">The data the node should hold.</param>
        /// <param name="edgeNodes">Collection of edges to be created. The pair consists of a
        /// neighbour node and an edge weight.</param>
        public WeightedNode<TData, TWeight> AddNode (TData data,
            ICollection<Pair<WeightedNode<TData, TWeight>, TWeight>> edgeNodes = null)
        {
            // create new node with object recycler
            WeightedNode<TData, TWeight> node = ObjectManager.CreateOrRecycle<WeightedNode<TData, TWeight>> ();

            // initialize new node with data
            node.Init (data);

            // do we have edges?
            if (edgeNodes != null && edgeNodes.Count > 0)
            {
                foreach (var edgeNode in edgeNodes)
                {
                    // does destination node exist? If not adding this node to the graph will fail
                    if (edgeNode.A == null)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                            "Failed to create edge to nonexistent node {0}, skipping...", edgeNode.A);

                        node.Destroy();

                        // failure
                        return null;
                    }

                    // add new edge
                    var edge = AddEdge(node, edgeNode.A, edgeNode.B);

                    if (edge == null)
                    {
                        Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Failed to create edge!");

                        node.Destroy();

                        return null;
                    }
                }
            }

            // add new node to internal node list
            InternalNodes.Add (node);

            return node;
        }

        /// <summary>
        /// Remove the given node by its identifier.
        /// </summary>
        /// <returns><c>true</c>, if node was removed, <c>false</c> otherwise.</returns>
        /// <param name="node">Node identifier.</param>
        public bool RemoveNode (WeightedNode<TData, TWeight> node)
        {
            // print error if remove failed
            if (InternalNodes.Remove(node))
            {
                Logger.Log.AddLogEntry (LogLevel.Warning, ModuleName, "The given node {0} does not exist!", node);
                return false;
            }

            // remove all edges associated with this node
            foreach (var edge in node.Edges)
            {
                if (edge.FirstNode == node)
                    edge.SecondNode.InternalEdges.Remove(edge);
                else
                    edge.FirstNode.InternalEdges.Remove(edge);

                InternalEdges.Remove (edge);
                edge.Destroy();
            }

            // destroy the node
            node.Destroy();

            return true;
        }

        /// <summary>
        /// Adds an edge from a given source node to a given destination node with a given edge weight.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="firstNode">The first node.</param>
        /// <param name="secondNode">The second node.</param>
        /// <param name="weight">The edge weight.</param>
        public WeightedEdge<TData, TWeight> AddEdge (WeightedNode<TData, TWeight> firstNode,
            WeightedNode<TData, TWeight> secondNode, TWeight weight)
        {
            // fail if one of the nodes is null
            if (firstNode == null || secondNode == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Cannot create edge on null node!");
                return null;
            }

            // create new edge with object recycler
            WeightedEdge<TData, TWeight> edge = ObjectManager.CreateOrRecycle<WeightedEdge<TData, TWeight>>();

            // initialize edge with data
            edge.Init(weight, firstNode, secondNode);

            // add created edge to source and destination nodes
            firstNode.InternalEdges.Add(edge);
            secondNode.InternalEdges.Add(edge);

            // add created node to graph
            InternalEdges.Add(edge);

            // everything ok
            return edge;
        }

        /// <summary>
        /// Removes an edge from the graph.
        /// </summary>
        /// <returns><c>true</c>, if edge was removed, <c>false</c> otherwise.</returns>
        /// <param name="edge">The edge.</param>
        public bool RemoveEdge (WeightedEdge<TData, TWeight> edge)
        {
            // fail if edge is null
            if (edge == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ModuleName, "Failed to remove edge as the given edge is null!");
                return false;
            }

            // if source or destination node are null we do really have a problem
            if (edge.FirstNode == null || edge.SecondNode == null)
            {
                Logger.Log.AddLogEntry (LogLevel.Severe, ModuleName,
                    "Detected an edge with referenced nodes that do not exist!" +
                    "This is a severe bug in the graph implementation.");
                return false;
            }

            // remove edge from source and destination nodes
            edge.FirstNode.InternalEdges.Remove(edge);
            edge.SecondNode.InternalEdges.Remove(edge);

            // remove edge from graph
            InternalEdges.Remove(edge);

            // destroy the edge
            edge.Destroy();
            return true;
        }

        #region IEnumerable<Node<TData, TWeight>> implementation

        /// <summary>
        /// Gets the enumerator for nodes.
        /// </summary>
        /// <returns>The node enumerator.</returns>
        IEnumerator<WeightedNode<TData, TWeight>> IEnumerable<WeightedNode<TData, TWeight>>.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        #endregion

        #region IEnumerable<WeightedEdge<TData, TWeight>> implementation

        /// <summary>
        /// Gets the enumerator for edge.
        /// </summary>
        /// <returns>The edge enumerator.</returns>
        IEnumerator<WeightedEdge<TData, TWeight>> IEnumerable<WeightedEdge<TData, TWeight>>.GetEnumerator()
        {
            return Edges.GetEnumerator();
        }

        #endregion

        #region IEnumerable<TData> implementation

        /// <summary>
        /// Gets the enumerator over the data.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
        {
            return Nodes.Select(n => n.Data).GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets the enumerator for nodes.
        /// </summary>
        /// <returns>The node enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Do depth-first-search on this graph.
        /// </summary>
        /// <returns>The node matching the predicate.</returns>
        /// <param name="startNode">Start node.</param>
        /// <param name="predicate">Predicate.</param>
        public WeightedNode<TData, TWeight> DepthFirstSearch (WeightedNode<TData, TWeight> startNode,
            Predicate<WeightedNode<TData, TWeight>> predicate)
        {
            WeightedNode<TData, TWeight> node;
            Stack<WeightedNode<TData, TWeight>> stack = new Stack<WeightedNode<TData, TWeight>>();
            List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();

            if (predicate(startNode))
                return startNode;

            reachedNodes.Add(startNode);
            startNode.Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                if (startNode == e.SecondNode)
                    stack.Push(e.FirstNode);
                else
                    stack.Push(e.SecondNode);
            });

            do
            {
                node = stack.Pop();

                if (!reachedNodes.Contains(node))
                {
                    if (predicate(node))
                        return node;

                    reachedNodes.Add(node);
                    node.Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                        if (node == e.SecondNode)
                            stack.Push(e.FirstNode);
                        else
                            stack.Push(e.SecondNode);
                    });
                }
            } while (stack.Count > 0);

            return null;
        }

        /// <summary>
        /// Do breadth-first-search on this graph.
        /// </summary>
        /// <returns>The node matching the predicate.</returns>
        /// <param name="startNode">Start node.</param>
        /// <param name="predicate">Predicate.</param>
        public WeightedNode<TData, TWeight> BreadthFirstSearch (WeightedNode<TData, TWeight> startNode,
            Predicate<WeightedNode<TData, TWeight>> predicate)
        {
            Queue<WeightedNode<TData, TWeight>> queue = new Queue<WeightedNode<TData, TWeight>>();
            List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();
            WeightedNode<TData, TWeight> node;
            IEnumerable<WeightedNode<TData, TWeight>> children;

            queue.Enqueue(startNode);
            reachedNodes.Add(startNode);

            while (queue.Count > 0)
            {
                node = queue.Dequeue();

                if (predicate(node))
                    return node;

                children = node.Edges.OrderBy(j => j.Weight).Select(
                    e => e.SecondNode == node ? e.FirstNode : e.SecondNode
                );

                foreach (var child in children)
                {
                    if (!reachedNodes.Contains(child))
                    {
                        queue.Enqueue(child);
                        reachedNodes.Add(child);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets an enumerable doing a depth first search through the entire graph.
        /// </summary>
        /// <value>The depth first enumerable.</value>
        public DepthFirstEnumerable AsDepthFirstEnumerable { get; private set; }

        /// <summary>
        /// Gets an enumerable doing a breadth first search through the entire graph.
        /// </summary>
        /// <value>As breadth first enumerable.</value>
        public BreadthFirstEnumerable AsBreadthFirstEnumerable { get; private set; }

        /// <summary>
        /// Depth first enumerable class.
        /// Creates enumerators doing a depth first search through a given weighted graph.
        /// </summary>
        public sealed class DepthFirstEnumerable : IEnumerable<TData>, IEnumerable<WeightedNode<TData, TWeight>>,
        IEnumerable<WeightedEdge<TData, TWeight>>
        {
            readonly WeightedGraph<TData, TWeight> graph;

            /// <summary>
            /// Initializes a new instance of the DepthFirstEnumerable with a given weighted graph.
            /// </summary>
            /// <param name="graph">Graph.</param>
            internal DepthFirstEnumerable(WeightedGraph<TData, TWeight> graph)
            {
                this.graph = graph;
            }

            #region IEnumerable<TData> implementation
            /// <summary>
            /// Gets the enumerator enumerating over the node data.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
            {
                WeightedNode<TData, TWeight> node;
                Stack<WeightedNode<TData, TWeight>> stack = new Stack<WeightedNode<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();

                yield return graph.Nodes[0].Data;

                reachedNodes.Add(graph.Nodes[0]);
                graph.Nodes[0].Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                    if (graph.Nodes[0] == e.SecondNode)
                        stack.Push(e.FirstNode);
                    else
                        stack.Push(e.SecondNode);
                });

                do
                {
                    node = stack.Pop();

                    if (!reachedNodes.Contains(node))
                    {
                        yield return node.Data;

                        reachedNodes.Add(node);
                        node.Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                            if (node == e.SecondNode)
                                stack.Push(e.FirstNode);
                            else
                                stack.Push(e.SecondNode);
                        });
                    }
                } while (stack.Count > 0);
            }
            #endregion

            #region IEnumerable<WeightedNode<TData, TWeight>> implementation
            /// <summary>
            /// Gets the enumerator enumerating over the nodes.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<WeightedNode<TData, TWeight>> IEnumerable<WeightedNode<TData, TWeight>>.GetEnumerator()
            {
                WeightedNode<TData, TWeight> node;
                Stack<WeightedNode<TData, TWeight>> stack = new Stack<WeightedNode<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();

                yield return graph.Nodes[0];

                reachedNodes.Add(graph.Nodes[0]);
                graph.Nodes[0].Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                    if (graph.Nodes[0] == e.SecondNode)
                        stack.Push(e.FirstNode);
                    else
                        stack.Push(e.SecondNode);
                });

                do
                {
                    node = stack.Pop();

                    if (!reachedNodes.Contains(node))
                    {
                        yield return node;

                        reachedNodes.Add(node);
                        node.Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                            if (node == e.SecondNode)
                                stack.Push(e.FirstNode);
                            else
                                stack.Push(e.SecondNode);
                        });
                    }
                } while (stack.Count > 0);
            }
            #endregion

            #region IEnumerable<WeightedEdge<TData, TWeight>> implementation
            /// <summary>
            /// Gets the enumerator enumerating over the edges.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<WeightedEdge<TData, TWeight>> IEnumerable<WeightedEdge<TData, TWeight>>.GetEnumerator()
            {
                WeightedNode<TData, TWeight> node;
                WeightedEdge<TData, TWeight> edge;
                Stack<WeightedNode<TData, TWeight>> stack = new Stack<WeightedNode<TData, TWeight>>();
                Stack<WeightedEdge<TData, TWeight>> estack = new Stack<WeightedEdge<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();

                reachedNodes.Add(graph.Nodes[0]);
                graph.Nodes[0].Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                    estack.Push(e);
                    if (graph.Nodes[0] == e.SecondNode)
                        stack.Push(e.FirstNode);
                    else
                        stack.Push(e.SecondNode);
                });

                do
                {
                    node = stack.Pop();
                    edge = estack.Pop();

                    if (!reachedNodes.Contains(node))
                    {
                        yield return edge;

                        reachedNodes.Add(node);
                        node.Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                            estack.Push(e);
                            if (node == e.SecondNode)
                                stack.Push(e.FirstNode);
                            else
                                stack.Push(e.SecondNode);
                        });
                    }
                } while (stack.Count > 0);
            }
            #endregion

            #region IEnumerable implementation
            /// <summary>
            /// Gets the enumerator enumerating over the nodes.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                WeightedNode<TData, TWeight> node;
                Stack<WeightedNode<TData, TWeight>> stack = new Stack<WeightedNode<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();

                yield return graph.Nodes[0];

                reachedNodes.Add(graph.Nodes[0]);
                graph.Nodes[0].Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                    if (graph.Nodes[0] == e.SecondNode)
                        stack.Push(e.FirstNode);
                    else
                        stack.Push(e.SecondNode);
                });

                do
                {
                    node = stack.Pop();

                    if (!reachedNodes.Contains(node))
                    {
                        yield return node;

                        reachedNodes.Add(node);
                        node.Edges.OrderByDescending(j => j.Weight).ForEach(e => {
                            if (node == e.SecondNode)
                                stack.Push(e.FirstNode);
                            else
                                stack.Push(e.SecondNode);
                        });
                    }
                } while (stack.Count > 0);
            }
            #endregion
        }

        /// <summary>
        /// Breadth first enumerable class.
        /// Creates enumerators doing a breadth first search through a given weighted graph.
        /// </summary>
        public sealed class BreadthFirstEnumerable : IEnumerable<TData>, IEnumerable<WeightedNode<TData, TWeight>>,
        IEnumerable<WeightedEdge<TData, TWeight>>
        {
            readonly WeightedGraph<TData, TWeight> graph;

            /// <summary>
            /// Initializes a new instance of the BreadthFirstEnumerable class with a given weighted graph.
            /// </summary>
            /// <param name="graph">Graph.</param>
            internal BreadthFirstEnumerable(WeightedGraph<TData, TWeight> graph)
            {
                this.graph = graph;
            }

            #region IEnumerable<TData> implementation
            /// <summary>
            /// Gets the enumerator enumerating over node data.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
            {
                Queue<WeightedNode<TData, TWeight>> queue = new Queue<WeightedNode<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();
                WeightedNode<TData, TWeight> node;
                IEnumerable<WeightedNode<TData, TWeight>> children;

                queue.Enqueue(graph.Nodes[0]);
                reachedNodes.Add(graph.Nodes[0]);

                while (queue.Count > 0)
                {
                    node = queue.Dequeue();

                    yield return node.Data;

                    children = node.Edges.OrderBy(j => j.Weight).Select(
                        e => e.SecondNode == node ? e.FirstNode : e.SecondNode
                    );

                    foreach (var child in children)
                    {
                        if (!reachedNodes.Contains(child))
                        {
                            queue.Enqueue(child);
                            reachedNodes.Add(child);
                        }
                    }
                }
            }
            #endregion

            #region IEnumerable<WeightedNode<TData, TWeight>> implementation
            /// <summary>
            /// Gets the enumerator enumerating over the nodes.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<WeightedNode<TData, TWeight>> IEnumerable<WeightedNode<TData, TWeight>>.GetEnumerator()
            {
                Queue<WeightedNode<TData, TWeight>> queue = new Queue<WeightedNode<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();
                WeightedNode<TData, TWeight> node;
                IEnumerable<WeightedNode<TData, TWeight>> children;

                queue.Enqueue(graph.Nodes[0]);
                reachedNodes.Add(graph.Nodes[0]);

                while (queue.Count > 0)
                {
                    node = queue.Dequeue();

                    yield return node;

                    children = node.Edges.OrderBy(j => j.Weight).Select(
                        e => e.SecondNode == node ? e.FirstNode : e.SecondNode
                    );

                    foreach (var child in children)
                    {
                        if (!reachedNodes.Contains(child))
                        {
                            queue.Enqueue(child);
                            reachedNodes.Add(child);
                        }
                    }
                }
            }
            #endregion

            #region IEnumerable<WeightedEdge<TData, TWeight>> implementation
            /// <summary>
            /// Gets the enumerator enumerating over the edges.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<WeightedEdge<TData, TWeight>> IEnumerable<WeightedEdge<TData, TWeight>>.GetEnumerator()
            {
                Queue<WeightedNode<TData, TWeight>> queue = new Queue<WeightedNode<TData, TWeight>>();
                List<WeightedEdge<TData, TWeight>> edges = new List<WeightedEdge<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();
                WeightedNode<TData, TWeight> node;
                IEnumerable<WeightedNode<TData, TWeight>> children;

                queue.Enqueue(graph.Nodes[0]);
                reachedNodes.Add(graph.Nodes[0]);

                while (queue.Count > 0)
                {
                    node = queue.Dequeue();

                    edges.Clear();

                    children = node.Edges.OrderBy(j => j.Weight).Select(e => {
                        edges.Add(e); 
                        return e.SecondNode == node ? e.FirstNode : e.SecondNode;
                    });

                    int i = 0;
                    foreach (var child in children)
                    {
                        if (!reachedNodes.Contains(child))
                        {
                            yield return edges[i];
                            queue.Enqueue(child);
                            reachedNodes.Add(child);
                        }
                        i++;
                    }
                }
            }
            #endregion

            #region IEnumerable implementation
            /// <summary>
            /// Gets the enumerator enumerating over the nodes.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                Queue<WeightedNode<TData, TWeight>> queue = new Queue<WeightedNode<TData, TWeight>>();
                List<WeightedNode<TData, TWeight>> reachedNodes = new List<WeightedNode<TData, TWeight>>();
                WeightedNode<TData, TWeight> node;
                IEnumerable<WeightedNode<TData, TWeight>> children;

                queue.Enqueue(graph.Nodes[0]);
                reachedNodes.Add(graph.Nodes[0]);

                while (queue.Count > 0)
                {
                    node = queue.Dequeue();

                    yield return node;

                    children = node.Edges.OrderBy(j => j.Weight).Select(
                        e => e.SecondNode == node ? e.FirstNode : e.SecondNode
                    );

                    foreach (var child in children)
                    {
                        if (!reachedNodes.Contains(child))
                        {
                            queue.Enqueue(child);
                            reachedNodes.Add(child);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
