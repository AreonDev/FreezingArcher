//
//  MessagingTest.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;
using System.Threading;

namespace FreezingArcher.Game
{
    public class MessagingTest : IMessageCreator, IDisposable
    {
        public MessagingTest ()
        {
            messageManager = new MessageManager();

            messageManager += this;
            messageManager += new MessageConsumerExample("global");

            messageProxy1 = new MessageProxy(messageManager);
            messageProxy1 += new MessageConsumerExample("proxy1");

            messageProxy2 = new MessageProxy(messageManager);
            messageProxy2 += new MessageConsumerExample("proxy2");

            messageProxy3 = new MessageProxy(messageProxy2);
            messageProxy3 += new MessageConsumerExample("proxy3");

            messageProxy1.StartProcessing();
            messageProxy3.StartProcessing();
            messageProxy2.StartProcessing();
            messageManager.StartProcessing();

            Thread.Sleep(100);
            MessageCreated(new MessageExample("test1"));

            Thread.Sleep(100);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "Disabling proxy1");
            messageProxy1.AddToBlacklist(42);
            MessageCreated(new MessageExample("test2"));

            Thread.Sleep(100);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "Disabling proxy2");
            messageProxy2.AddToBlacklist(42);
            MessageCreated(new MessageExample("test3"));

            Thread.Sleep(100);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "Enabling proxy1 and proxy2");
            messageProxy1.RemoveFromBlacklist(42);
            messageProxy2.RemoveFromBlacklist(42);
            MessageCreated(new MessageExample("test4"));

            Thread.Sleep(100);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "Disabling proxy3");
            messageProxy3.AddToBlacklist(42);
            MessageCreated(new MessageExample("test5"));

            Thread.Sleep(100);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "Enabling proxy3");
            messageProxy3.RemoveFromBlacklist(42);
            MessageCreated(new MessageExample("test6"));

            Thread.Sleep(100);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "Stopping proxy2");
            messageProxy2.StopProcessing();
            MessageCreated(new MessageExample("test7"));

            Thread.Sleep(100);
        }

        #region IDisposable implementation

        public void Dispose ()
        {
            //messageProxy1.StopProcessing();
            //messageProxy3.StopProcessing();
            //messageProxy2.StopProcessing();
            messageManager.StopProcessing();
        }

        #endregion

        MessageManager messageManager;

        MessageProxy messageProxy1;

        MessageProxy messageProxy2;

        MessageProxy messageProxy3;

        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion

        class MessageConsumerExample : IMessageConsumer
        {
            public MessageConsumerExample(string prefix)
            {
                ValidMessages = new[] { 42 };
                this.prefix = prefix;
            }

            readonly string prefix;

            #region IMessageConsumer implementation

            public void ConsumeMessage (IMessage msg)
            {
                if (msg.MessageId == 42)
                {
                    var me = msg as MessageExample;
                    if (me != null)
                        Logger.Log.AddLogEntry(LogLevel.Debug, "MessagingTest", "{0}: {1}", prefix,
                            me.Message);
                }
            }

            public int[] ValidMessages { get; private set; }

            #endregion
        }

        class MessageExample : IMessage
        {
            public MessageExample(string message)
            {
                Message = message;
                MessageId = 42;
            }

            #region IMessage implementation

            public object Source { get; set; }

            public object Destination { get; set; }

            public int MessageId { get; private set; }

            #endregion

            public string Message;
        }
    }
}
