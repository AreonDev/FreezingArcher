using System;
using System.Collections.Generic;
using System.Threading;
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    public class MessageManager
    {
        readonly Dictionary<int, List<IMessageConsumer>> messageList = new Dictionary<int, List<IMessageConsumer>> ();
        Thread messageThread = null;
        bool run = false;
        //default does not work
        readonly Queue<IMessage> mc = null;

        public MessageManager ()
        {
            messageThread = new Thread (FlushQueue);
            mc = new Queue<IMessage> (2000);
        }

        public void RegisterMessageConsumer (IMessageConsumer m)
        {
            m.ValidMessages.ForEach (i =>
            {
                List<IMessageConsumer> tmp = null;
                if (messageList.TryGetValue (i, out tmp))
                    tmp.Add (m);
                else
                {
                    tmp = new List<IMessageConsumer> (m.ToCollection ());
                    messageList [i] = tmp;
                }
            });
        }

        public void UnregisterMessageConsumer (IMessageConsumer m)
        {
            m.ValidMessages.ForEach (i =>
            {
                List<IMessageConsumer> tmp = null;
                if (messageList.TryGetValue (i, out tmp))
                    tmp.Remove (m);
            });
        }

        public void AddMessageCreator (IMessageCreator c)
        {
            c.MessageCreated += HandleMessageCreated;
        }

        void HandleMessageCreated (IMessage m)
        {
            lock (mc)
                mc.Enqueue (m);

        }

        public void StartProcessing ()
        {
            if (run)
                return;
            run = true;
            messageThread.Start ();
        }

        public void StopProcessing ()
        {
            if (!run)
                return;
            run = false;
        }

        public void PauseProcessing (int time)
        {
            //MessageThread.Sleep (Time);TODO
            throw new NotImplementedException ();
        }

        public static MessageManager operator + (MessageManager j, object o)
        {
            if (o is IMessageConsumer)
                j.RegisterMessageConsumer (o as IMessageConsumer);

            if (o is IMessageCreator)
                j.AddMessageCreator (o as IMessageCreator);
            return j;
        }

        public void FlushQueue ()
        {
            while (run)
            {
                if (mc.Count != 0)
                {
                    IMessage Message = null;
                    lock (mc)
                        Message = mc.Dequeue ();
                    List<IMessageConsumer> tmp = null;
                    if (messageList.TryGetValue (Message.MessageId, out tmp))
                        tmp.ForEach (i => i.ConsumeMessage (Message));
                }
                else
                    Thread.Sleep (5);
            }
            //ensure queue is empty
            mc.Clear ();
        }
    }
}
