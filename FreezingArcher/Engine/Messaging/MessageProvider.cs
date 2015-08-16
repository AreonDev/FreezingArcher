//
//  MessageProvider.cs
//
//  Author:
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
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;
using System.Collections.Concurrent;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Class for passing messages from IMessageCreators to IMessageConsumers
    /// A n-to-m relationship can be modeled.
    /// </summary>
    public abstract class MessageProvider
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static string ClassName = "MessageProvider";

        /// <summary>
        /// The message list mapping message ids to a list of message consumers.
        /// </summary>
        protected readonly SortedDictionary<int, List<IMessageConsumer>> MessageList =
            new SortedDictionary<int, List<IMessageConsumer>> ();

        /// <summary>
        /// A flag indicating whether this instance of a message provider is running or not.
        /// </summary>
        public bool Running { get; protected set; }

        /// <summary>
        /// The parent message provider.
        /// </summary>
        protected readonly MessageProvider parentProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.MessageProvider"/> class.
        /// </summary>
        /// <param name="parentProvider">Parent provider.</param>
        protected MessageProvider(MessageProvider parentProvider)
        {
            Logger.Log.AddLogEntry(LogLevel.Fine, ClassName, "Creating new {0}", convert(ClassName));
            this.parentProvider = parentProvider;
        }

        /// <summary>
        /// Convert 'ExampleString' to 'example string'.
        /// </summary>
        /// <param name="name">Name.</param>
        static string convert(string name)
        {
            string res = string.Empty;

            foreach(var c in name)
            {
                if (char.IsUpper(c))
                    res += " " + char.ToLower(c);
                else
                    res += c;
            }

            return res.Trim();
        }

        /// <summary>
        /// Registers the message consumer.
        /// </summary>
        /// <param name="consumer">Consumer.</param>
        public void RegisterMessageConsumer (IMessageConsumer consumer)
        {
            Logger.Log.AddLogEntry(LogLevel.Fine, ClassName, "Registering new message consumer '{0}'",
                consumer.GetType().ToString());

            DeferredRegisters.Enqueue (() => consumer.ValidMessages.ForEach (i =>
                {
                    List<IMessageConsumer> tmp;
                    if (MessageList.TryGetValue(i, out tmp))
                        tmp.Add(consumer);
                    else
                    {
                        tmp = new List<IMessageConsumer>(consumer.ToCollection());
                        MessageList[i] = tmp;
                    }
                })
            );
        }

        /// <summary>
        /// Unregisters the message consumer.
        /// </summary>
        /// <param name="consumer">Consumer.</param>
        public void UnregisterMessageConsumer(IMessageConsumer consumer)
        {
            Logger.Log.AddLogEntry(LogLevel.Fine, ClassName, "Removing message consumer '{0}'",
                consumer.GetType().ToString());

            DeferredUnregisters.Enqueue(() => consumer.ValidMessages.ForEach(i =>
                {
                    List<IMessageConsumer> tmp;
                    if (MessageList.TryGetValue(i, out tmp))
                    {
                        tmp.Remove(consumer);
                    }
                })
            );
        }

        protected ConcurrentQueue<Action> DeferredUnregisters = new ConcurrentQueue<Action>();
        protected ConcurrentQueue<Action> DeferredRegisters = new ConcurrentQueue<Action>();

        /// <summary>
        /// Adds the message creator.
        /// </summary>
        /// <param name="creator">Creator.</param>
        public void AddMessageCreator(IMessageCreator creator)
        {
            Logger.Log.AddLogEntry(LogLevel.Fine, ClassName, "Adding new message creator '{0}'",
                creator.GetType().ToString());

            creator.MessageCreated -= HandleMessageCreated;
            creator.MessageCreated += HandleMessageCreated;
        }

        /// <summary>
        /// Removes the message creator.
        /// </summary>
        /// <param name="creator">Creator.</param>
        public void RemoveMessageCreator(IMessageCreator creator)
        {
            Logger.Log.AddLogEntry(LogLevel.Fine, ClassName, "Removing message creator '{0}'",
                creator.GetType().ToString());

            creator.MessageCreated -= HandleMessageCreated;
        }

        /// <summary>
        /// Handles the message created event.
        /// </summary>
        /// <param name="message">Message.</param>
        internal virtual void HandleMessageCreated (IMessage message)
        {
            if (Running)
                parentProvider.HandleMessageCreated (message);
        }

        /// <summary>
        /// Starts the processing.
        /// </summary>
        public virtual void StartProcessing()
        {
            Logger.Log.AddLogEntry(LogLevel.Debug, ClassName, "Starting processing messages...");

            if (Running)
                return;
            Running = true;
        }

        /// <summary>
        /// Stops the processing.
        /// </summary>
        public virtual void StopProcessing ()
        {
            Logger.Log.AddLogEntry(LogLevel.Debug, ClassName, "Stopping processing messages...");

            if (!Running)
                return;

            Running = false;
        }

        /// <summary>
        /// Pauses the processing.
        /// </summary>
        /// <param name="time">Time.</param>
        public virtual void PauseProcessing (int time)
        {
            Logger.Log.AddLogEntry(LogLevel.Debug, ClassName, "Pausing processing messages..." );
            Running = false;

            Action pauseAction = () => {
                Running = true;
            };

            pauseAction.RunAfter(new TimeSpan(0, 0, time));
        }

        /// <param name="j">MessageManager to register object to</param>
        /// <param name="o">Object to register</param>
        public static MessageProvider operator + (MessageProvider j, object o)
        {
            if (o is IMessageConsumer)
                j.RegisterMessageConsumer (o as IMessageConsumer);

            if (o is IMessageCreator)
                j.AddMessageCreator (o as IMessageCreator);
            return j;
        }

        /// <param name="j">MessageManager to remove object from.</param>
        /// <param name="o">Object to remove.</param>
        public static MessageProvider operator - (MessageProvider j, object o)
        {
            if (o is IMessageConsumer)
                j.UnregisterMessageConsumer(o as IMessageConsumer);

            if (o is IMessageCreator)
                j.RemoveMessageCreator(o as IMessageCreator);

            return j;
        }
    }
}
