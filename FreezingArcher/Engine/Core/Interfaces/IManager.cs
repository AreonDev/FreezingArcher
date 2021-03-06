//
//  IManager.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
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
using System.Collections;

namespace FreezingArcher.Core.Interfaces
{
    /// <summary>
    /// Manager interface.
    /// </summary>
    public interface IManager<T> : IEnumerable where T : IManageable
    {
        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        void Add (T item);

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        void Remove (T item);

        /// <summary>
        /// Remove by the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        void Remove (string name);

        /// <summary>
        /// Gets the IManageable by the specified name.
        /// </summary>
        /// <returns>The IManageable.</returns>
        /// <param name="name">Name.</param>
        T GetByName (string name);

        /// <summary>
        /// Gets the <see cref="FreezingArcher.Core.Interfaces.IManageable"/> with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        T this[string name] { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }
    }
}
