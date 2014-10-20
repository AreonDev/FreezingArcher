//
//  Physics.cs
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
using FurryLana.Engine.Interaction;

namespace FurryLara.Engine.Physics
{
    /// <summary>
    /// Physics.
    /// </summary>
    public class Physics
    {
        /// <summary>
        /// Gravitation.
        /// </summary>
        public const float G = 9.81f;

        /// <summary>
        /// The time base factor.
        /// </summary>
        const float timeBaseFactor = 0.001f;

        /// <summary>
        /// Adds the gravity.
        /// </summary>
        /// <param name="m">Moveable.</param>
        /// <param name="deltaTime">Delta time.</param>
        public static void AddGravity (IMoveable m, int deltaTime)
        {
            var v = m.Speed;
            v.Y -= G * deltaTime * timeBaseFactor;
            m.Speed = v;
        }
    }
}
