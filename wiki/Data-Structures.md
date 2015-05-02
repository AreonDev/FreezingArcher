---
layout: wikipage
title: Data Structures
wikiPageName: Data-Structures
menu: wiki
---

```c#
namespace FreezingArcher.DataStructures
```

The FreezingArcher framework ships with its own data structures including the `ReadOnlyList`, graphs and trees.

## Read only list

The `ReadOnlyList` is used to lock lists and make them read-only. Array operators (`[]`) are available and an implicit
cast from `System.Collections.Generic.List` exists.

## Graphs

FreezingArcher has 4 different types of graphs: `Graph`, `WeightedGraph`, `DirectedGraph` and `DirectedWeightedGraph`.
A graph consists of nodes and edges. Data of a generic type may be stored in the nodes. Edges could have a generic 
weight aplied. The weight type must be a `IComparable`.

### Operations on graphs

The graph may be manipulated with the public functions in the graph class only. All operations on node and edge classes
may cause in a fatal error.

#### Adding nodes

```c#
Node<DataType, WeightType> node = graph.AddNode(
    // data the node should hold
    data,
    // outgoing edges
    new Pair<Node<DataType, WeightType>, WeightType>[] {
        new Pair<Node<DataType, WeightType>, WeightType>(destinationNode, edgeWeight1)
    },
    // incoming edges
    new Pair<Node<DataType, WeightType>, WeightType>[] {
        new Pair<Node<DataType, WeightType>, WeightType>(sourceNode, edgeWeight2)
    }
);
```

You may pass `null` as edge list for no edges.

#### Adding edges

```c#
Edge<DataType, WeightType> edge = graph.AddEdge(
    sourceNode,
    destinationNode,
    edgeWeight
);
```

#### Removing nodes

```c#
if (graph.RemoveNode(node))
    // remove successful
else
    // remove failed
```

#### Removing edges

```c#
if (graph.RemoveEdge(edge))
    // remove successful
else
    // remove failed
```

#### Searching in graphs

##### Searching via breadth-first-search

```c#
var node = graph.BreadthFirstSearch(startNode, n => n == goal);
```

##### Searching via depth-first-search

```c#
var node = graph.DepthFirstSearch(startNode, n => n == goal);
```

#### Enumerating over graphs

```c#
foreach (var n in (IEnumerable<Node<DataType, WeightType>>) graph)
    // enumerate over nodes

foreach (var s in (IEnumerable<DataType>) graph)
    // enumerate over data stored in the nodes

foreach (var e in (IEnumerable<Edge<DataType, WeightType>>) graph)
    // enumerate over edges
```

##### Enumerating via breadth-first-enumerator

```c#
foreach (var n in (IEnumerable<Node<DataType, WeightType>>) graph.AsBreadthFirstEnumerable)
    // enumerate over nodes

foreach (var s in (IEnumerable<DataType>) graph.AsBreadthFirstEnumerable)
    // enumerate over data stored in the nodes

foreach (var e in (IEnumerable<Edge<DataType, WeightType>>) graph.AsBreadthFirstEnumerable)
    // enumerate over edges
```

##### Enumerating via depth-first-enumerator

```c#
foreach (var n in (IEnumerable<Node<DataType, WeightType>>) graph.AsDepthFirstEnumerable)
    // enumerate over nodes

foreach (var s in (IEnumerable<DataType>) graph.AsDepthFirstEnumerable)
    // enumerate over data stored in the nodes

foreach (var e in (IEnumerable<Edge<DataType, WeightType>>) graph.AsDepthFirstEnumerable)
    // enumerate over edges
```

### Graph types

#### Graph

A normal graph is undirected and does not have weights on its edges. All algorithms are assuming the edge weight of all
edges is equal.

#### Weighted graph

A weighted graph is undirected but has edge weights on its edges. Algorithms on the graph may use the edge weight to
sort the edges.

#### Directed graph

A directed graph has an direction aplied to all its edges. Each node has incoming and outgoing edges. The edges do not
have a weight, so algorithms are assuming the edge weight is equal on all edges.

#### Directed and weighted graph

A directed and weighted graph has a direction and weight aplied to all its edges. Each node has incoming and outgoing
edges. Algorithms on this graph are using the edge weight to sort the edges.

## Trees

To be continued...

