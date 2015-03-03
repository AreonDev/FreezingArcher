using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Integer value.
    /// </summary>
    public class IntegerValue : IValue<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.IntegerValue"/> class.
        /// </summary>
        /// <param name="val">The integer hold in this value.</param>
        public IntegerValue (int val)
        {
            Value = val;
        }

        #region IValue implementation
        
        /// <summary>
        /// Internally stored value.
        /// </summary>
        protected int val;

        /// <summary>
        /// Get or set the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value {
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
                int? i = value as int?;
                if (i != null)
                    Value = (int) i;
            }
        }

        public IValue<int> Clone ()
        {
            return new IntegerValue (val);
        }

        IValue IValue.Clone ()
        {
            return Clone ();
        }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                return Value.ToString ();
            }
        }

        #endregion

        #region ISettingsRoot implementation

        public ISettings RootSettings { get; set; }

        #endregion

        /// <param name="i">Integer Value.</param>
        public static implicit operator int (IntegerValue i)
        {
            return i.Value;
        }

        /// <param name="i">Integer.</param>
        public static implicit operator IntegerValue (int i)
        {
            return new IntegerValue (i);
        }
    }
}

