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
    public class Tile : IGraphicsResource
    {
        public Tile (TiledMap map, IModel model, bool walkable)
        {
            this.Model = model;
            this.Walkable = walkable;
            this.Map = map;
        }

        public IModel Model { get; set; }
        public bool Walkable { get; set; }
        public TiledMap Map { get; set; }

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            list = Model.GetInitJobs (list);
            return list;
        }

        public void Load ()
        {
            Loaded = true;
        }

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            list = Model.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            Model.Destroy ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            Model.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            Model.Update (desc);
        }

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {
            Model.Draw ();
        }

        #endregion
    }
}
