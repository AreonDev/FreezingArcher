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
    /// <summary>
    /// Game manager.
    /// </summary>
    public class GameManager : IGameManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Game.GameManager"/> class.
        /// </summary>
        /// <param name="rootGame">Root game.</param>
        public GameManager (IGame rootGame)
        {
            Games = new List<IGame> ();
            RootGame = rootGame;
            SetCurrentGame (rootGame);
            Games.Add (rootGame);
            Loaded = true;
        }

        /// <summary>
        /// The games.
        /// </summary>
        protected List<IGame> Games;

        #region IManager implementation

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add (IGame item)
        {
            Games.Add (item);
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove (IGame item)
        {
            Games.Remove (item);
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Remove (string name)
        {
            Games.RemoveAll (g => g.Name == name);
        }

        /// <summary>
        /// Gets the IManageable by name.
        /// </summary>
        /// <returns>The IManageable.</returns>
        /// <param name="name">Name.</param>
        public IGame GetByName (string name)
        {
            return Games.Find (g => g.Name == name);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count 
        {
            get
            {
                return Games.Count;
            }
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator GetEnumerator ()
        {
            return Games.GetEnumerator ();
        }

        #endregion

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
            foreach (var g in Games)
                list = g.GetInitJobs (list);
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
            foreach (var g in Games)
                list = g.GetLoadJobs (list, reloader);
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
            Games.ForEach (g => g.Destroy ());
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Game.GameManager"/> is loaded.
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

        #region IGameManager implementation

        /// <summary>
        /// Gets the root game.
        /// </summary>
        /// <value>The root game.</value>
        public IGame RootGame { get; protected set; }

        /// <summary>
        /// Gets the current game.
        /// </summary>
        /// <value>The current game.</value>
        public IGame CurrentGame { get; protected set; }

        /// <summary>
        /// Sets the current game.
        /// </summary>
        /// <param name="name">Name.</param>
        public void SetCurrentGame (string name)
        {
            CurrentGame = Games.Find (g => g.Name == name);
        }

        /// <summary>
        /// Sets the current game.
        /// </summary>
        /// <param name="game">Game.</param>
        public void SetCurrentGame (IGame game)
        {
            CurrentGame = game;
        }

        #endregion
    }
}
