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
using FreezingArcher.Input;

using Gwen;
using Gwen.Control;

namespace FreezingArcher.UI.Input
{
    public class FreezingArcherInput : Messaging.Interfaces.IMessageConsumer
    {
        private Canvas m_Canvas = null;
        private MessageProvider MessageManager;

        private int m_MouseX;
        private int m_MouseY;

        public FreezingArcherInput(MessageProvider mssgmngr)
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
            if (msg.MessageId == (int) MessageId.Input)
            {
                var input = msg as Messaging.InputMessage;
                int dx = (int)input.MousePosition.X - m_MouseX;
                int dy = (int)input.MousePosition.Y - m_MouseY;

                m_MouseX = (int)input.MousePosition.X;
                m_MouseY = (int)input.MousePosition.Y;

                m_Canvas.Input_MouseMoved(m_MouseX, m_MouseY, dx, dy);

                m_Canvas.Input_MouseButton(0, input.IsMouseButtonDown(Pencil.Gaming.MouseButton.LeftButton));
                m_Canvas.Input_MouseButton(1, input.IsMouseButtonPressed(Pencil.Gaming.MouseButton.RightButton));
                m_Canvas.Input_MouseButton(2, input.IsMouseButtonPressed(Pencil.Gaming.MouseButton.MiddleButton));
            }
        }

        public int[] ValidMessages
        {
            get;
            private set;
        }
    }
}

