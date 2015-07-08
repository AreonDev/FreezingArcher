//
//  FreezingArcherInput.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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

using Gwen;
using Gwen.Control;

namespace FreezingArcher.UI.Input
{
    public class FreezingArcherInput : Messaging.Interfaces.IMessageConsumer
    {
        private Canvas m_Canvas = null;
        private MessageManager MessageManager;

        private int m_MouseX;
        private int m_MouseY;

        public FreezingArcherInput(MessageManager mssgmngr)
        {
            MessageManager = mssgmngr;

            ValidMessages = new int[] { (int)Messaging.MessageId.Input };
            mssgmngr += this;
        }

        public void Initialize(Canvas c)
        {
            m_Canvas = c;
        }

        public void ConsumeMessage(Messaging.Interfaces.IMessage msg)
        {
            Messaging.InputMessage input = msg as Messaging.InputMessage;
            if (input != null)
            {
                int ButtonID = -1;
            }
        }

        public int[] ValidMessages
        {
            get;
            private set;
        }
    }
}

