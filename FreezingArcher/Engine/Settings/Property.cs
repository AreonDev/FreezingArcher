using System.Collections.Generic;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Property.
    /// </summary>
    public class Property : IProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.Property"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The value hold by this property.</param>
        /// <param name="comments">Comments.</param>
        /// <param name="linecomment">A line comment.</param>
        public Property (string name, IValue value = null, List<string> comments = null, string linecomment = "")
        {
            Name = name;
            Value = value;
            Comments = comments ?? new List<string> ();
            LineComment = linecomment ?? "";
        }

        #region IProperty implementation

        protected IValue val;
        public IValue Value {
            get {
                return val;
            }

            set {
                val = value;
                if (value != null)
                    Value.RootSettings = RootSettings;
                if (RootSettings != null && RootSettings.SettingsChanged != null)
                    RootSettings.SettingsChanged (this, null);
            }
        }

        public string Name { get; set; }
        public string LineComment { get; set; }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                string s = Value == null ? "" : Value.CfgString;
                s = Name + " = " + s + " " + LineComment + "\n";
                foreach (string c in Comments)
                    s += c + "\n";
                return s;
            }
        }

        #endregion

        #region IComment implementation

        public List<string> Comments { get; set; }

        #endregion

        #region ISettingsRoot implementation

        protected ISettings rootSettings;
        public ISettings RootSettings {
            get {
                return rootSettings;
            }

            set {
                rootSettings = value;
                Value.RootSettings = RootSettings;
            }
        }

        #endregion
    }
}
