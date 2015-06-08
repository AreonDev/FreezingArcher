//
//  MessageManager.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//       Willy Failla <wfailla@wfailla.de>
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
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Class for passing messages from IMessageProducers to IMessageConsumers
    /// A n-to-m relationship can be modeled.
    /// </summary>
    public class MessageManager
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "MessageManager";

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
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Creating new message manager");
            messageThread = new Thread (FlushQueue);
            mc = new Queue<IMessage> (2000);
        }

        /// <summary>
        /// Registers the message consumer.
        /// </summary>
        /// <param name="m">Message Consumer to register</param>
        public void RegisterMessageConsumer (IMessageConsumer m)
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Registering new message consumer '{0}'",
                m.GetType ().ToString ());
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
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Removing message consumer '{0}'",
                m.GetType ().ToString ());
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
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Adding message creator '{0}'",
                c.GetType ().ToString ());
            c.MessageCreated -= HandleMessageCreated;
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
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Starting processing messages ...");
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
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Stopping processing messages ...");
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
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Pausing processing messages ...");
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
