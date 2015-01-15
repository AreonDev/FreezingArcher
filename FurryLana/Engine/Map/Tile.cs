//
//  Tile.cs
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
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Model.Interfaces;
using System.Collections.Generic;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Map
{
    /// <summary>
    /// Tile.
    /// </summary>
    public class Tile : IGraphicsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Map.Tile"/> class.
        /// </summary>
        /// <param name="map">Map.</param>
        /// <param name="model">Model.</param>
        /// <param name="walkable">If set to <c>true</c> walkable.</param>
        public Tile (TiledMap map, IModel model, bool walkable)
        {
            this.Model = model;
            this.Walkable = walkable;
            this.Map = map;
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public IModel Model { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Map.Tile"/> is walkable.
        /// </summary>
        /// <value><c>true</c> if walkable; otherwise, <c>false</c>.</value>
        public bool Walkable { get; set; }
        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        /// <value>The map.</value>
        public TiledMap Map { get; set; }

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
            list.Add (Init);
            list = Model.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {
            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            list = Model.GetLoadJobs (list, reloader);
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
            Model.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Map.Tile"/> is loaded.
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
            Model.FrameSyncedUpdate (deltaTime);
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
            Model.Update (desc);
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            Model.Draw ();
        }

        #endregion
    }
}
