
namespace FreezingArcher.Messaging.Interfaces
{
    /// <summary>
    /// Defines an interface for objects which can create messages
    /// </summary>
    public interface IMessageCreator
    {
        /// <summary>
        /// Occurs when a new message is created an is ready for processing
        /// </summary>
        event MessageEvent MessageCreated;

        /// <summary>
        /// Gets the valid messages.
        /// </summary>
        /// <value>The valid messages.</value>
        int[] ValidMessages { get; }
    }
}
