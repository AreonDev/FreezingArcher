---
layout: wikipage
title: Messaging System
wikiPageName: Messaging-System
menu: wiki
---

```c#
namespace FreezingArcher.Messaging
```

The messaging system is used to communicate between the components. The `MessageManager` manages the communication.

## Example usage:

```c#
class ItemProducedMessage : IMessage
{
    public ItemProducedMessage (string item) { Item = item; }

    public string Item { get; protected set; }

    #region IMessage implementation
    public object Source { get; set; }
    public object Destination { get; set; }
    public int MessageId { get { return 42; }} // return unique identifier for this message type
    #endregion
}

class Producer : IMessageCreator
{
    public Producer (..., MessageManager msgMnr)
    {
        msgMnr += this;
        ...
    }
    ...
    public void Produce ()
    {
        if (MessageCreated != null)
            MessageCreated (new ItemProducedMessage ("item"));
    }
    ...
    public event MessageEvent MessageCreated;
}

class Consumer : IMessageConsumer
{
    public Consumer (..., MessageManager msgMnr)
    {
        ValidMessages = new int[] { 42 };
        msgMnr += this;
        ...
    }
    ...
    public void ConsumeMessage (IMessage msg)
    {
        ItemProducedMessage m = msg as ItemProducedMessage;
        if (m != null)
            Console.WriteLine (m.Item);
    }
    ...
    // array containing which messages this consumer accepts
    public int[] ValidMessages { get; protected set; }
}
```

