
namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Configuration string interface.
    /// </summary>
    public interface ICfgString
    {
        /// <summary>
        /// Get the item as a saveable string for the config file.
        /// </summary>
        /// <value>The configuration string.</value>
        string CfgString { get; }
    }
}
