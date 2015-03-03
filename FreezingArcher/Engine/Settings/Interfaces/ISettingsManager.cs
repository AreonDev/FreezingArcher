using System.Collections;
using System.Collections.Generic;

namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Settings Manager.
    /// </summary>
    public interface ISettingsManager : ISettingsChangedEvent, IEnumerable
    {
        /// <summary>
        /// Get a list of settings managed by this manager.
        /// </summary>
        /// <value>The settings.</value>
        List<ISettings> Settings { get; }

        /// <summary>
        /// Load all settings by their url.
        /// </summary>
        void LoadAll ();

        /// <summary>
        /// Save all settings to their url.
        /// </summary>
        void SaveAll ();

        /// <summary>
        /// Add a setting to the manager.
        /// </summary>
        /// <param name="settings">The setting.</param>
        void Add (ISettings settings);

        /// <summary>
        /// Add a setting to the manager by its url.
        /// </summary>
        /// <param name="url">The URL.</param>
        void Add (string url);

        /// <summary>
        /// Remove the specified setting.
        /// </summary>
        /// <param name="settings">The setting.</param>
        void Remove (ISettings settings);

        /// <summary>
        /// Remove a setting by its url.
        /// </summary>
        /// <param name="url">The URL.</param>
        void Remove (string url);

        /// <summary>
        /// Prints all settings as a config string to the console. The output is equal to what would be saved on Save().
        /// </summary>
        void Print ();

        /// <summary>
        /// Get a setting by its URL.
        /// </summary>
        /// <returns>The setting.</returns>
        /// <param name="url">The URL.</param>
        ISettings GetSettingsByURL (string url);

        /// <summary>
        /// Get the <see cref="FreezingArcher.Settings.Interfaces.ISettingsManager"/> by the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        ISettings this[string url] { get; }
    }
}
