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
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Input;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Game.
    /// </summary>
    public class Game : IResource, IManageable, IUpdate
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Game_";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Game"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public Game (string name)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName + name, "Creating new game '{0}'", name);
            Name = name;
            LevelManager = new LevelManager ();
            Loaded = true;
        }

        #region IResource implementation

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            LevelManager.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, Handler reloader)
        {
            LevelManager.GetLoadJobs (list, reloader);
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
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName + Name, "Destroying game '{0}'", Name);
            LevelManager.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Content.Game"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        public event Handler NeedsLoad;

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (double deltaTime)
        {
            LevelManager.FrameSyncedUpdate (deltaTime);
        }

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            LevelManager.Update (desc);
        }

        #endregion

        /// <summary>
        /// Gets the level manager.
        /// </summary>
        /// <value>The level manager.</value>
        public LevelManager LevelManager { get; protected set; }

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}
