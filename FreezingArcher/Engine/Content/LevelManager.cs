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
using System.Collections;
using System.Collections.Generic;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Input;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Level manager.
    /// </summary>
    public class LevelManager : IManager<Level>, IResource, IUpdate
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "LevelManager";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.LevelManager"/> class.
        /// </summary>
        public LevelManager ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating new level manager");
            Levels = new List<Level> ();
            Loaded = true;
        }

        /// <summary>
        /// The levels.
        /// </summary>
        protected List<Level> Levels;

        /// <summary>
        /// Gets the current level.
        /// </summary>
        /// <value>The current level.</value>
        public Level CurrentLevel { get; protected set; }

        /// <summary>
        /// Sets the current level.
        /// </summary>
        /// <param name="name">Name.</param>
        public void SetCurrentLevel (string name)
        {
            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Setting current level to '{0}'", name);
            CurrentLevel = Levels.Find (l => l.Name == name);
        }

        /// <summary>
        /// Sets the current level.
        /// </summary>
        /// <param name="level">Level.</param>
        public void SetCurrentLevel (Level level)
        {
            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Setting current level to '{0}'", level.Name);
            CurrentLevel = level;
        }

        #region IManager implementation

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add (Level item)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Adding new level '{0}' to level manager", item.Name);
            Levels.Add (item);

            if (Levels.Count == 1)
            {
                Add (item);
                SetCurrentLevel (item);
            }
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove (Level item)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Removing level '{0}' from level manager", item.Name);
            Levels.Remove (item);
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Remove (string name)
        {
            Levels.RemoveAll (l => {
                if (l.Name == name)
                {
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                        "Removing level '{0}' from level manager", l.Name);
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Gets the IManageable by name.
        /// </summary>
        /// <returns>The IManageable.</returns>
        /// <param name="name">Name.</param>
        public Level GetByName (string name)
        {
            return Levels.Find (l => l.Name == name);
        }

        /// <summary>
        /// Gets the <see cref="FreezingArcher.Content.LevelManager"/> with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public Level this[string name]
        {
            get
            {
                return GetByName (name);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return Levels.Count;
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
            return Levels.GetEnumerator ();
        }

        #endregion

        #region IResource implementation

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            foreach (var l in Levels)
                list = l.GetInitJobs (list);
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
            foreach (var l in Levels)
                list = l.GetLoadJobs (list, reloader);
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
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Destroying level manager");
            Levels.ForEach (l => l.Destroy ());
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Content.LevelManager"/> is loaded.
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
            Levels.ForEach (l => l.FrameSyncedUpdate (deltaTime));
        }

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (InputDescription desc)
        {
            Levels.ForEach (l => l.Update (desc));
        }

        #endregion
    }
}
