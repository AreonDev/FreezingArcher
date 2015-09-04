//
//  SettingsMenu.cs
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
using System.Linq;
using FreezingArcher.Core;
using Gwen.Control;
using FreezingArcher.Localization;
using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Configuration;

namespace FreezingArcher.Game
{
    public sealed class SettingsMenu : IMessageConsumer
    {
        const int CONTROL_WIDTH = 200;
        public SettingsMenu (Application app, Base parent, CompositorColorCorrectionNode colorCorrectionNode)
        {
            this.application = app;
            this.parent = parent;
            this.colorCorrectionNode = colorCorrectionNode;
            window = new WindowControl (parent);
            window.DisableResizing();
            window.Title = Localizer.Instance.GetValueForName("settings");
            window.Hide();

            float gamma = (float) ConfigManager.Instance["freezing_archer"].GetDouble("general", "gamma");
            colorCorrectionNode.Gamma = gamma;
            string language = ConfigManager.Instance["freezing_archer"].GetString("general", "language");
            Localizer.Instance.CurrentLocale = (LocaleEnum) Enum.Parse(typeof (LocaleEnum), language);

            languageLabel = new Label (window);
            languageLabel.AutoSizeToContents = true;
            languageLabel.Y = 10;
            languageLabel.Text = Localizer.Instance.GetValueForName("language");

            languageDropdown = new ComboBox (window);
            languageDropdown.Width = CONTROL_WIDTH;
            languageDropdown.Y = 10;
            foreach (var lang in Localizer.Instance.Locales)
            {
                var item = languageDropdown.AddItem(Localizer.Instance.GetValueForName(lang.Key.ToString()), lang.Key.ToString());
                item.AutoSizeToContents = false;
                item.Width = CONTROL_WIDTH;
            }
            languageDropdown.SelectByText(Localizer.Instance.GetValueForName(Localizer.Instance.CurrentLocale.ToString()));
            languageDropdown.ItemSelected += handleSelect;

            gammaSlider = new HorizontalSlider (window);
            gammaSlider.SnapToNotches = false;
            gammaSlider.Min = 0;
            gammaSlider.Max = 2;
            gammaSlider.Value = gamma;
            gammaSlider.ValueChanged += (sender, arguments) => {
                var slider = sender as HorizontalSlider;
                ConfigManager.Instance["freezing_archer"].SetDouble("general", "gamma", slider.Value);
                if (slider != null)
                    this.colorCorrectionNode.Gamma = slider.Value;
            };
            gammaSlider.Width = CONTROL_WIDTH;
            gammaSlider.Height = 20;
            gammaSlider.Y = 10 + languageDropdown.Y + languageDropdown.Height;

            gammaLabel = new Label (window);
            gammaLabel.AutoSizeToContents = true;
            gammaLabel.Y = 10 + languageLabel.Y + languageLabel.Height;
            gammaLabel.Text = Localizer.Instance.GetValueForName("gamma");

            int max_width = languageLabel.Width > gammaLabel.Width ? languageLabel.Width : gammaLabel.Width;

            languageLabel.X = 10 + max_width - languageLabel.Width;
            gammaLabel.X = 10 + max_width - gammaLabel.Width;
            languageDropdown.X = 10 + max_width + 5;
            gammaSlider.X = 10 + max_width + 5;

            window.Width = 40 + max_width + CONTROL_WIDTH;

            resetButton = new Button (window);
            resetButton.Text = Localizer.Instance.GetValueForName("reset");
            resetButton.Width = (window.Width / 2) - 20;
            resetButton.X = 10;
            resetButton.Y = gammaSlider.Y + gammaSlider.Height + 10;
            resetButton.Clicked += (sender, arguments) => {
                var general = ConfigManager.DefaultConfig.B.FirstOrDefault (a => a.Key == "general");
                var gamma_pair = general.Value.FirstOrDefault(a => a.Key == "gamma");
                
                float _gamma = (float) gamma_pair.Value.Double;
                gammaSlider.Value = _gamma;
                colorCorrectionNode.Gamma = _gamma;

                var language_pair = general.Value.FirstOrDefault(a => a.Key == "language");

                string lang = language_pair.Value.String;
                Localizer.Instance.CurrentLocale = (LocaleEnum) Enum.Parse(typeof (LocaleEnum), lang);
            };

            saveButton = new Button (window);
            saveButton.Text = Localizer.Instance.GetValueForName("save");
            saveButton.Width = (window.Width / 2) - 20;
            saveButton.X = resetButton.X + resetButton.Width + 10;
            saveButton.Y = gammaSlider.Y + gammaSlider.Height + 10;
            saveButton.Clicked += (sender, arguments) => ConfigManager.Instance.SaveAll();

            window.Height += languageDropdown.Height + gammaSlider.Height + 50;
            window.X = (parent.Width - window.Width) / 2;
            window.Y = (parent.Height - window.Height) / 2;

            ValidMessages = new[] { (int) MessageId.UpdateLocale };
            application.MessageManager += this;
        }

        readonly Application application;
        readonly Base parent;
        readonly CompositorColorCorrectionNode colorCorrectionNode;
        readonly WindowControl window;
        readonly ComboBox languageDropdown;
        readonly HorizontalSlider gammaSlider;
        readonly Label gammaLabel;
        readonly Label languageLabel;
        readonly Button resetButton;
        readonly Button saveButton;

        public void Show ()
        {
            window.Show();
        }

        public void Hide ()
        {
            window.Hide();
        }

        public void Destroy ()
        {
            application.MessageManager.UnregisterMessageConsumer(this);
        }

        void handleSelect (Base sender, ItemSelectedEventArgs args)
        {
            var item = sender as ComboBox;
            Localizer.Instance.CurrentLocale = (LocaleEnum) Enum.Parse(typeof (LocaleEnum), item.SelectedItem.Name);
            ConfigManager.Instance["freezing_archer"].SetString("general", "language", item.SelectedItem.Name);
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                window.Title = Localizer.Instance.GetValueForName("settings");
                languageLabel.Text = Localizer.Instance.GetValueForName("language");
                languageDropdown.DeleteAll();
                languageDropdown.ItemSelected -= handleSelect;
                foreach (var lang in Localizer.Instance.Locales)
                {
                    var item = languageDropdown.AddItem(Localizer.Instance.GetValueForName(lang.Key.ToString()), lang.Key.ToString());
                    item.AutoSizeToContents = false;
                    item.Width = CONTROL_WIDTH;
                }
                languageDropdown.SelectByText(Localizer.Instance.GetValueForName(Localizer.Instance.CurrentLocale.ToString()));
                languageDropdown.ItemSelected += handleSelect;
                gammaLabel.Text = Localizer.Instance.GetValueForName("gamma");
                int max_width = languageLabel.Width > gammaLabel.Width ? languageLabel.Width : gammaLabel.Width;
                languageLabel.X = 10 + max_width - languageLabel.Width;
                gammaLabel.X = 10 + max_width - gammaLabel.Width;
                languageDropdown.X = 10 + max_width + 5;
                gammaSlider.X = 10 + max_width + 5;
                window.Width = 40 + max_width + CONTROL_WIDTH;
                resetButton.Text = Localizer.Instance.GetValueForName("reset");
                resetButton.Width = (window.Width / 2) - 20;
                saveButton.Text = Localizer.Instance.GetValueForName("save");
                saveButton.Width = (window.Width / 2) - 20;
                saveButton.X = resetButton.X + resetButton.Width + 10;
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
