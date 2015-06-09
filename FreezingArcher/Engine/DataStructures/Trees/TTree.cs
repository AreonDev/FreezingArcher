//
//  TTree.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2015 Martin Koppehel
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

namespace FreezingArcher.DataStructures.Trees
{
    /// <summary>
    /// Generic tree of arbitrary data
    /// </summary>
    public class Tree<TData> : ITree<TData>, IEnumerable<Tree<TData>>
    {
        IEnumerable<TData> TiefenSuche;

        /// <summary>
        /// Enumerates all nodes or their data in level-order traversal
        /// </summary>
        public class LevelOrderEnumerable : IEnumerable<TData>, IEnumerable<Tree<TData>>
        {
            readonly Tree<TData> root;
            internal LevelOrderEnumerable(Tree<TData> r)
            {
                root = r;
            }
            #region IEnumerable implementation
            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
            {
                Queue<Tree<TData>> tmp = new Queue<Tree<TData>>();
                var node = root;
                tmp.Enqueue(node);

                while (tmp.Count > 0)
                {
                    node = tmp.Dequeue();
                    yield return node.Data;
                    foreach (var item in node.children)
                        tmp.Enqueue(item);
                }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<Tree<TData>> IEnumerable<Tree<TData>>.GetEnumerator()
            {
                Queue<Tree<TData>> tmp = new Queue<Tree<TData>>();
                var node = root;
                tmp.Enqueue(node);

                while (tmp.Count > 0)
                {
                    node = tmp.Dequeue();
                    yield return node;
                    foreach (var item in node.children)
                        tmp.Enqueue(item);
                }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                Queue<Tree<TData>> tmp = new Queue<Tree<TData>>();
                var node = root;
                tmp.Enqueue(node);

                while (tmp.Count > 0)
                {
                    node = tmp.Dequeue();
                    yield return node.Data;
                    foreach (var item in node.children)
                        tmp.Enqueue(item);
                }
            }

            #endregion
        }

        public class DepthFirstEnumerable : IEnumerable<TData>, IEquatable<Tree<TData>>
        {
            readonly Tree<TData> root;
            internal DepthFirstEnumerable(Tree<TData> r)
            {
                root = r;
            }
            #region IEnumerable implementation
            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
            {
                Tree<TData> node;
                Stack<Tree<TData>> stack = new Stack<Tree<TData>>();
                List<Tree<TData>> reachedNodes = new List<Tree<TData>>();

                yield return root.Data;

                reachedNodes.Add(root);
                root.children.ForEach(e => {
                    if (!stack.Contains(e))
                        stack.Push(e)});

                do {
                    node = stack.Pop();

                    if(!reachedNodes.Contains(node))
                    {
                        yield return node.Data;

                        reachedNodes.Add(node);
                        node.children.ForEach(e => {
                            if (!stack.Contains(e))
                                stack.Push(e)});
                    }

                } while (stack.Count > 0);

            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<Tree<TData>> IEnumerable<Tree<TData>>.GetEnumerator()
            {
                Tree<TData> node;
                Stack<Tree<TData>> stack = new Stack<Tree<TData>>();
                List<Tree<TData>> reachedNodes = new List<Tree<TData>>();

                yield return root;

                reachedNodes.Add(root);
                root.children.ForEach(e => {
                    if (!stack.Contains(e))
                        stack.Push(e)});

                do {
                    node = stack.Pop();

                    if(!reachedNodes.Contains(node))
                    {
                        yield return node;

                        reachedNodes.Add(node);
                        node.children.ForEach(e => {
                            if (!stack.Contains(e))
                                stack.Push(e)});
                    }

                } while (stack.Count > 0);
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                Tree<TData> node;
                Stack<Tree<TData>> stack = new Stack<Tree<TData>>();
                List<Tree<TData>> reachedNodes = new List<Tree<TData>>();

                yield return root.Data;

                reachedNodes.Add(root);
                root.children.ForEach(e => {
                    if (!stack.Contains(e))
                        stack.Push(e)});

                do {
                    node = stack.Pop();

                    if(!reachedNodes.Contains(node))
                    {
                        yield return node.Data;

                        reachedNodes.Add(node);
                        node.children.ForEach(e => {
                            if (!stack.Contains(e))
                                stack.Push(e)});
                    }

                } while (stack.Count > 0);
            }

            #endregion
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public TData Data {get; set;}

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Tree<TData> Parent { get; set;}

        private List<Tree<TData>> children = new List<Tree<TData>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.DataStructures.Trees.Tree{TData}"/> class.
        /// </summary>
        public Tree()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Tree class.
        /// </summary>
        /// <param name="_Data">Data.</param>
        public Tree(TData _Data)
        {
            Data = _Data;
        }

        /// <summary>
        /// Gets the <see cref="FreezingArcher.DataStructures.Trees.Tree{TData}"/> at the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public Tree<TData> this[int index]
        {
            get
            {
                return children[index];
            }
        }

        /// <summary>
        /// Gets a readonly collecton of children.
        /// </summary>
        /// <value>The children.</value>
        public ReadOnlyList<Tree<TData>> Children
        {
            get
            {
                return children;
            }
        }

        private LevelOrderEnumerable levelOrder;

        /// <summary>
        /// Gets an enumerable which enumerates in levl-order traversal
        /// </summary>
        /// <value>The level-order enumerable.</value>
        public LevelOrderEnumerable LevelOrder
        {
            get
            {
                return levelOrder ?? (levelOrder = new LevelOrderEnumerable(this));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the root node.
        /// </summary>
        /// <value>true</value>
        /// <c>false</c>
        public bool IsRoot
        {
            get
            {
                return Parent == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a leaf.
        /// </summary>
        /// <value>true</value>
        /// <c>false</c>
        public bool IsLeaf
        {
            get
            {
                return children.Count == 0;
            }
        }
        /// <summary>
        /// Gets the level of this node.
        /// </summary>
        /// <value>The level.</value>
        public int Level
        {
            get
            {
                int lvl = 0;
                var crt = this;
                while(crt.Parent != null)
                {
                    lvl++;
                    crt = crt.Parent;
                }
                return lvl;
            }
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="data">Data.</param>
        public Tree<TData> AddChild(TData data)
        {
            var tree = new Tree<TData>{ Data = data };
            children.Add(tree);
            tree.Parent = this;
            return tree;
        }
        /// <summary>
        /// Removes the given child.
        /// </summary>
        /// <param name="child">Child.</param>
        public void RemoveChild(Tree<TData> child)
        {
            child.Parent = null;
            children.Remove(child);
        }
        /// <summary>
        /// Removes the child at the given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public void RemoveChild(int index)
        {
            var item = children[index];
            item.Parent = null;
            children.Remove(item);
        }

        #region IEnumerable implementation
        IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
        {
            return ((IEnumerable<TData>)LevelOrder).GetEnumerator();
        }
        IEnumerator<Tree<TData>> IEnumerable<Tree<TData>>.GetEnumerator()
        {
            return ((IEnumerable<Tree<TData>>)LevelOrder).GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)LevelOrder).GetEnumerator();
        }

        #endregion
    }
}

