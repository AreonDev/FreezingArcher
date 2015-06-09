//
//  TBinaryTree.cs
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
using FreezingArcher.Core;
using System.Collections.Generic;

namespace FreezingArcher.DataStructures.Trees
{
    /// <summary>
    /// Binary tree of arbitrary data
    /// </summary>
    public class BinaryTree<TData> : ITree<TData>
    {
        /// <summary>
        /// Enumerates all nodes or their data in pre-order traversal
        /// </summary>
        public class PreOrderEnumerable : IEnumerable<TData>, IEnumerable<BinaryTree<TData>>
        {
            readonly BinaryTree<TData> root;
            internal PreOrderEnumerable(BinaryTree<TData> r)
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
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                tmp.Push(root);
                BinaryTree<TData> tree;
                while(tmp.Count > 0)
                {
                    tree = tmp.Pop();
                    yield return tree.Data;
                    if (tree.Right != null)
                        tmp.Push(tree.Right);
                    if (tree.Left != null)
                        tmp.Push(tree.Left);
                }
            }
            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<BinaryTree<TData>> IEnumerable<BinaryTree<TData>>.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                tmp.Push(root);
                BinaryTree<TData> tree;
                while(tmp.Count > 0)
                {
                    tree = tmp.Pop();
                    yield return tree;
                    if (tree.Right != null)
                        tmp.Push(tree.Right);
                    if (tree.Left != null)
                        tmp.Push(tree.Left);
                }
            }
            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                tmp.Push(root);
                BinaryTree<TData> tree;
                while(tmp.Count > 0)
                {
                    tree = tmp.Pop();
                    yield return tree.Data;
                    if (tree.Right != null)
                        tmp.Push(tree.Right);
                    if (tree.Left != null)
                        tmp.Push(tree.Left);
                }
            }

            #endregion
        }

        /// <summary>
        /// Enumerates all nodes or their data in in-order traversal
        /// </summary>
        public class InOrderEnumerable : IEnumerable<TData>, IEnumerable<BinaryTree<TData>>
        {
            readonly BinaryTree<TData> root;
            internal InOrderEnumerable(BinaryTree<TData> r)
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
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                tmp.Push(root);
                BinaryTree<TData> current = root;
                while (tmp.Count > 0)
                {
                    while(current.Left != null)
                    {
                        tmp.Push(current);
                        current = current.Left;
                    }
                    var pk = tmp.Peek();
                    yield return pk.Data;
                    current = pk.Right;
                    tmp.Pop();
                }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<BinaryTree<TData>> IEnumerable<BinaryTree<TData>>.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                tmp.Push(root);
                BinaryTree<TData> current = root;
                while (tmp.Count > 0)
                {
                    while(current.Left != null)
                    {
                        tmp.Push(current);
                        current = current.Left;
                    }
                    var pk = tmp.Peek();
                    yield return pk;
                    current = pk.Right;
                    tmp.Pop();
                }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                tmp.Push(root);
                BinaryTree<TData> current = root;
                while (tmp.Count > 0)
                {
                    while(current.Left != null)
                    {
                        tmp.Push(current);
                        current = current.Left;
                    }
                    var pk = tmp.Peek();
                    yield return pk.Data;
                    current = pk.Right;
                    tmp.Pop();
                }
            }

            #endregion
         
        }

        /// <summary>
        /// Enumerates all nodes or their data in post-order traversal
        /// </summary>
        public class PostOrderEnumerable : IEnumerable<TData>, IEnumerable<BinaryTree<TData>>
        {
            readonly BinaryTree<TData> root;
            internal PostOrderEnumerable(BinaryTree<TData> r)
            {
                root = r;
            }
            #region IEnumerable implementation

            IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                BinaryTree<TData> lastVisited = null, node = root;
                while(tmp.Count > 0 || node != null)
                {
                    if (node != null)
                    {
                        tmp.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        var pk = tmp.Peek();
                        if (pk.Right != null && lastVisited != pk.Right)
                            node = pk.Right;
                        else
                        {
                            yield return pk.Data;
                            lastVisited = tmp.Pop();
                        }

                    }
                }
            }

            IEnumerator<BinaryTree<TData>> IEnumerable<BinaryTree<TData>>.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                BinaryTree<TData> lastVisited = null, node = root;
                while(tmp.Count > 0 || node != null)
                {
                    if (node != null)
                    {
                        tmp.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        var pk = tmp.Peek();
                        if (pk.Right != null && lastVisited != pk.Right)
                            node = pk.Right;
                        else
                        {
                            yield return pk;
                            lastVisited = tmp.Pop();
                        }

                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                Stack<BinaryTree<TData>> tmp = new Stack<BinaryTree<TData>>();
                BinaryTree<TData> lastVisited = null, node = root;
                while(tmp.Count > 0 || node != null)
                {
                    if (node != null)
                    {
                        tmp.Push(node);
                        node = node.Left;
                    }
                    else
                    {
                        var pk = tmp.Peek();
                        if (pk.Right != null && lastVisited != pk.Right)
                            node = pk.Right;
                        else
                        {
                            yield return pk.Data;
                            lastVisited = tmp.Pop();
                        }

                    }
                }
            }

            #endregion
        }


        /// <summary>
        /// Enumerates all nodes or their data in level-order traversal
        /// </summary>
        public class LevelOrderEnumerable : IEnumerable<TData>, IEnumerable<BinaryTree<TData>>
        {
            readonly BinaryTree<TData> root;
            internal LevelOrderEnumerable(BinaryTree<TData> r)
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
                Queue<BinaryTree<TData>> tmp = new Queue<BinaryTree<TData>>();
                var node = root;
                tmp.Enqueue(node);

                while (tmp.Count > 0)
                {
                    node = tmp.Dequeue();
                    yield return node.Data;
                    if (node.Left != null)
                        tmp.Enqueue(node.Left);
                    if (node.Right != null)
                        tmp.Enqueue(node.Right);
                }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            IEnumerator<BinaryTree<TData>> IEnumerable<BinaryTree<TData>>.GetEnumerator()
            {
                Queue<BinaryTree<TData>> tmp = new Queue<BinaryTree<TData>>();
                var node = root;
                tmp.Enqueue(node);

                while (tmp.Count > 0)
                {
                    node = tmp.Dequeue();
                    yield return node;
                    if (node.Left != null)
                        tmp.Enqueue(node.Left);
                    if (node.Right != null)
                        tmp.Enqueue(node.Right);
                }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                Queue<BinaryTree<TData>> tmp = new Queue<BinaryTree<TData>>();
                var node = root;
                tmp.Enqueue(node);

                while (tmp.Count > 0)
                {
                    node = tmp.Dequeue();
                    yield return node.Data;
                    if (node.Left != null)
                        tmp.Enqueue(node.Left);
                    if (node.Right != null)
                        tmp.Enqueue(node.Right);
                }
            }

            #endregion
        }

        private bool isLeftChild = false;

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public TData Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the root of the tree.
        /// </summary>
        /// <value><c>true</c> if this instance is the root of the tree; otherwise, <c>false</c>.</value>
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
        /// <value><c>true</c> if this instance is a leaf; otherwise, <c>false</c>.</value>
        public bool IsLeaf
        {
            get
            {
                return Left == null && Right == null;
            }
        }

        /// <summary>
        /// Gets the level of this node in the tree.
        /// </summary>
        /// <value>The level of the node.</value>
        public int Level
        {
            get
            {
                int lvl = 0;
                BinaryTree<TData> tree = this; 
                while (tree.Parent != null)
                {
                    tree = tree.Parent;
                    lvl++;
                }
                return lvl;
            }
        }

        /// <summary>
        /// Gets the parent of this node (null if root).
        /// </summary>
        /// <value>The parent of this node.</value>
        public BinaryTree<TData> Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the left child.
        /// </summary>
        /// <value>The left child.</value>
        public BinaryTree<TData> Left
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the right child.
        /// </summary>
        /// <value>The right child.</value>
        public BinaryTree<TData> Right
        { 
            get;
            private set;
        }

        /// <summary>
        /// Gets the brother (the other child of the parent).
        /// </summary>
        /// <value>The brother node.</value>
        public BinaryTree<TData> Brother
        {
            get
            {
                return Parent == null ? null : isLeftChild ? Parent.Right : Parent.Left;
            }
        }

        /// <summary>
        /// Gets the <see cref="FreezingArcher.DataStructures.Trees.BinaryTree{TData}"/> at the specified index.
        /// </summary>
        /// <param name="index">0 for left, otherwise right</param>
        public BinaryTree<TData> this[byte index]
        {
            get
            {
                return index == 0 ? Left : Right;
            }
        }

        /// <summary>
        /// Adds a new child as left child.
        /// </summary>
        /// <returns><c>true</c>, if left child was added, <c>false</c> otherwise.</returns>
        /// <param name="data">Data of the child.</param>
        public bool AddLeftChild(TData data = default(TData))
        {
            if (Left != null)
                return false;
            Left = new BinaryTree<TData>{ Data = data };
            Left.Parent = this;
            Left.isLeftChild = true;
            return true;
        }

        /// <summary>
        /// Adds a new child as right child.
        /// </summary>
        /// <returns><c>true</c>, if right child was added, <c>false</c> otherwise.</returns>
        /// <param name="data">Data of the child.</param>
        public bool AddRightChild(TData data = default(TData))
        {
            if (Right != null)
                return false;
            Right = new BinaryTree<TData>{ Data = data };
            Left.Parent = this;
            Left.isLeftChild = false;
            return true;
        }

        /// <summary>
        /// Removes the left child.
        /// </summary>
        public void RemoveLeftChild()
        {
            Left = null;
        }
        /// <summary>
        /// Removes the right child.
        /// </summary>
        public void RemoveRightChild()
        {
            Right = null;
        }

        private PreOrderEnumerable preOrder;
        /// <summary>
        /// Gets an enumerable which enumerates in pre-order traversal
        /// </summary>
        /// <value>The pre-order enumerable.</value>
        public PreOrderEnumerable PreOrder
        {
            get 
            {
                return preOrder ?? (preOrder = new PreOrderEnumerable(this));
            }
        }
        private InOrderEnumerable inOrder;

        /// <summary>
        /// Gets an enumerable which enumerates in in-order traversal
        /// </summary>
        /// <value>The in-order enumerable.</value>
        public InOrderEnumerable InOrder
        {
            get
            {
                return inOrder ?? (inOrder = new InOrderEnumerable(this));
            }
        }
        private PostOrderEnumerable postOrder;

        /// <summary>
        /// Gets an enumerable which enumerates in post-order traversal
        /// </summary>
        /// <value>The post-order enumerable.</value>
        public PostOrderEnumerable PostOrder
        {
            get
            {
                return postOrder ?? (postOrder = new PostOrderEnumerable(this));
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

        #region IEnumerable implementation
        IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
        {
            return ((IEnumerable<TData>)PreOrder).GetEnumerator();
        }
        IEnumerator<ITree<TData>> IEnumerable<ITree<TData>>.GetEnumerator()
        {
            return ((IEnumerable<BinaryTree<TData>>)PreOrder).GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)PreOrder).GetEnumerator();
        }
        #endregion

    }
}

