//
//  ConfigManager.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
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
using System.Collections;
using System.Collections.Generic;
using FreezingArcher.Core;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Output;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;

namespace FreezingArcher.Configuration
{
    /// <summary>
    /// Config manager class. Manages config files.
    /// </summary>
    public class ConfigManager : IManager<ConfigFile>
    {
        /// <summary>
        /// Item added handler.
        /// </summary>
        public delegate void ItemAddedHandler (ConfigFile config);

        /// <summary>
        /// Item removed handler.
        /// </summary>
        public delegate void ItemRemovedHandler (ConfigFile config);

        /// <summary>
        /// The default config of the freezing_archer.conf.
        /// </summary>
        public readonly static Pair<string, Dictionary<string, Section>> DefaultConfig =
            new Pair<string, Dictionary<string, Section>> ("freezing_archer", new Dictionary<string, Section> ());

        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "ConfigManager";

        /// <summary>
        /// Initializes the <see cref="FreezingArcher.Configuration.ConfigManager"/> class.
        /// </summary>
        static ConfigManager ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading configuration defaults ...");

            Section general = new Section ();
            DefaultConfig.B.Add ("general", general);

            general.Add ("resolution", new Value ("1024x576"));
            general.Add ("fullscreen_resolution", new Value ("1920x1080"));
            general.Add ("fullscreen_monitor", new Value (0));
            general.Add ("fullscreen", new Value (false));
            general.Add ("mouse_speed", new Value (1.2));
        }

        /// <summary>
        /// The global instance.
        /// </summary>
        public static ConfigManager Instance;

        /// <summary>
        /// Initialize the global instance.
        /// </summary>
        public static void Initialize ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Initializing config manager ...");
            Instance = new ConfigManager ();
            Instance.Add (new ConfigFile (DefaultConfig.A, DefaultConfig.B));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.ConfigManager"/> class.
        /// </summary>
        public ConfigManager ()
        {
            ConfigFiles = new List<ConfigFile> ();
        }

        /// <summary>
        /// Saves all configuration files.
        /// </summary>
        public void SaveAll ()
        {
            ConfigFiles.ForEach (c => c.Save ());
        }

        /// <summary>
        /// The set of holded config files.
        /// </summary>
        protected List<ConfigFile> ConfigFiles;

        /// <summary>
        /// Occurs when item is added.
        /// </summary>
        public event ItemAddedHandler ItemAdded;

        /// <summary>
        /// Occurs when item is removed.
        /// </summary>
        public event ItemRemovedHandler ItemRemoved;

        #region IManager implementation

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add (ConfigFile item)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Adding {0}.conf to config manager ...", item.Name);
            if (ItemAdded != null)
                ItemAdded (item);
            ConfigFiles.Add (item);
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove (ConfigFile item)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Removing {0}.conf from config manager ...", item.Name);
            if (ItemRemoved != null)
                ItemRemoved (item);
            ConfigFiles.Remove (item);
        }

        /// <summary>
        /// Remove element by the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Remove (string name)
        {
            ConfigFiles.RemoveAll (c => {
                if (c.Name == name)
                {
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                        "Removing {0}.conf from config manager ...", c.Name);
                    if (ItemRemoved != null)
                        ItemRemoved (c);
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Gets the IManageable by the specified name.
        /// </summary>
        /// <returns>The IManageable.</returns>
        /// <param name="name">Name.</param>
        public ConfigFile GetByName (string name)
        {
            return ConfigFiles.Find (c => c.Name == name);
        }

        /// <summary>
        /// Gets the <see cref="FreezingArcher.Configuration.ConfigManager"/> with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public ConfigFile this [string name]
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
                return ConfigFiles.Count;
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
            return ConfigFiles.GetEnumerator ();
        }

        #endregion
    }
}
