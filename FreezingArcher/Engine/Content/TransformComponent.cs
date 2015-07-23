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
using FreezingArcher.Math;
using FreezingArcher.Messaging;
using System;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Transformation component. This component describes the transformation of an entity.
    /// </summary>
    public sealed class TransformComponent : EntityComponent
    {
        #region default values

        /// <summary>
        /// The default position.
        /// </summary>
        public static readonly Vector3 DefaultPosition = Vector3.Zero;

        /// <summary>
        /// The default rotation.
        /// </summary>
        public static readonly Quaternion DefaultRotation;

        /// <summary>
        /// The default scale.
        /// </summary>
        public static readonly Vector3 DefaultScale = Vector3.One;

        static TransformComponent ()
        {
            Vector3 axis = Vector3.UnitY;
            Quaternion.CreateFromAxisAngle(ref axis, 0, out DefaultRotation);
        }

        #endregion

        Vector3 position;
        Quaternion rotation;
        Vector3 scale;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if (OnPositionChanged != null)
                    OnPositionChanged(value);
            }
        }

        public event Action<Vector3> OnPositionChanged;

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                if (OnRotationChanged != null)
                    OnRotationChanged(value);
            }
        }

        public event Action<Quaternion> OnRotationChanged;

        /// <summary>
        /// Gets or sets the scale of this entity instance.
        /// </summary>
        /// <value>The scale.</value>
        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                if (OnScaleChanged != null)
                    OnScaleChanged(value);
            }
        }

        public event Action<Vector3> OnScaleChanged;

        /// <summary>
        /// Gets the world matrix.
        /// </summary>
        /// <value>The world matrix.</value>
        public Matrix WorldMatrix
        {
            get
            {
                return Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            }
        }
    }
}
