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
using System.Linq;
using Pencil.Gaming;

namespace FreezingArcher.UI.Input
{
    public class FreezingArcherInput : Messaging.Interfaces.IMessageConsumer
    {
        Canvas m_Canvas;

        public FreezingArcherInput(MessageProvider messageProvider)
        {
            ValidMessages = new int[] { (int) MessageId.Input };
            messageProvider += this;
        }

        public void Initialize(Canvas c)
        {
            m_Canvas = c;
        }

        public void ConsumeMessage(Messaging.Interfaces.IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Input)
            {
                var input = msg as InputMessage;

                m_Canvas.Input_MouseMoved(
                    (int) input.MousePosition.X, (int) input.MousePosition.Y,
                    (int) input.MouseMovement.X, (int) input.MouseMovement.Y);

                if (input.Mouse.Any(m => m.Action == KeyAction.Press && m.Button == MouseButton.LeftButton))
                    m_Canvas.Input_MouseButton(0, true);
                if (input.Mouse.Any(m => m.Action == KeyAction.Release && m.Button == MouseButton.LeftButton))
                    m_Canvas.Input_MouseButton(0, false);

                if (input.Mouse.Any(m => m.Action == KeyAction.Press && m.Button == MouseButton.MiddleButton))
                    m_Canvas.Input_MouseButton(1, true);
                if (input.Mouse.Any(m => m.Action == KeyAction.Release && m.Button == MouseButton.MiddleButton))
                    m_Canvas.Input_MouseButton(1, false);

                if (input.Mouse.Any(m => m.Action == KeyAction.Press && m.Button == MouseButton.RightButton))
                    m_Canvas.Input_MouseButton(2, true);
                if (input.Mouse.Any(m => m.Action == KeyAction.Release && m.Button == MouseButton.RightButton))
                    m_Canvas.Input_MouseButton(2, false);
            }
        }

        public int[] ValidMessages { get; private set; }
    }
}

