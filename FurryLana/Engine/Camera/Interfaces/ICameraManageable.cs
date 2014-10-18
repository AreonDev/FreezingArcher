//
//  ICameraManageable.cs
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

namespace FurryLana.Engine.Camera.Interfaces
{
    public interface ICameraManageable
    {
        /// <summary>
        /// This method is called when the camera manager switches to this subject.
        /// </summary>
        void Enable ();

        /// <summary>
        /// This method is called when the camera manager switches from this subject to another one.
        /// </summary>
        void Disable ();

        /// <summary>
        /// The Name is an unique identifier used to identify the subject inside a camera manager.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }
    }
}
