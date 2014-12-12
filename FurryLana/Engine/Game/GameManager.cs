//
//  GameManager.cs
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
using System.Collections;
using System.Collections.Generic;
using FurryLana.Engine.Game.Interfaces;

namespace FurryLana.Engine.Game
{
    public class GameManager : IGameManager
    {
        public GameManager (IGame rootGame)
        {
            Games = new List<IGame> ();
            RootGame = rootGame;
            Games.Add (rootGame);
            Loaded = true;
        }

        protected List<IGame> Games;

        #region IManager implementation

        public void Add (IGame item)
        {
            Games.Add (item);
        }

        public void Remove (IGame item)
        {
            Games.Remove (item);
        }

        public void Remove (string name)
        {
            Games.RemoveAll (g => g.Name == name);
        }

        public IGame GetByName (string name)
        {
            return Games.Find (g => g.Name == name);
        }

        public int Count 
        {
            get
            {
                return Games.Count;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return Games.GetEnumerator ();
        }

        #endregion

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            foreach (var g in Games)
                list = g.GetInitJobs (list);
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            foreach (var g in Games)
                list = g.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            Games.ForEach (g => g.Destroy ());
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IGameManager implementation

        public IGame RootGame { get; protected set; }

        #endregion
    }
}
