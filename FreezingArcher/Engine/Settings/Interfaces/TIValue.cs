
namespace FreezingArcher.Settings.Interfaces
{
    public interface IValue<T> : IValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        new T Value { get; set; }

        /// <summary>
        /// Clone this instance of a value.
        /// </summary>
        new IValue<T> Clone ();
    }
}
