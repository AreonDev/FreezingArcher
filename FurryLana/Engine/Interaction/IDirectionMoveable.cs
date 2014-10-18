//
//  IDirectionMoveable.cs
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

namespace FurryLana.Engine.Interaction
{
    /// <summary>
    /// Indicates that the implementing object can move along certain directions
    /// </summary>
    public interface IDirectionMoveable
    {
        /// <summary>
        /// Move forward by specified distance
        /// </summary>
        /// <param name="distance">Distance</param>
        void MoveForward (double distance);
        
        /// <summary>
        /// Move backward by specified distance
        /// </summary>
        /// <param name="distance">Distance</param>
        void MoveBackward (double distance);
        
        /// <summary>
        /// Move left by specified distance
        /// </summary>
        /// <param name="distance">Distance</param>
        void MoveLeft (double distance);
        
        /// <summary>
        /// Move right by specified distance
        /// </summary>
        /// <param name="distance">Distance</param>
        void MoveRight (double distance);
        
        /// <summary>
        /// Move up by specified distance
        /// </summary>
        /// <param name="distance">Distance</param>
        void MoveUp (double distance);
        
        /// <summary>
        /// Move down by specified distance
        /// </summary>
        /// <param name="distance">Distance</param>
        void MoveDown (double distance);
    }
}
