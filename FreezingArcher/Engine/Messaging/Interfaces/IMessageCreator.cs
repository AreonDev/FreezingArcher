
namespace FreezingArcher.Messaging.Interfaces
{
    public interface IMessageCreator
    {
        event MessageEvent MessageCreated;

        int[] ValidMessages { get; }
    }
}
