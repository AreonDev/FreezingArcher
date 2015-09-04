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

            this.application = application;

            ValidMessages = new[] { (int) MessageId.WindowResize };
            messageProvider += this;

            image = new ImagePanel(application.RendererContext.Canvas);
            image.ImageName = backgroundPath;
            updateImage();
            image.BringToFront();

            progressBar = new ProgressBar (image);
            progressBar.IsHorizontal = true;
            progressBar.AutoLabel = false;
            progressBar.Y = application.RendererContext.Canvas.Height - 70;
            progressBar.X = 40 + (image.Width - application.RendererContext.Canvas.Width) / 2;
            progressBar.Height = 30;
            progressBar.Width = application.RendererContext.Canvas.Width - 80;
        }

        readonly Application application;
        readonly ProgressBar progressBar;

        public void Ready()
        {
            image.Parent.RemoveChild (image, true);
        }

        public void BringToFront ()
        {
            image.BringToFront();
        }

        public void UpdateProgress (float progressDelta, string message)
        {
            progressBar.Value += progressDelta;
            progressBar.SetText(message);
        }

        void updateImage ()
        {
            int width, height;
            if (application.RendererContext.Canvas.Height * 16 / 9 > application.RendererContext.Canvas.Width)
            {
                width = application.RendererContext.Canvas.Height * 16 / 9;
                height = application.RendererContext.Canvas.Height;
            }
            else
            {
                width = application.RendererContext.Canvas.Width;
                height = application.RendererContext.Canvas.Width * 9 / 16;
            }
            image.Width = width;
            image.Height = height;
            image.X = (application.RendererContext.Canvas.Width - width) / 2;
            image.Y = (application.RendererContext.Canvas.Height - height) / 2;
        }

        public GameState LoadingState { get; private set; }

        readonly ImagePanel image;

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.WindowResize) 
            {
                updateImage();
                progressBar.Y = application.RendererContext.Canvas.Height - 70;
                progressBar.X = 40 + (image.Width - application.RendererContext.Canvas.Width) / 2;
                progressBar.Width = application.RendererContext.Canvas.Width - 80;
            }
        }

        public int[] ValidMessages { get; private set; }
    }
}
