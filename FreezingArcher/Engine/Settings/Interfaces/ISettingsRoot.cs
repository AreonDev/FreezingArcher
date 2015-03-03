
namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Settings root instance interface.
    /// </summary>
    public interface ISettingsRoot
    {
        /// <summary>
        /// Get or set the root settings instance.
        /// </summary>
        /// <value>The root setting.</value>
        ISettings RootSettings { get; set; }
    }
}
