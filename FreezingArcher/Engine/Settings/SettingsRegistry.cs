using System;
using System.Collections.Generic;
using System.Linq;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Settings registry.
    /// </summary>
    public static class SettingsRegistry
    {
        static SettingsRegistry ()
        {}

        static readonly Dictionary<string, Dictionary<string, RegistryEntry>> Registry =
            new Dictionary<string, Dictionary<string, RegistryEntry>> ();

        /// <summary>
        /// Add the specified property with the specified type to the specified group.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="property">Property.</param>
        /// <param name="type">Type.</param>
        public static void Add (string group, string property, string type, List<object> validValues = null)
        {
            PropertyTypes.Types UIType;
            if (Enum.TryParse (type, out UIType))
                Add (group, property, UIType, validValues);
            else
                throw new ArgumentOutOfRangeException ("type", "Property type of name {0} was not found!", type);
        }

        /// <summary>
        /// Add the specified property with the specified type to the specified group.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="property">Property.</param>
        /// <param name="UIType">Type.</param>
        /// <param name="validValues">A list of valid values.</param>
        public static void Add (string group, string property, PropertyTypes.Types UIType,
                                List<object> validValues = null)
        {
            if (group == null)
                throw new ArgumentNullException ("group", "The group must not be null!");
            if (group.Length == 0)
                throw new ArgumentOutOfRangeException ("group", "The group name must not be of length 0!");
            if (property == null)
                throw new ArgumentNullException ("property", "The property must not be null!");
            if (property.Length == 0)
                throw new ArgumentOutOfRangeException ("property", "The property name must not be of length 0!");

            Dictionary<string, RegistryEntry> dic;
            if (!Registry.TryGetValue (group, out dic))
            {
                dic = new Dictionary<string, RegistryEntry> ();
                Registry.Add (group, dic);
            }
            dic.Add (property, new RegistryEntry (UIType, validValues));
        }

        /// <summary>
        /// Remove the specified property in the specified group.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="property">Property.</param>
        public static void Remove (string group, string property)
        {
            if (string.IsNullOrEmpty (group))
                return;
            if (string.IsNullOrEmpty (property))
                return;

            Registry[group].Remove (property);
            if (Registry[group].Count == 0)
                Registry.Remove (group);
        }

        /// <summary>
        /// Get the specified property in the specified group.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="property">Property.</param>
        public static RegistryEntry Get (string group, IProperty property)
        {
            Dictionary<string, RegistryEntry> group_l = null;
            RegistryEntry entry = null;
            if (Registry.TryGetValue (group, out group_l))
                group_l.TryGetValue (property.Name, out entry);

            PropertyTypes.Types[] valid;
            if (!PropertyTypes.TypeRegistry.TryGetValue (property.Value.GetType (), out valid) || valid.Length == 0)
                throw new ArgumentOutOfRangeException ("property",
                                                       "The value type of the property is not " +
                                                       "registered in the type registry!");

            entry = entry != null && valid.Contains (entry.Type) ? entry : new RegistryEntry (valid[0]);
            return entry;
        }
    }

    public class RegistryEntry
    {
        public RegistryEntry (PropertyTypes.Types type, List<object> validValues = null)
        {
            Type = type;
            ValidValues = validValues ?? new List<object> ();
        }

        public PropertyTypes.Types Type { get; set; }
        public List<object> ValidValues { get; set; }
    }
}
