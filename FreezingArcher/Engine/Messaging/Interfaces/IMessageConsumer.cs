
namespace FreezingArcher.Messaging.Interfaces
{
    /// <summary>
    /// Defines an interface for objects which can consume messages.
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Processes the invoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        void ConsumeMessage (IMessage msg);

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        int[] ValidMessages { get; }
    }
}
