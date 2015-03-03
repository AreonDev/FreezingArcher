
namespace FreezingArcher.Messaging.Interfaces
{
    public interface IMessageConsumer
    {
        void ConsumeMessage (IMessage msg);

        int[] ValidMessages { get; }
    }
}
