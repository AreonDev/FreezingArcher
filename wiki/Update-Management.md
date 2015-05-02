---
layout: wikipage
title: Update Management
wikiPageName: Update-Management
menu: wiki
---

```c#
using FreezingArcher.Messaging
```

In FreezingArcher all game logik and computations should be done in a special environment. In this environment the user
is cannot effect the rendering performance as the user operates in special thread. To get into this environment you
must be a `MessageConsumer` consuming messages of the type `FreezingArcher.Messaging.MessageId.Update`.

The message consumer shall look like this:

```c#
public class Updatable : IMessageConsumer
{
    public Updatable (MessageManager msgMnr)
    {
        ValidMessage = new int[] { (int) MessageId.Update };
        msgMnr += this;
    }
    
    #region IMessageConsumer implementation
    
    public int[] ValidMessages { get; protected set; }
    
    public void ConsumeMessage (IMessage msg)
    {
        // update data
    }
    
    #endregion
}
```

The `ConsumeMessage` method is called with every update. An update occurs every 32 miliseconds.

