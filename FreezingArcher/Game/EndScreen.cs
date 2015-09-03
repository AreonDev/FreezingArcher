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
using FreezingArcher.Localization;

namespace FreezingArcher.Game
{
    public class EndScreen : IMessageConsumer
    {
        readonly Application Application;
        MessageProvider MessageProvider;
        readonly RendererContext Renderer;
        readonly Canvas Canvas;
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

            ValidMessages = new int [] { (int) MessageId.GameEnded, (int) MessageId.GameEndedDied,
                (int) MessageId.WindowResize, (int) MessageId.UpdateLocale };
            MessageProvider += this;

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(application, application.MessageManager);
            input.Initialize (application.RendererContext.Canvas);

            Canvas.SetSize(application.Window.Size.X, application.Window.Size.Y);
            Canvas.ShouldDrawBackground = false;

            exitButton = new Button (Canvas);
            exitButton.Text = Localizer.Instance.GetValueForName("quit_game");
            exitButton.Width = 300;
            exitButton.X = (Canvas.Width - exitButton.Width) / 2;
            exitButton.Y = Canvas.Height - exitButton.Height - 20;
            exitButton.Clicked += (sender, arguments) => application.Window.Close ();

            surviveTimeLabel = new Label (Canvas);
            surviveTimeLabel.AutoSizeToContents = true;
            surviveTimeLabel.X = (Canvas.Width - surviveTimeLabel.Width) / 2;
            surviveTimeLabel.Y = exitButton.Y - surviveTimeLabel.Height - 40;

            labelWin = new ImagePanel (Canvas);
            labelWin.Width = 1211;
            labelWin.Height = 170;
            labelWin.X = (Canvas.Width - labelWin.Width) / 2;
            labelWin.Y = 100;
            labelWin.ImageName = "Content/YouEscapedTheMaze.png";
            labelWin.Hide();
            labelWin_de = new ImagePanel (Canvas);
            labelWin_de.Width = 1211;
            labelWin_de.Height = 170;
            labelWin_de.X = (Canvas.Width - labelWin_de.Width) / 2;
            labelWin_de.Y = 100;
            labelWin_de.ImageName = "Content/YouEscapedTheMaze_de.png";
            labelWin_de.Hide();

            labelLoose = new ImagePanel (Canvas);
            labelLoose.Width = 845;
            labelLoose.Height = 168;
            labelLoose.X = (Canvas.Width - labelLoose.Width) / 2;
            labelLoose.Y = 100;
            labelLoose.ImageName = "Content/YouAreDead.png";
            labelLoose.Hide();
            labelLoose_de = new ImagePanel (Canvas);
            labelLoose_de.Width = 845;
            labelLoose_de.Height = 168;
            labelLoose_de.X = (Canvas.Width - labelLoose_de.Width) / 2;
            labelLoose_de.Y = 100;
            labelLoose_de.ImageName = "Content/YouAreDead_de.png";
            labelLoose_de.Hide();
        }

        readonly Button exitButton;
        readonly Label surviveTimeLabel;
        readonly ImagePanel labelWin;
        readonly ImagePanel labelWin_de;
        readonly ImagePanel labelLoose;
        readonly ImagePanel labelLoose_de;

        bool win;

        float minuteCount;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.GameEnded || msg.MessageId == (int)MessageId.GameEndedDied)
            {
                Application.Window.ReleaseMouse ();

                Renderer.SetCanvas (Canvas);

                minuteCount = (float) (DateTime.Now - MazeTest.StartTime).TotalMinutes;

                Canvas.Width = Application.Window.Size.X;
                Canvas.Height = Application.Window.Size.Y;
                exitButton.Text = Localizer.Instance.GetValueForName("quit_game");
                exitButton.X = (Canvas.Width - exitButton.Width) / 2;
                exitButton.Y = Canvas.Height - exitButton.Height - 20;
                surviveTimeLabel.Text = string.Format("{0} {1:0.##} {2}", Localizer.Instance.GetValueForName("you_survived"),
                    minuteCount, Localizer.Instance.GetValueForName("minutes"));
                surviveTimeLabel.X = (Canvas.Width - surviveTimeLabel.Width) / 2;
                surviveTimeLabel.Y = exitButton.Y - surviveTimeLabel.Height - 40;
                labelWin.X = (Canvas.Width - labelWin.Width) / 2;
                labelWin_de.X = (Canvas.Width - labelWin_de.Width) / 2;
                labelLoose.X = (Canvas.Width - labelLoose.Width) / 2;
                labelLoose_de.X = (Canvas.Width - labelLoose_de.Width) / 2;
               
                var input = new FreezingArcher.UI.Input.FreezingArcherInput(Application, Application.MessageManager);
                input.Initialize (Application.RendererContext.Canvas);

                if (msg.MessageId == (int) MessageId.GameEndedDied)
                {
                    if (Localizer.Instance.CurrentLocale == LocaleEnum.de_DE)
                    {
                        labelLoose_de.Show();
                    }
                    else
                    {
                        labelLoose.Show();
                    }
                    win = false;
                }
                else
                {
                    if (Localizer.Instance.CurrentLocale == LocaleEnum.de_DE)
                    {
                        labelWin_de.Show();
                    }
                    else
                    {
                        labelWin.Show();
                    }
                    win = true;
                }
            }

            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                exitButton.X = (Canvas.Width - exitButton.Width) / 2;
                exitButton.Y = Canvas.Height - exitButton.Height - 20;
                surviveTimeLabel.X = (Canvas.Width - surviveTimeLabel.Width) / 2;
                surviveTimeLabel.Y = exitButton.Y - surviveTimeLabel.Height - 40;
                labelWin.X = (Canvas.Width - labelWin.Width) / 2;
                labelWin_de.X = (Canvas.Width - labelWin_de.Width) / 2;
                labelLoose.X = (Canvas.Width - labelLoose.Width) / 2;
                labelLoose_de.X = (Canvas.Width - labelLoose_de.Width) / 2;
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                surviveTimeLabel.Text = string.Format("{0} {1:#.##} {2}", Localizer.Instance.GetValueForName("you_survived"),
                    minuteCount, Localizer.Instance.GetValueForName("minutes"));
                exitButton.Text = Localizer.Instance.GetValueForName("quit_game");

                if (Localizer.Instance.CurrentLocale == LocaleEnum.de_DE)
                {
                    if (win)
                    {
                        labelWin.Hide();
                        labelWin_de.Show();
                    }
                    else
                    {
                        labelLoose.Hide();
                        labelLoose_de.Show();
                    }
                }
                else
                {
                    if (win)
                    {
                        labelWin_de.Hide();
                        labelWin.Show();
                    }
                    else
                    {
                        labelLoose_de.Hide();
                        labelLoose.Show();
                    }
                }
            }
        }

        public int[] ValidMessages{get;}

        #endregion
    }
}

