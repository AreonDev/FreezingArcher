//
//  PositionComponen.cs
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
using System;
using FreezingArcher.Math;
using System.Threading;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Transformation component. This component describes the transformation of an entity.
    /// </summary>
    public sealed class TransformComponent : EntityComponent
    {
        #region default values

        // ommit warning: variable assigned but never used
        #pragma warning disable 414

        public static readonly Vector3 DefaultPosition = Vector3.Zero;

        public static readonly Quaternion DefaultRotation;

        public static readonly Vector3 DefaultScale = Vector3.One;

        static TransformComponent ()
        {
            Vector3 axis = Vector3.UnitY;
            Quaternion.CreateFromAxisAngle(ref axis, 0, out DefaultRotation);
        }

        #pragma warning restore 414

        #endregion

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public Vector3 Scale { get; set; }
    }
}
