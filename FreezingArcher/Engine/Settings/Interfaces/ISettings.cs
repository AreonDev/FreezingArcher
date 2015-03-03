using System.Collections;
using System.Collections.Generic;

namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Settings.
    /// </summary>
    public interface ISettings : ICfgString, ISettingsChangedEvent, IComment, IEnumerable
    {
        /// <summary>
        /// Get or set the groups in this settings instance.
        /// </summary>
        /// <value>The groups.</value>
        List<IGroup> Groups { get; set; }

        /// <summary>
        /// Get or set the location of this settings instance.
        /// </summary>
        /// <value>The URL.</value>
        string URL { get; set; }

        /// <summary>
        /// Save the settings to the specified location.
        /// </summary>
        /// <param name="url">The URL.</param>
        void Save (string url = null);

        /// <summary>
        /// Load this settings instance from the url.
        /// </summary>
        void Load ();

        /// <summary>
        /// Sets to defaults.
        /// </summary>
        void SetToDefaults ();

        /// <summary>
        /// Get a group by its name.
        /// </summary>
        /// <returns>The group.</returns>
        /// <param name="name">The name.</param>
        IGroup GetGroupByName (string name);

        /// <summary>
        /// Get the <see cref="FreezingArcher.Settings.Interfaces.ISettings"/> by the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        IGroup this[string name] { get; }
    }
}
