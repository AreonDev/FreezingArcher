//
//  MessageProxy.cs
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
using System.Linq;
using FreezingArcher.Messaging.Interfaces;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FreezingArcher.Messaging
{
    public sealed class MessageProxy : MessageProvider, IMessageConsumer
    {
        static MessageProxy()
        {
            ClassName = "MessageProxy";
        }

        public MessageProxy(MessageProvider parentProvider) : base(parentProvider)
        {
            ValidMessages = new[] { (int) MessageId.All };
            parentProvider += this;
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage(IMessage msg)
        {
            if (!Running)
                return;
            
            if (messageIdBlacklist.Contains(msg.MessageId))
                return;

            foreach (var filter in messageFilters)
                if (filter(msg.MessageId))
                    return;

            List<IMessageConsumer> tmp;
            lock (MessageList)
            {
                if (MessageList.TryGetValue((int) MessageId.All, out tmp))
                {
                    lock (tmp)
                        tmp.ForEach(i => i.ConsumeMessage(msg));
                }
                if (MessageList.TryGetValue(msg.MessageId, out tmp))
                {
                    lock (tmp)
                        tmp.ForEach(i => i.ConsumeMessage(msg));
                }
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion

        internal override void HandleMessageCreated (IMessage message)
        {
            if (!Running)
                return;

            if (messageIdBlacklist.Contains(message.MessageId))
                return;

            foreach (var filter in messageFilters)
                if (filter(message.MessageId))
                    return;

            parentProvider.HandleMessageCreated(message);
        }

        readonly List<int> messageIdBlacklist = new List<int>();

        public void AddToBlacklist(params int[] messages)
        {
            messageIdBlacklist.AddRange(messages);
        }

        public void RemoveFromBlacklist(params int[] messages)
        {
            messageIdBlacklist.RemoveAll(m => messages.Contains(m));
        }

        readonly List<Func<int, bool>> messageFilters = new List<Func<int, bool>>();

        public int AddFilter(Func<int, bool> filter)
        {
            messageFilters.Add(filter);
            return messageFilters.IndexOf(filter);
        }

        public void RemoveFilter(int filterIdentifier)
        {
            messageFilters.RemoveAt(filterIdentifier);
        }

        /// <param name="j">MessageManager to register object to</param>
        /// <param name="o">Object to register</param>
        public static MessageProxy operator + (MessageProxy j, object o)
        {
            if (o is IMessageConsumer)
                j.RegisterMessageConsumer (o as IMessageConsumer);

            if (o is IMessageCreator)
                j.AddMessageCreator (o as IMessageCreator);
            return j;
        }
    }
}
