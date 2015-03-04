using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Occurs when a message is sent or forwarded
    /// </summary>
    public delegate void MessageEvent (IMessage m);
}
