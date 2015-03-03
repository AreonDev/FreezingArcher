using System.Globalization;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Float value.
    /// </summary>
    public class FloatValue : IValue<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.FloatValue"/> class.
        /// </summary>
        /// <param name="val">The double hold in this value.</param>
        public FloatValue (double val)
        {
            Value = val;
        }

        #region IValue implementation
        
        /// <summary>
        /// Internally stored value.
        /// </summary>
        protected double val;

        /// <summary>
        /// Get or set the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value {
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
                double? d = value as double?;
                if (d != null)
                    Value = (double) d;
            }
        }

        public IValue<double> Clone ()
        {
            return new FloatValue (val);
        }

        IValue IValue.Clone ()
        {
            return Clone ();
        }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                return Value.ToString (CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region ISettingsRoot implementation

        public ISettings RootSettings { get; set; }

        #endregion

        /// <param name="f">FloatValue.</param>
        public static implicit operator double (FloatValue f)
        {
            return f.Value;
        }

        /// <param name="f">Float.</param>
        public static implicit operator FloatValue (float f)
        {
            return new FloatValue (f);
        }
    }
}
