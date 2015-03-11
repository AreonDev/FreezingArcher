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
using Pencil.Gaming.MathUtils;

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
    public class Listener : IResource
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Listener_";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Audio.Listener"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="velocity">Velocity.</param>
        /// <param name="orientation">Orientation.</param>
        /// <param name="gain">Gain.</param>
        internal Listener (Vector3 position, Vector3 velocity, Pair<Vector3, UpVector> orientation, float gain)
        {
            InitPosition = position;
            InitVelocity = velocity;
            InitGain = gain;
            InitOrientation = orientation;
            Loaded = false;
        }

        /// <summary>
        /// The initial position.
        /// </summary>
        protected Vector3 InitPosition;

        /// <summary>
        /// The initial velocity.
        /// </summary>
        protected Vector3 InitVelocity;

        /// <summary>
        /// The initial gain.
        /// </summary>
        protected float InitGain;

        /// <summary>
        /// The initial orientation.
        /// </summary>
        protected Pair<Vector3, UpVector> InitOrientation;

        /// <summary>
        /// Load this resource.
        /// </summary>
        protected void Load ()
        {
            Loaded = true;
            Position = InitPosition;
            Velocity = InitVelocity;
            Gain = InitGain;
            Orientation = InitOrientation;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Position property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float px, py, pz;
                AL.GetListener (ALListener3f.Position, out px, out py, out pz);
                return new Vector3 (px, py, pz);
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Position property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Velocity property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float vx, vy, vz;
                AL.GetListener (ALListener3f.Velocity, out vx, out vy, out vz);
                return new Vector3 (vx, vy, vz);
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Velocity property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Gain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

                float gain;
                AL.GetListener (ALListenerf.Gain, out gain);
                return gain;
            }
            set
            {
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Gain property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Orientation property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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
                if (!Loaded)
                {
                    Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                        "Trying to modify Orientation property before resource was loaded!");
                    throw new InvalidOperationException ();
                }

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

        #region IResource implementation

        /// <summary>
        /// Fire this event when you need the binded load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        public event Handler NeedsLoad;

        /// <summary>
        /// Gets the init jobs. The init jobs may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            return list;
        }

        /// <summary>
        /// Gets the load jobs. The load jobs will be executed sequentially in the gl thread.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, Handler reloader)
        {
            list.Add (Load);
            NeedsLoad = reloader;
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the garbage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Audio.Sound"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; set; }

        #endregion
    }
}
