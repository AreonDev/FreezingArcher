//
//  MessageManager.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//       Willy Failla <wfailla@wfailla.de>
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using System.Collections.Generic;
using System.Threading;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;
using System.Collections.Concurrent;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Class for passing messages from IMessageProducers to IMessageConsumers
    /// A n-to-m relationship can be modeled.
    /// </summary>
    public class MessageManager : MessageProvider
    {
        readonly Thread messageThread;

        readonly ConcurrentQueue<IMessage> MessageQueue;

        static MessageManager()
        {
            ClassName = "MessageManager";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.MessageManager"/> class.
        /// </summary>
        public MessageManager() : base(null)
        {
            messageThread = new Thread(Process);
            MessageQueue = new ConcurrentQueue<IMessage>();
        }

        /// <summary>
        /// Handles the message created event.
        /// </summary>
        /// <param name="message">Message.</param>
        internal override void HandleMessageCreated (IMessage message)
        {
            if (Running)
                MessageQueue.Enqueue (message);
        }

        /// <summary>
        /// Starts the processing of messages.
        /// </summary>
        public override void StartProcessing ()
        {
            base.StartProcessing();
            messageThread.Start ();
        }

        int pauseTime;

        /// <summary>
        /// Pauses the processing.
        /// <remarks>>Not implemented!</remarks>
        /// </summary>
        /// <param name="time">Time.</param>
        public override void PauseProcessing (int time)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Pausing processing messages ...");
            pauseTime = time;
        }

        /// <summary>
        /// Process messages.
        /// </summary>
        void Process ()
        {
            while (Running)
            {
                while (DeferredUnregisters.Count > 0)
                {
                    Action tmp;
                    if (DeferredUnregisters.TryDequeue (out tmp))
                    {
                        tmp ();
                    }
                }

                while (DeferredRegisters.Count > 0)
                {
                    Action tmp;
                    if (DeferredRegisters.TryDequeue (out tmp))
                    {
                        tmp ();
                    }
                }

                if (MessageQueue.Count > 0)
                {
                    IMessage Message = null;
                    while (!MessageQueue.TryDequeue(out Message)) {}
                    List<IMessageConsumer> tmp;
                    if (MessageList.TryGetValue (Message.MessageId, out tmp))
                    {
                        tmp.ForEach (i => i.ConsumeMessage (Message));
                    }
                    if (MessageList.TryGetValue((int) MessageId.All, out tmp))
                    {
                        tmp.ForEach(i => i.ConsumeMessage(Message));
                    }
                }
                else
                    Thread.Sleep (5);

                if (pauseTime > 0)
                    Thread.Sleep (new TimeSpan(0, 0, pauseTime));
                
                pauseTime = 0;
            }

            //ensure queue is empty
            while (!MessageQueue.IsEmpty)
            {
                IMessage tmp;
                MessageQueue.TryDequeue (out tmp);
            }
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
    }
}
