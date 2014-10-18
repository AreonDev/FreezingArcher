//
//  IDirectionRotateable.cs
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
using FurryLana.Math;

namespace FurryLana.Engine.Interaction
{
    /// <summary>
    /// Indicates that the implementing object can rotate along Directions
    /// </summary>
    public interface IDirectionRotateable
    {
        /// <summary>
        /// Rotate left around Y axis
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <param name="angle">Angle format</param>
        void RotLeft (double degree, AngleEnum angle = AngleEnum.Degree);
        
        /// <summary>
        /// Rotate right around Y axis
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <param name="angle">Angle format</param>
        void RotRight (double degree, AngleEnum angle = AngleEnum.Degree);
        
        /// <summary>
        /// Rotate up around X axis
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <param name="angle">Angle format</param>
        void RotUp (double degree, AngleEnum angle = AngleEnum.Degree);
        
        /// <summary>
        /// Rotate down around X axis
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <param name="angle">Angle format</param>
        void RotDown (double degree, AngleEnum angle = AngleEnum.Degree);
        
        /// <summary>
        /// Rotate clockwise around Z axis
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <param name="angle">Angle format</param>
        void RotCw (double degree, AngleEnum angle = AngleEnum.Degree);
        
        /// <summary>
        /// Rotate counter-clockwise around Z axis
        /// </summary>
        /// <param name="degree">Degree</param>
        /// <param name="angle">Angle format</param>
        void RotCcw (double degree, AngleEnum angle = AngleEnum.Degree);
    }
}
