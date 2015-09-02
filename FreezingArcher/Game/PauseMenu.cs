//
//  PauseMenu.cs
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
using Gwen.Control;
using FreezingArcher.Localization;
using FreezingArcher.Core;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;

namespace FreezingArcher.Game
{
    public sealed class PauseMenu : IMessageConsumer
    {
        const int BUTTON_WIDTH = 300;

        public PauseMenu (Application app, Base parent, Action onContinuePressed)
        {
            this.onContinuePressed = onContinuePressed;
            this.application = app;
            this.parent = parent;

            window = new WindowControl (parent, Localizer.Instance.GetValueForName("pause"));
            window.DisableResizing();
            window.IsMoveable = false;
            window.OnClose += (sender, arguments) => Hide();
            window.Width = BUTTON_WIDTH + 20;
            window.Hide();

            continueButton = new Button (window);
            continueButton.Text = Localizer.Instance.GetValueForName("back_to_game");
            continueButton.Width = BUTTON_WIDTH;
            continueButton.X = 10;
            continueButton.Y = 10;
            continueButton.Clicked += (sender, arguments) => Hide();

            exitButton = new Button (window);
            exitButton.Text = Localizer.Instance.GetValueForName("quit_game");
            exitButton.Width = BUTTON_WIDTH;
            exitButton.X = 10;
            exitButton.Y = 10 + continueButton.Y + continueButton.Height;
            exitButton.Clicked += (sender, arguments) => application.Window.Close ();

            window.Height = exitButton.Y + exitButton.Height + 10;
            window.X = (parent.Width - window.Width) / 2;
            window.Y = (parent.Height - window.Height) / 2;

            ValidMessages = new int[] { (int) MessageId.WindowResize, (int) MessageId.UpdateLocale, (int) MessageId.Input };
        }

        readonly Action onContinuePressed;
        readonly Application application;

        readonly Base parent;
        readonly WindowControl window;
        readonly Button continueButton;
        readonly Button exitButton;

        public void Show ()
        {
            window.Show();
        }

        public void Hide ()
        {
            onContinuePressed();
            window.Hide();
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                window.X = (parent.Width - window.Width) / 2;
                window.Y = (parent.Height - window.Height) / 2;
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                window.Title = Localizer.Instance.GetValueForName("pause");
                continueButton.Text = Localizer.Instance.GetValueForName("back_to_game");
                exitButton.Text = Localizer.Instance.GetValueForName("quit_game");
            }

            if (msg.MessageId == (int) MessageId.Input)
            {
                
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}

