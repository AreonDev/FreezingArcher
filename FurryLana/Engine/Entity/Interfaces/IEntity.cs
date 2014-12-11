//
//  IEntity.cs
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
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Interaction;
using FurryLana.Engine.Model.Interfaces;

namespace FurryLana.Engine.Entity.Interfaces
{
    /// <summary>
    /// Entity interface.
    /// </summary>
    public interface IEntity : IGraphicsResource, IPosition, IRotation, IManageable, ISmoothedPosition
    {
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        int ID { get; }
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        IModel Model { get; }

        BoundingBox BBox { get; set; }

        float Height { get; set; }
    }
}
