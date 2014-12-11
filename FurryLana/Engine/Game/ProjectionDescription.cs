//
//  ProjectionDescription.cs
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
using System;

namespace FurryLana.Engine.Game
{
    /// <summary>
    /// Projection description.
    /// </summary>
    public class ProjectionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Game.ProjectionDescription"/> class.
        /// </summary>
        public ProjectionDescription ()
        {
            FieldOfView = (float) (System.Math.PI / 3);
            ZNear = 0.1f;
            ZFar = 200f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Game.ProjectionDescription"/> class.
        /// </summary>
        /// <param name="fieldOfView">Field of view.</param>
        /// <param name="zNear">Z near.</param>
        /// <param name="zFar">Z far.</param>
        public ProjectionDescription (float fieldOfView, float zNear, float zFar)
        {
            FieldOfView = fieldOfView;
            ZNear = zNear;
            ZFar = zFar;
        }

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>The field of view.</value>
        public float FieldOfView { get; set; }

        /// <summary>
        /// Gets or sets the Z near.
        /// </summary>
        /// <value>The Z near.</value>
        public float ZNear { get; set; }

        /// <summary>
        /// Gets or sets the Z far.
        /// </summary>
        /// <value>The Z far.</value>
        public float ZFar { get; set; }
    }
}
