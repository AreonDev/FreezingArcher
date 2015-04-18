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
    /// <summary>
    /// Binary tree.
    /// </summary>
    public class BinaryTree<T>
    {
        /// <summary>
        /// Clear this instance.
        /// </summary>
        public virtual void Clear ()
        {
            Root = null;
        }

        /// <summary>
        /// Gets or sets the root of the tree.
        /// </summary>
        /// <value>The root.</value>
        public BinaryTreeNode<T> Root { get; set; }
    }
}
