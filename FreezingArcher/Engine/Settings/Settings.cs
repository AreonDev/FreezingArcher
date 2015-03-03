using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Settings.
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.Settings"/> class.
        /// </summary>
        /// <param name="url">The location to load/save this setting to.</param>
        /// <param name="comments">Comments.</param>
        public Settings (string url, List<string> comments = null)
        {
            URL = url;
            Groups = new List<IGroup> ();
            Comments = comments ?? new List<string> ();
            Load ();
        }

        #region ISettings implementation

        public void Save (string url = null)
        {
            if (loaded)
            {
                StreamWriter outfile = new StreamWriter (url ?? URL);
                outfile.Write (CfgString);
                outfile.Close ();
            }
        }

        /// <summary>
        /// Internally bool storing if settings are loaded.
        /// </summary>
        protected bool loaded = false;

        public void Load ()
        {
            Load (URL);
        }

        protected void Load (string url)
        {
            ConfigParser.ParseFromFile (url, this);
            
            foreach (IGroup g in Groups)
                g.RootSettings = this;
            
            loaded = true;
        }

        public void SetToDefaults ()
        {
            string url = URL + ".default";
            Groups = new List<IGroup> ();            
            Comments = new List<string> ();

            Load (url);

            if (SettingsChanged != null)
                SettingsChanged (this, new SettingsChangedEventArgs () {Settings = this});
        }

        public IGroup GetGroupByName (string name)
        {
            return Groups.Find (g => g.Name == name);
        }

        public IGroup this[string name]
        {
            get
            {
                return GetGroupByName (name);
            }
        }

        public List<IGroup> Groups { get; set; }

        public string URL { get; set; }

        public EventHandler<SettingsChangedEventArgs> SettingsChanged { get; set; }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                string s = "";
                foreach (string c in Comments)
                    s += c + "\n";
                foreach (IGroup g in Groups)
                    s += g.CfgString;
                return s;
            }
        }

        #endregion

        #region IComment implementation

        public List<string> Comments { get; set; }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return Groups.GetEnumerator ();
        }

        #endregion
    }
}
