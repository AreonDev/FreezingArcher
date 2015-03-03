
namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Value interface.
    /// </summary>
    public interface IValue : ICfgString, ISettingsRoot
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Clone this instance of a value.
        /// </summary>
        IValue Clone ();
    }
}
