using System;

namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Settings changed event arguments.
    /// </summary>
    public class SettingsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The setting which fired this event.
        /// </summary>
        /// <value>The setting.</value>
        public ISettings Settings { get; set; }
    }
}
