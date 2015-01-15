//
//  Level.cs
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
using System.Collections.Generic;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Camera;
using FurryLana.Engine.Camera.Interfaces;
using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Game.Interfaces;
using FurryLana.Engine.Map.Interfaces;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Game
{
    /// <summary>
    /// Level.
    /// </summary>
    public class Level : ILevel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Game.Level"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="map">Map.</param>
        /// <param name="cameraManager">Camera manager.</param>
        /// <param name="projDesc">Projection description.</param>
        public Level (string name, IMap map, ICameraManager cameraManager,ProjectionDescription projDesc)
        {
            Name = name;
            CameraManager = cameraManager;
            Map = map;
            Entities = new List<IEntity> ();
            ProjectionDescription = projDesc;
            UpdateProjectionMatrix (Engine.Application.Application.Instance.Window.Size.X,
                                    Engine.Application.Application.Instance.Window.Size.Y);
            Loaded = true;
        }

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {}

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            CameraManager.GetInitJobs (list);
            Map.GetInitJobs (list);
            foreach (var e in Entities)
                list = e.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {}

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            CameraManager.GetLoadJobs (list, reloader);
            Map.GetLoadJobs (list, reloader);
            foreach (var e in Entities)
                e.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {
            CameraManager.Destroy ();
            Map.Destroy ();
            Entities.ForEach (e => e.Destroy ());
            Entities.Clear ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Game.Level"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (float deltaTime)
        {
            CameraManager.FrameSyncedUpdate (deltaTime);
            Map.FrameSyncedUpdate (deltaTime);
            Entities.ForEach (e => e.FrameSyncedUpdate (deltaTime));
        }

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            CameraManager.Update (desc);
            Map.Update (desc);
            Entities.ForEach (e => e.Update (desc));
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            CameraManager.Draw ();
            Entities.ForEach (e => e.Draw ());
            Map.Draw ();
        }

        #endregion

        #region ILevel implementation

        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <value>The map.</value>
        public IMap Map { get; protected set; }

        /// <summary>
        /// Gets the camera manager.
        /// </summary>
        /// <value>The camera manager.</value>
        public ICameraManager CameraManager { get; protected set; }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public List<IEntity> Entities { get; protected set; }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>The projection matrix.</value>
        public Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// Gets or sets the projection description.
        /// </summary>
        /// <value>The projection description.</value>
        public ProjectionDescription ProjectionDescription { get; set; }

        /// <summary>
        /// Updates the projection matrix.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void UpdateProjectionMatrix (int width, int height)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (
                ProjectionDescription.FieldOfView, (float) width / height,
                ProjectionDescription.ZNear, ProjectionDescription.ZFar);
        }

        #endregion

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}
