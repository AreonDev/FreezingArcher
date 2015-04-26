//
//  ReadOnlyList.cs
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
using System.Collections.Generic;
using System.Collections;

namespace FreezingArcher.DataStructures
{
    /// <summary>
    /// Read only list.
    /// </summary>
    public sealed class ReadOnlyList<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyList class.
        /// </summary>
        /// <param name="list">List.</param>
        public ReadOnlyList(IList<T> list)
        {
            internalList = list;
        }

        readonly IList<T> internalList;

        #region IEnumerable implementation

        /// <summary>
        /// Gets the enumerator of T.
        /// </summary>
        /// <returns>The T enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        #endregion

        #region IReadOnlyCollection implementation

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return internalList.Count;
            }
        }

        #endregion

        /// <summary>
        /// Gets the data with the specified idx.
        /// </summary>
        /// <param name="idx">Index.</param>
        public T this[int idx]
        {
            get
            {
                return internalList[idx];
            }
        }

        /// <param name="list">List.</param>
        public static implicit operator ReadOnlyList<T>(List<T> list)
        {
            return new ReadOnlyList<T>(list);
        }
    }
}
