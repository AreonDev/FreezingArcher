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
    public class Level : ILevel
    {
        public Level (string name, IMap map, ICameraManager cameraManager,ProjectionDescription projDesc)
        {
            Name = name;
            CameraManager = cameraManager;
            Map = map;
            Entities = new List<IEntity> ();
            ProjectionDescription = projDesc;
            Matrix.CreatePerspectiveFieldOfView (ProjectionDescription.FieldOfView,
                                                 (float) Engine.Application.Application.Instance.Window.WindowedSize.X /
                                                 Engine.Application.Application.Instance.Window.WindowedSize.Y,
                                                 ProjectionDescription.ZNear, ProjectionDescription.ZFar);
            Engine.Application.Application.Instance.Window.WindowResize +=
            (GlfwWindowPtr window, int width, int height) => {
                ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (
				       ProjectionDescription.FieldOfView, (float) width / height,
                                       ProjectionDescription.ZNear, ProjectionDescription.ZFar);
            };
            Loaded = true;
        }

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            CameraManager.GetInitJobs (list);
            Map.GetInitJobs (list);
            foreach (var e in Entities)
                list = e.GetInitJobs (list);
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            CameraManager.GetLoadJobs (list, reloader);
            Map.GetLoadJobs (list, reloader);
            foreach (var e in Entities)
                e.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            CameraManager.Destroy ();
            Map.Destroy ();
            Entities.ForEach (e => e.Destroy ());
            Entities.Clear ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            CameraManager.FrameSyncedUpdate (deltaTime);
            Map.FrameSyncedUpdate (deltaTime);
            Entities.ForEach (e => e.FrameSyncedUpdate (deltaTime));
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            CameraManager.Update (desc);
            Map.Update (desc);
            Entities.ForEach (e => e.Update (desc));
        }

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {
            CameraManager.Draw ();
            Entities.ForEach (e => e.Draw ());
            Map.Draw ();
        }

        #endregion

        #region ILevel implementation

        public IMap Map { get; protected set; }

        public ICameraManager CameraManager { get; protected set; }

        public List<IEntity> Entities { get; protected set; }

        public Matrix ProjectionMatrix { get; set; }

        public ProjectionDescription ProjectionDescription { get; set; }

        #endregion

        #region IManageable implementation

        public string Name { get; set; }

        #endregion
    }
}
