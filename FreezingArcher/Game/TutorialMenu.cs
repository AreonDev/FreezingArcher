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
using System.Drawing.Drawing2D;

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

            updateTutorialText(Localizer.Instance.GetValueForName("tutorial_text"));

            ValidMessages = new[] { (int) MessageId.WindowResize, (int) MessageId.UpdateLocale };
            application.MessageManager += this;
        }

        readonly Application application;
        readonly Base parent;
        readonly WindowControl window;
        readonly ScrollControl scrollFrame;
        List<List<Label>> textLabels;

        public void Show ()
        {
            window.Show();
        }

        public void Hide ()
        {
            window.Hide();
        }

        void updateTutorialText (string text = null)
        {
            if (text != null && textLabels != null)
            {
                foreach (var line in textLabels)
                {
                    foreach (var word in line)
                    {
                        word.Parent.RemoveChild(word, true);
                    }
                }
            }

            if (textLabels == null || text != null)
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

                textLabels = new List<List<Label>>();
                var lines = text.Split('\n');

                string[] words;
                Label label = null;
                List<Label> labelLine = null;
                foreach (var line in lines)
                {
                    labelLine = new List<Label>();
                    words = line.Split(' ');
                    foreach (var word in words)
                    {
                        if (word.Length == 0)
                            continue;

                        label = new Label(scrollFrame);
                        label.AutoSizeToContents = true;
                        label.Text = word;
                        labelLine.Add(label);
                    }
                    textLabels.Add(labelLine);
                }
            }

            int maxWidth = scrollFrame.Width;

            if (scrollFrame.CanScrollV)
            {
                maxWidth -= 16;
            }

            int y = 0, x = 0;
            const int paragraphHeight = 8;
            foreach (var line in textLabels)
            {
                foreach (var word in line)
                {
                    word.Y = y;
                    word.X = x;
                        
                    x += word.Width;

                    if (x >= maxWidth)
                    {
                        y += word.Height;
                        word.Y = y;
                        word.X = 0;
                        x = word.Width;
                    }
                }

                y += textLabels[0][0].Height + paragraphHeight;
                x = 0;
            }
        }

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
                updateTutorialText();
                scrollFrame.AutoHideBars = true;
                scrollFrame.EnableScroll (false, true);
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                window.Title = Localizer.Instance.GetValueForName("tutorial");
                updateTutorialText(Localizer.Instance.GetValueForName("tutorial_text"));
                scrollFrame.AutoHideBars = true;
                scrollFrame.EnableScroll (false, true);
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
