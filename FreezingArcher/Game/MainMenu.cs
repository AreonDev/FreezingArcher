//
//  MainMenu.cs
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Core;
using FreezingArcher.Messaging;
using Gwen.Control;
using FreezingArcher.Localization;
using FreezingArcher.UI.Input;
using FreezingArcher.Renderer.Compositor;

namespace FreezingArcher.Game
{
    public class MainMenu : IMessageConsumer
    {
        const int BUTTON_WIDTH = 200;

        public MainMenu (Application application, Action onPlayGame, CompositorColorCorrectionNode colorCorrectionNode)
        {
            this.application = application;
            this.onPlayGame = onPlayGame;

            ValidMessages = new[] { (int) MessageId.WindowResize, (int) MessageId.UpdateLocale };
            application.MessageManager += this;

            canvas = application.RendererContext.Canvas;
            var input = new FreezingArcherInput(application, application.MessageManager);
            input.Initialize (canvas);

            canvas.SetSize(application.Window.Size.X, application.Window.Size.Y);
            canvas.ShouldDrawBackground = false;

            settings = new SettingsMenu (application, canvas, colorCorrectionNode);
            tutorial = new TutorialMenu (application, canvas);

            background = new ImagePanel (canvas);
            background.ImageName = "Content/MainMenu.jpg";
            updateBackground();

            exitButton = new Button (canvas);
            exitButton.Text = Localizer.Instance.GetValueForName("quit_game");
            exitButton.Width = BUTTON_WIDTH;
            exitButton.X = 40;
            exitButton.Y = canvas.Height - exitButton.Height - 60;
            exitButton.Clicked += (sender, arguments) => application.Window.Close ();

            settingsButton = new Button (canvas);
            settingsButton.Text = Localizer.Instance.GetValueForName("settings");
            settingsButton.Width = BUTTON_WIDTH;
            settingsButton.X = 40;
            settingsButton.Y = exitButton.Y - settingsButton.Height - 40;
            settingsButton.Clicked += (sender, arguments) => settings.Show();

            tutorialButton = new Button (canvas);
            tutorialButton.Text = Localizer.Instance.GetValueForName("tutorial");
            tutorialButton.Width = BUTTON_WIDTH;
            tutorialButton.X = 40;
            tutorialButton.Y = settingsButton.Y - tutorialButton.Height - 40;
            tutorialButton.Clicked += (sender, arguments) => tutorial.Show();

            startButton = new Button (canvas);
            startButton.Text = Localizer.Instance.GetValueForName("start_game");
            startButton.Width = BUTTON_WIDTH;
            startButton.X = 40;
            startButton.Y = tutorialButton.Y - startButton.Height - 40;
            startButton.Clicked += (sender, arguments) => {
                application.MessageManager.UnregisterMessageConsumer(this);
                application.MessageManager.UnregisterMessageConsumer(settings);
                settings.Destroy();
                onPlayGame();
            };
        }

        readonly Application application;
        readonly Action onPlayGame;
        readonly Canvas canvas;

        readonly ImagePanel background;
        readonly Button startButton;
        readonly Button settingsButton;
        readonly Button exitButton;
        readonly Button tutorialButton;
        SettingsMenu settings;
        TutorialMenu tutorial;

        void updateBackground ()
        {
            int width, height;
            if (canvas.Height * 16 / 9 > canvas.Width)
            {
                width = canvas.Height * 16 / 9;
                height = canvas.Height;
            }
            else
            {
                width = canvas.Width;
                height = canvas.Width * 9 / 16;
            }
            background.Width = width;
            background.Height = height;
            background.X = (canvas.Width - width) / 2;
            background.Y = (canvas.Height - height) / 2;
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                updateBackground();
                exitButton.Y = canvas.Height - exitButton.Height - 40;
                settingsButton.Y = exitButton.Y - settingsButton.Height - 40;
                tutorialButton.Y = settingsButton.Y - tutorialButton.Height - 40;
                startButton.Y = tutorialButton.Y - startButton.Height - 40;
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                exitButton.Text = Localizer.Instance.GetValueForName("quit_game");
                tutorialButton.Text = Localizer.Instance.GetValueForName("tutorial");
                settingsButton.Text = Localizer.Instance.GetValueForName("settings");
                startButton.Text = Localizer.Instance.GetValueForName("start_game");
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}

