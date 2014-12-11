//
//  LevelManager.cs
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
using System.Collections;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Game
{
    public class LevelManager : ILevelManager
    {
        public LevelManager ()
        {
            Levels = new List<ILevel> ();
            Loaded = true;
        }

        protected List<ILevel> Levels;

        #region IManager implementation

        public void Add (ILevel item)
        {
            Levels.Add (item);
        }

        public void Remove (ILevel item)
        {
            Levels.Remove (item);
        }

        public void Remove (string name)
        {
            Levels.RemoveAll (l => l.Name == name);
        }

        public ILevel GetByName (string name)
        {
            return Levels.Find (l => l.Name == name);
        }

        public int Count
        {
            get
            {
                return Levels.Count;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return Levels.GetEnumerator ();
        }

        #endregion

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            Levels.ForEach (l => l.GetInitJobs  (list));
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            Levels.ForEach (l => l.GetLoadJobs (list, reloader));
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            Levels.ForEach (l => l.Destroy ());
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            Levels.ForEach (l => l.FrameSyncedUpdate (deltaTime));
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            Levels.ForEach (l => l.Update (desc));
        }

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {
            Levels.ForEach (l => l.Draw ());
        }

        #endregion
    }
}

