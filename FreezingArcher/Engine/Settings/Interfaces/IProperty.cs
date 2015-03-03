
namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Property
    /// </summary>
    public interface IProperty : ICfgString, ISettingsRoot, IComment
    {
        /// <summary>
        /// Get or set the value of this property.
        /// </summary>
        /// <value>The value.</value>
        IValue Value { get; set; }

        /// <summary>
        /// Get or set the name of this property.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Get or set the line comment.
        /// </summary>
        /// <value>The line comment.</value>
        string LineComment { get; set; }
    }
}
