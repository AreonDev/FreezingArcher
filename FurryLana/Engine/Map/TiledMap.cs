//
//  TiledMap.cs
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
using FurryLana.Engine.Map.Interfaces;
using FurryLana.Engine.Model.Interfaces;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Map
{
    /// <summary>
    /// Tiled map.
    /// </summary>
    public class TiledMap : IMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Map.TiledMap"/> class.
        /// </summary>
        /// <param name="tileSize">Tile size.</param>
        /// <param name="size">Size.</param>
        public TiledMap (float tileSize, Vector2i size)
        {
            TileSize = tileSize;
            Size = size;
            Sky = new Sky ();
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
            Sky.GetInitJobs (list);
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
            Sky.GetLoadJobs (list, reloader);
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
            Sky.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Map.TiledMap"/> is loaded.
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
            Sky.FrameSyncedUpdate (deltaTime);
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
            Sky.Update (desc);
        }
        #endregion

        #region IDrawable implementation
        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            Sky.Draw ();
        }
        #endregion

        /// <summary>
        /// Gets or sets the sky.
        /// </summary>
        /// <value>The sky.</value>
        public Sky Sky { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Vector2i Size { get; set; }
        /// <summary>
        /// Gets or sets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public float TileSize { get; set; }
    }
}

