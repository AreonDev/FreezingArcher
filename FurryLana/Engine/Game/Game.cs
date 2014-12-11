//
//  Game.cs
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
using FurryLana.Engine.Game.Interfaces;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Game
{
    public class Game : IGame
    {
        public Game (string name)
        {
            Name = name;
            LevelManager = new LevelManager ();
            Loaded = true;
        }

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            LevelManager.GetInitJobs (list);
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            LevelManager.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            LevelManager.Destroy ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            LevelManager.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            LevelManager.Update (desc);
        }

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {
            LevelManager.Draw ();
        }

        #endregion

        #region IGame implementation

        public ILevelManager LevelManager { get; protected set; }

        #endregion

        #region IManageable implementation

        public string Name { get; set; }

        #endregion
    }
}

