using System.Collections.Generic;
using System.Linq;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Array value.
    /// </summary>
    public class ArrayValue : IValue<List<IValue>>
    {
        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        /// <param name="arr">The list of <see cref="FreezingArcher.Settings.Interfaces.IValue"/>.</param>
        public ArrayValue (List<IValue> arr)
        {
            if (arr == null)
                Value = new List<IValue> ();
            else
                Value = arr;
        }

        #region IValue implementation
        
        protected List<IValue> val;

        /// <summary>
        /// Get or set the value.
        /// </summary>
        /// <value>The value.</value>
        public List<IValue> Value {
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
                List<IValue> l = value as List<IValue>;
                if (l != null)
                    Value = l;
            }
        }

        public IValue<List<IValue>> Clone ()
        {
            return new ArrayValue (val);
        }

        IValue IValue.Clone ()
        {
            return Clone ();
        }
        
        #endregion

        #region ICfgString implementation

        public string CfgString {
            get {
                string s = "";
                foreach (IValue i in Value)
                    s += i.CfgString + ", ";
                s = s.Substring (0, s.Length - 2);
                return s;
            }
        }

        #endregion

        #region ISettingsRoot implementation

        public ISettings RootSettings { get; set; }

        #endregion

        /// <param name="a">Array Value.</param>
        public static implicit operator IValue[] (ArrayValue a)
        {
            return a.Value.ToArray ();
        }

        /// <param name="a">Array.</param>
        public static implicit operator ArrayValue (IValue[] a)
        {
            return new ArrayValue (a.ToList ());
        }

        /// <param name="a">Array Value.</param>
        public static implicit operator List<IValue> (ArrayValue a)
        {
            return a.Value.ToList ();
        }

        /// <param name="a">List.</param>
        public static implicit operator ArrayValue (List<IValue> a)
        {
            return new ArrayValue (a);
        }
    }
}

