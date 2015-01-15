//
//  ILevel.cs
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
using System.Collections.Generic;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Camera.Interfaces;
using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Map.Interfaces;

namespace FurryLana.Engine.Game.Interfaces
{
    /// <summary>
    /// Level interface.
    /// </summary>
    public interface ILevel : IGraphicsResource, IManageable
    {
        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <value>The map.</value>
        IMap Map { get; }

        /// <summary>
        /// Gets the camera manager.
        /// </summary>
        /// <value>The camera manager.</value>
        ICameraManager CameraManager { get; }
        
        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <value>The entities.</value>
        List<IEntity>  Entities { get; }
        
        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>The projection matrix.</value>
        Matrix         ProjectionMatrix { get; set; }

        /// <summary>
        /// Gets or sets the projection description.
        /// </summary>
        /// <value>The projection description.</value>
        ProjectionDescription ProjectionDescription { get; set; }

        /// <summary>
        /// Updates the projection matrix.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        void UpdateProjectionMatrix (int width, int height);
    }
}
