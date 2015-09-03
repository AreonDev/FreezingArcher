//
//  LoadingScreen.cs
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
using FreezingArcher.Content;
using System.Collections.Generic;
using FreezingArcher.Output;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Core;
using Gwen.Control;

namespace FreezingArcher.Game
{
    public sealed class LoadingScreen : IMessageConsumer
    {
        public LoadingScreen (Application application, MessageProvider messageProvider, string backgroundPath,
            string name,
            IEnumerable<Tuple<string, GameStateTransition>> from = null,
            IEnumerable<Tuple<string, GameStateTransition>> to = null)
        {
            if (application.Game.AddGameState(name, Content.Environment.Default, from, to))
                LoadingState = application.Game.GetGameState(name);
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "LoadingScreen", "Could not add loading screen game state!");
                return;
            }

            ValidMessages = new[] { (int) MessageId.WindowResize };
            messageProvider += this;

            image = new ImagePanel(application.RendererContext.Canvas);
            image.ImageName = backgroundPath;
            image.Width = application.Window.Size.X;
            image.Height = application.Window.Size.Y;
            image.BringToFront();
        }

        public void Ready()
        {
            image.Parent.RemoveChild (image, true);
        }

        public void BringToFront ()
        {
            image.BringToFront();
        }

        public GameState LoadingState { get; private set; }

        readonly ImagePanel image;

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.WindowResize) 
            {
                WindowResizeMessage wrm = msg as WindowResizeMessage;

                image.SetBounds(new System.Drawing.Rectangle(0, 0, wrm.Width, wrm.Height));
            }
        }

        public int[] ValidMessages { get; private set; }
    }
}
