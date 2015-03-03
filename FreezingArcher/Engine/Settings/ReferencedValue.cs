using System;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Referenced value.
    /// </summary>
    public class ReferencedValue : IValue<IValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.ReferencedValue"/> class.
        /// </summary>
        /// <param name="referenceName">Reference name.</param>
        /// <param name="value">The dereferenced value.</param>
        /// <param name="group">The group containing this values.</param>
        public ReferencedValue (string referenceName, IValue value, IGroup group)
        {
            if (value == null)
                Value = new StringValue (referenceName);
            else
                Value = value;

            ReferenceName = referenceName;

            if (group == null)
                throw new ArgumentNullException ("group", "The parent group of a ReferencedValue must not be null!");

            parentGroup = group;
        }

        /// <summary>
        /// Get or set the name of the reference.
        /// </summary>
        /// <value>The name of the reference.</value>
        public string ReferenceName { get; set; }

        /// <summary>
        /// The parent group.
        /// </summary>
        protected IGroup parentGroup;

        #region IValue implementation
        
        /// <summary>
        /// Internally stored value.
        /// </summary>
        protected IValue val;

        /// <summary>
        /// Get or set the value.
        /// </summary>
        /// <value>The value.</value>
        public IValue Value  {
            get {
                return val;
            }
        
            set {
                if (value != null)
                {
                    val = value;
                    if (RootSettings != null && RootSettings.SettingsChanged != null)
                        RootSettings.SettingsChanged (this, new SettingsChangedEventArgs ()
                                                      { Settings = RootSettings });
                }
            }
        }

        object IValue.Value {
            get {
                return (object) Value;
            }
            set {
                IValue v = value as IValue;
                if (v != null)
                    Value = v;
            }
        }

        public IValue<IValue> Clone ()
        {
            return new ReferencedValue (ReferenceName, val, parentGroup);
        }

        IValue IValue.Clone ()
        {
            return Clone ();
        }

        #endregion

        #region ISettingsRoot implementation

        public ISettings RootSettings { get; set; }

        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                string s = "";
                if (parentGroup[ReferenceName] == null)
                    s += ReferenceName + " = " + Value.CfgString + "\n";
                return s + "$" + ReferenceName;
            }
        }

        #endregion
    }
}
