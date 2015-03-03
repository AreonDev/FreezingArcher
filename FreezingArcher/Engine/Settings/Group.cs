using System.Collections;
using System.Collections.Generic;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Group.
    /// </summary>
    public class Group : IGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.Group"/> class.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="properties">The properties hold by this group.</param>
        /// <param name="comments">Comments.</param>
        public Group (string name, List<IProperty> properties = null, List<string> comments = null)
        {
            Name = name;
            Properties = properties ?? new List<IProperty> ();
            Comments = comments ?? new List<string> ();

            foreach (IProperty p in Properties)
                p.RootSettings = RootSettings;
        }

        #region IGroup implementation

        public List<IProperty> Properties { get; set; }
        public string Name { get; set; }

        public IProperty GetPropertyByName (string name)
        {
            return Properties.Find (p => p.Name == name);
        }
        
        public IProperty this[string name]
        {
            get
            {
                return GetPropertyByName (name);
            }
        }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                string s = "[" + Name + "]\n";
                foreach (string c in Comments)
                    s += c + "\n";
                foreach (IProperty p in Properties)
                    s += p.CfgString;
                return s;
            }
        }

        #endregion

        #region IComment implementation

        public List<string> Comments { get; set; }

        #endregion

        #region ISettingsRoot implementation

        /// <summary>
        /// Internally stored root settings.
        /// </summary>
        protected ISettings rootSettings;
        public ISettings RootSettings {
            get {
                return rootSettings;
            }

            set {
                rootSettings = value;
                foreach (IProperty p in Properties)
                    p.RootSettings = RootSettings;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return Properties.GetEnumerator ();
        }

        #endregion
    }
}

