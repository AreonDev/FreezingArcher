
namespace FreezingArcher.Messaging.Interfaces
{
    /// <summary>
    /// Interface for Messages which can be forwarded using the MessageManager
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        object Source { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        object Destination { get; set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        int MessageId { get; }
    }
}
