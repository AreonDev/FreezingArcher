//
//  TBinaryTree.cs
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

namespace FreezingArcher.DataStructures
{
    public class BinaryTreeNode<T> : Node<T>
    {
        public BinaryTreeNode (T data) : base (data, null)
        {}

        public BinaryTreeNode (T data, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
        {
            Value = data;
            NodeList<T> children = new NodeList<T> (2);
            children[0] = left;
            children[1] = right;

            Neighbors = children;
        }

        public BinaryTreeNode<T> Left
        {
            get
            {
                return Neighbors == null ? null : (BinaryTreeNode<T>) Neighbors[0];
            }
            set
            {
                Neighbors = Neighbors ?? new NodeList<T>(2);
                Neighbors[0] = value;
            }
        }

        public BinaryTreeNode<T> Right
        {
            get
            {
                return Neighbors == null ? null : (BinaryTreeNode<T>) Neighbors[1];
            }
            set
            {
                Neighbors = Neighbors ?? new NodeList<T>(2);
                Neighbors[1] = value;
            }
        }
    }
}
