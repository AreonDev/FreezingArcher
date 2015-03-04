using System;
using System.Collections.Generic;
using System.Threading;
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Class for passing messages from IMessageProducers to IMessageConsumers
    /// A n-to-m relationship can be modeled.
    /// <remarks>>
    /// SCHEIß DOKUMENTATION - Fin hat mich dazu gezwungen!!!
    /// </remarks>
    /// </summary>
    public class MessageManager
    {
        readonly Dictionary<int, List<IMessageConsumer>> messageList = new Dictionary<int, List<IMessageConsumer>> ();
        Thread messageThread = null;
        bool run = false;
        //default does not work
        readonly Queue<IMessage> mc = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.MessageManager"/> class.
        /// </summary>
        public MessageManager ()
        {
            messageThread = new Thread (FlushQueue);
            mc = new Queue<IMessage> (2000);
        }

        /// <summary>
        /// Registers the message consumer.
        /// </summary>
        /// <param name="m">Message Consumer to register</param>
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

        /// <summary>
        /// Unregisters the message consumer.
        /// </summary>
        /// <param name="m">Message Consumer to unregister</param>
        public void UnregisterMessageConsumer (IMessageConsumer m)
        {
            m.ValidMessages.ForEach (i =>
            {
                List<IMessageConsumer> tmp = null;
                if (messageList.TryGetValue (i, out tmp))
                    tmp.Remove (m);
            });
        }

        /// <summary>
        /// Adds the message creator.
        /// </summary>
        /// <param name="c">Message Creator to add</param>
        public void AddMessageCreator (IMessageCreator c)
        {
            c.MessageCreated += HandleMessageCreated;
        }

        void HandleMessageCreated (IMessage m)
        {
            lock (mc)
                mc.Enqueue (m);

        }

        /// <summary>
        /// Starts the processing of messages.
        /// </summary>
        public void StartProcessing ()
        {
            if (run)
                return;
            run = true;
            messageThread.Start ();
        }

        /// <summary>
        /// Stops the processing of messages.
        /// </summary>
        public void StopProcessing ()
        {
            if (!run)
                return;
            run = false;
        }

        /// <summary>
        /// Pauses the processing.
        /// <remarks>>Not implemented!</remarks>
        /// </summary>
        /// <param name="time">Time.</param>
        public void PauseProcessing (int time)
        {
            //MessageThread.Sleep (Time);TODO
            throw new NotImplementedException ();
        }

        /// <param name="j">MessageManager to register object to</param>
        /// <param name="o">Object to register</param>
        public static MessageManager operator + (MessageManager j, object o)
        {
            if (o is IMessageConsumer)
                j.RegisterMessageConsumer (o as IMessageConsumer);

            if (o is IMessageCreator)
                j.AddMessageCreator (o as IMessageCreator);
            return j;
        }

        /// <summary>
        /// Flushs the queue.
        /// </summary>
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
