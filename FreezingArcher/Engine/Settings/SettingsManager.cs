using System;
using System.Collections;
using System.Collections.Generic;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Settings manager.
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        /// <summary>
        /// A global instance of the settings manager.
        /// </summary>
        public static SettingsManager Manager;

        static SettingsManager ()
        {
            Manager = new SettingsManager ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.SettingsManager"/> class.
        /// </summary>
        /// <param name="urls">A list of configuration file locations which should be parsed initially.</param>
        public SettingsManager (string[] urls = null)
        {
            Settings = new List<ISettings> ();

            if (urls != null)
                foreach (string url in urls)
                    Add (url);
        }

        /// <summary>
        /// The name of the module.
        /// </summary>
        protected string moduleName = "SettingsManager";

        #region ISettingsManager implementation

        public void LoadAll ()
        {
            Settings.ForEach (s => s.Load ());
        }

        public void SaveAll ()
        {
            Settings.ForEach (s => s.Save ());
        }

        public void Add (ISettings settings)
        {
            if (settings != null)
            {
                Settings.Add (settings);
                settings.SettingsChanged += (sender, e) => {
                    if (SettingsChanged != null)
                        SettingsChanged (sender, e);
                };
            }
            //else
                //Logger.Log.addLogEntry (LogLevel.Error, moduleName, "You cannot add a null setting!");FIXME
        }

        public void Add (string url)
        {
            Add (new Settings (url));
        }

        public void Remove (ISettings settings)
        {
            Settings.Remove (settings);
            if (SettingsChanged != null)
                SettingsChanged (this, new SettingsChangedEventArgs {Settings = settings});
        }

        public void Remove (string url)
        {
            List<ISettings> rml = Settings.FindAll (s => s.URL == url);
            foreach (ISettings s in rml)
                Remove (s);
        }

        public void Print ()
        {
            foreach (ISettings s in Settings)
                Console.WriteLine (s.CfgString);
        }

        public ISettings GetSettingsByURL (string url)
        {
            return Settings.Find (s => s.URL == url);
        }

        public ISettings this[string url]
        {
            get
            {
                return GetSettingsByURL (url);
            }
        }

        public List<ISettings> Settings { get; protected set; }

        #endregion

        #region ISettingsChangedEvent implementation

        public EventHandler<SettingsChangedEventArgs> SettingsChanged { get; set; }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return Settings.GetEnumerator ();
        }

        #endregion

        public static IValue operator | (SettingsManager m, string path)
        {
            string[] items = path.Split (new char[] {'/'}, 3);
            return m[items[0]][items[1]][items[2]].Value;
        }
    }
}
