//
//  Listener.cs
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
using System.Collections.Generic;
using FreezingArcher.Core;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Output;
using Pencil.Gaming.Audio;
using FreezingArcher.Math;

namespace FreezingArcher.Audio
{
    /// <summary>
    /// Up vector.
    /// </summary>
    public enum UpVector
    {
        /// <summary>
        /// The unit x vector.
        /// </summary>
        UnitX,
        /// <summary>
        /// The unit y vector.
        /// </summary>
        UnitY,
        /// <summary>
        /// The unit z vector.
        /// </summary>
        UnitZ
    }

    /// <summary>
    /// Audio Listener class.
    /// </summary>
    public class Listener
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Listener";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Listener"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="velocity">Velocity.</param>
        /// <param name="orientation">Orientation.</param>
        /// <param name="gain">Gain.</param>
        internal Listener (Vector3 position, Vector3 velocity, Pair<Vector3, UpVector> orientation, float gain)
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Creating new listener instance");
            Position = position;
            Velocity = velocity;
            Gain = gain;
            Orientation = orientation;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get
            {
                float px, py, pz;
                AL.GetListener (ALListener3f.Position, out px, out py, out pz);
                return new Vector3 (px, py, pz);
            }
            set
            {
                AL.Listener (ALListener3f.Position, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>The velocity.</value>
        public Vector3 Velocity
        {
            get
            {
                float vx, vy, vz;
                AL.GetListener (ALListener3f.Velocity, out vx, out vy, out vz);
                return new Vector3 (vx, vy, vz);
            }
            set
            {
                AL.Listener (ALListener3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// Gets or sets the gain.
        /// </summary>
        /// <value>The gain.</value>
        public float Gain
        {
            get
            {
                float gain;
                AL.GetListener (ALListenerf.Gain, out gain);
                return gain;
            }
            set
            {
                AL.Listener (ALListenerf.Gain, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Pair<Vector3, UpVector> Orientation
        {
            get
            {
                float[] values = new float[6];
                AL.GetListener (ALListenerfv.Orientation, values);

                UpVector unit = UpVector.UnitX;
                if (values[3] > 0)
                    unit = UpVector.UnitX;
                else if (values[4] > 0)
                    unit = UpVector.UnitY;
                else if (values[5] > 0)
                    unit = UpVector.UnitZ;

                return new Pair<Vector3, UpVector> (new Vector3 (values[0], values[1], values[2]), unit);
            }
            set
            {
                Vector3 unit = Vector3.UnitX;
                switch (value.B)
                {
                case UpVector.UnitX:
                    unit = Vector3.UnitX;
                    break;
                case UpVector.UnitY:
                    unit = Vector3.UnitY;
                    break;
                case UpVector.UnitZ:
                    unit = Vector3.UnitZ;
                    break;
                }

                float[] values = new float[] {value.A.X, value.A.Y, value.A.Z, unit.X, unit.Y, unit.Z};
                AL.Listener (ALListenerfv.Orientation, values);
            }
        }
    }
}
