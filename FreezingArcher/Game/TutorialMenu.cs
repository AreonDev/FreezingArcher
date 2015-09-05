//
//  TutorialMenu.cs
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
using FreezingArcher.Core;
using Gwen.Control;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Localization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FreezingArcher.Game
{
    public sealed class TutorialMenu : IMessageConsumer
    {
        public TutorialMenu (Application application, Base parent)
        {
            this.application = application;
            this.parent = parent;

            window = new WindowControl (parent);
            window.DisableResizing();
            window.Title = Localizer.Instance.GetValueForName("tutorial");
            window.IsMoveable = false;
            window.Hide();
            window.Height = parent.Height - (int) (parent.Height * 0.35);
            window.Y = (int) (parent.Height * 0.35) - 20;
            window.Width = parent.Width - 320;
            window.X = 280;

            scrollFrame = new ScrollControl (window);
            scrollFrame.AutoHideBars = true;
            scrollFrame.EnableScroll (false, true);
            scrollFrame.Width = window.Width - 12;
            scrollFrame.Height = window.Height - 32;

            setTutorialText(Localizer.Instance.GetValueForName("tutorial_text"));

            ValidMessages = new[] { (int) MessageId.WindowResize, (int) MessageId.UpdateLocale, (int) MessageId.WindowFocus };
            application.MessageManager += this;
        }

        readonly Application application;
        readonly Base parent;
        readonly WindowControl window;
        readonly ScrollControl scrollFrame;
        List<Label> textLabels;

        public void Show ()
        {
            window.Show();
        }

        public void Hide ()
        {
            window.Hide();
        }

        void setTutorialText (string text)
        {
            RegexOptions options = RegexOptions.Multiline;
            var regex = new Regex(@"(.+)(\n*)\n", options);     
            var m = regex.Matches(text);
            string new_text = "";
            foreach (Match match in m)
            {
                new_text += match.Groups[1] + " " + match.Groups[2];
            }

            regex = new Regex (@"\n(.)$", options);
            new_text += regex.Match(text).Groups[1];
            text = new_text;

            if (textLabels != null)
            {
                foreach (var l in textLabels)
                {
                    l.DelayedDelete();
                }
            }

            textLabels = new List<Label>();
            var lines = text.Split('\n');

            int maxWidth = scrollFrame.Width;

            if (scrollFrame.CanScrollV)
            {
                maxWidth -= 16;
            }

            int y = 0, x = 0;
            const int paragraphHeight = 8;
            string[] words;
            Label label = null;
            foreach (var line in lines)
            {
                words = line.Split(' ');
                foreach (var word in words)
                {
                    if (word.Length == 0)
                        continue;

                    label = new Label(scrollFrame);
                    label.AutoSizeToContents = true;
                    label.Y = y;
                    label.X = x;

                    label.Text = word;
                        
                    x += label.Width;

                    if (x >= maxWidth)
                    {
                        y += label.Height;
                        label.Y = y;
                        label.X = 0;
                        x = label.Width;
                    }

                    textLabels.Add(label);
                }

                y += textLabels[0].Height + paragraphHeight;
                x = 0;
            }
        }

        bool recalcText = false;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                window.Height = parent.Height - (int) (parent.Height * 0.35);
                window.Width = parent.Width - 320;
                window.Y = (int) (parent.Height * 0.35) - 20;
                window.X = 280;
                scrollFrame.Width = window.Width - 12;
                scrollFrame.Height = window.Height - 32;

                recalcText = true;
            }

            if (msg.MessageId == (int) MessageId.WindowFocus && recalcText)
            {
                setTutorialText(Localizer.Instance.GetValueForName("tutorial_text"));
                scrollFrame.AutoHideBars = true;
                scrollFrame.EnableScroll (false, true);
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                window.Title = Localizer.Instance.GetValueForName("tutorial");
                setTutorialText(Localizer.Instance.GetValueForName("tutorial_text"));
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
