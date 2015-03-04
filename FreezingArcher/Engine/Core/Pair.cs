//
//  Pair.cs
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

namespace FreezingArcher.Core
{
    /// <summary>
    /// Represents a pair of 2 generic objects
    /// </summary>
    /// <typeparam name="TX">Type of A</typeparam>
    /// <typeparam name="TY">Type of B</typeparam>
    public class Pair<TX, TY>
    {
        /// <summary>Gets or sets the first item.</summary>
        /// <value>A.</value>
        public TX A { get; set; }
        /// <summary>Gets or sets the second item.</summary>
        /// <value>The b.</value>
        public TY B { get; set; }
        /// <summary>Initializes a new instance of the <see cref="Pair{X, Y}"/> class.</summary>
        /// <param name="a">first item.</param>
        /// <param name="b">second item.</param>
        public Pair (TX a, TY b)
        {
            this.A = a;
            this.B = b;
        }
        /// <summary>Initializes a new instance of the <see cref="Pair{X, Y}"/> class.</summary>
        public Pair ()
        {
            this.A = default (TX);
            this.B = default (TY);
        }
    }
}
