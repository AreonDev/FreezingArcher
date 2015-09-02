//
//  EndScreen.cs
//
//  Author:
//       david <${AuthorEmail}>
//
//  Copyright (c) 2015 david
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

using FreezingArcher.Core;
using FreezingArcher.Content;
using FreezingArcher.Renderer;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;

using Gwen.Control;
using System.Collections.Generic;

namespace FreezingArcher.Game
{
    public class EndScreen : IMessageConsumer
    {
        Application Application;
        MessageProvider MessageProvider;
        RendererContext Renderer;
        Canvas Canvas;
        public GameState State { get; private set;}

        public Texture2D BackgroundTexture { get; set;}

        public EndScreen (Application application, RendererContext rc, IEnumerable<Tuple<string, GameStateTransition>> from = null)
        {
            Renderer = rc;

            Application = application;

            if (!Application.Game.AddGameState ("endscreen_state", FreezingArcher.Content.Environment.Default, from))
            {
                Logger.Log.AddLogEntry (LogLevel.Error, "EndScreen", "Could not add Endscreen-State!");
            }

            State = Application.Game.GetGameState ("endscreen_state");

            Canvas = Renderer.CreateCanvas ();
            //Renderer.SetCanvas (Canvas);

            MessageProvider = State.MessageProxy;

            BackgroundTexture = null;

            ValidMessages = new int [] { (int) MessageId.GameEnded, (int) MessageId.GameEndedDied };
            MessageProvider += this;

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(application, MessageProvider);
            input.Initialize (application.RendererContext.Canvas);

            Canvas.SetSize(application.Window.Size.X, application.Window.Size.Y);
            Canvas.ShouldDrawBackground = false;

            //Hier musst du deinen Stuff machen
        }



        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.GameEnded || msg.MessageId == (int)MessageId.GameEndedDied)
            {
                Application.Window.ReleaseMouse ();

                Renderer.SetCanvas (Canvas);
               

            }
        }

        public int[] ValidMessages{get;}

        #endregion
    }
}

