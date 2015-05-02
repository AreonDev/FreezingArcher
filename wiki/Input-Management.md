---
layout: wikipage
title: Input Management
wikiPageName: Input-Management
menu: wiki
---

```c#
using FreezingArcher.Input
```

In FreezingArcher all input is handles by an input manager that creates input messages every 16 miliseconds. To receive
input messages the user should implement a `IMessageConsumer` accepting `MessageId.Input`.

```c#
public class InputReceiver : IMessageConsumer
{
    public InputReceiver (MessageManager msgMnr)
    {
        ValidMessage = new int[] { (int) MessageId.Input };
        msgMnr += this;
    }
    
    #region IMessageConsumer implementation
    
    public int[] ValidMessages { get; protected set; }
    
    public void ConsumeMessage (IMessage msg)
    {
        InputMessage im = (InputMessage) msg;
        
        if (im.IsKeyPressed ("forward")
            goForward();
    }
    
    #endregion
}
```

The `ConsumeMessage` method is called with every input flush. The key mentioned in the `IsKeyPressed` method is
registered in the key registry where string identifiers are mapped to real keys.

Example usage of the `KeyRegistry`:

```c#
using Pencil.Gaming;
using FreezingArcher.Input;

KeyRegistry.Instance.RegisterOrUpdateKey ("custom-action", Key.C);
```

