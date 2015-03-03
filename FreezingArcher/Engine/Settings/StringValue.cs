using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// String value.
    /// </summary>
    public class StringValue : IValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.StringValue"/> class.
        /// </summary>
        /// <param name="val">The string hold by this value.</param>
        public StringValue (string val)
        {
            if (val == null)
                Value = "";
            else
                Value = val;
        }

        #region IValue implementation
        
        protected string val;

        /// <summary>
        /// Get or set the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value {
            get {
                return val;
            }
            set {
                val = value;
                if (RootSettings != null && RootSettings.SettingsChanged != null)
                    RootSettings.SettingsChanged (this, new SettingsChangedEventArgs () { Settings = RootSettings });
            }
        }

        object IValue.Value {
            get {
                return (object) Value;
            }
            set {
                string s = value as string;
                if (s != null)
                    Value = s;
            }
        }

        public IValue<string> Clone ()
        {
            return new StringValue (val);
        }

        IValue IValue.Clone ()
        {
            return Clone ();
        }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                return Value;
            }
        }

        #endregion

        #region ISettingsRoot implementation

        public ISettings RootSettings { get; set; }

        #endregion

        /// <param name="s">String Value.</param>
        public static implicit operator string (StringValue s)
        {
            return s.Value;
        }

        /// <param name="s">String.</param>
        public static implicit operator StringValue (string s)
        {
            return new StringValue (s);
        }
    }
}

