
namespace FreezingArcher.Messaging.Interfaces
{
    public interface IMessage
    {
        object Source { get; set; }
        object Destination { get; set; }
        int MessageId { get; }
    }
}
