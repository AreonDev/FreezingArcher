using System;

namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Settings changed event.
    /// </summary>
    public interface ISettingsChangedEvent
    {
        /// <summary>
        /// This Event is fired when one property/value changes in a settings instance.
        /// </summary>
        /// <value>The handler to handle this event.</value>
        EventHandler<SettingsChangedEventArgs> SettingsChanged { get; set; }
    }
}
