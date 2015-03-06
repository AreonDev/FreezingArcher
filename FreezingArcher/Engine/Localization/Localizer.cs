//
//  Localizer.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2014 Fin Christensen
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
using System.Collections.Generic;
using FreezingArcher.Core;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;

namespace FreezingArcher.Localization
{
    /// <summary>
    /// Localizer.
    /// </summary>
    public class Localizer : IMessageCreator
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Localizer";

        /// <summary>
        /// The global localizer instance.
        /// </summary>
        public static Localizer Instance;

        /// <summary>
        /// Initializes the <see cref="FreezingArcher.Localization.Localizer"/> class.
        /// </summary>
        public static void Initialize (MessageManager messageManager)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Initializing global localizer instance ...");
            Dictionary<LocaleEnum, LocalizationData> dic = new Dictionary<LocaleEnum, LocalizationData> ();
            dic.Add (LocaleEnum.en_US, new LocalizationData ("Localization/en_US.xml"));
            dic.Add (LocaleEnum.de_DE, new LocalizationData ("Localization/de_DE.xml"));
            Instance = new Localizer (dic, messageManager);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Localization.Localizer"/> class.
        /// </summary>
        /// <param name="locales">Locales.</param>
        /// <param name="messageManager">Message manager instance.</param>
        /// <param name="initialLocale">Initial locale.</param>
        public Localizer (Dictionary<LocaleEnum, LocalizationData> locales, MessageManager messageManager,
            LocaleEnum initialLocale = LocaleEnum.en_US)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating logger instance with initial locale '{0}'",
                initialLocale.ToString ());
            Locales = locales;
            CurrentLocale = initialLocale;

            if (messageManager != null)
                messageManager += this;
        }

        /// <summary>
        /// The current locale.
        /// </summary>
        protected LocaleEnum CurLocale;

        /// <summary>
        /// Gets or sets the current locale.
        /// </summary>
        /// <value>The current locale.</value>
        public LocaleEnum CurrentLocale
        {
            get
            {
                return CurLocale;
            }
            set
            {
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Changing locale to '{0}'", value);
                CurLocale = value;
                if (MessageCreated != null)
                    MessageCreated (new GeneralMessage ((int) MessageId.UpdateLocale));
            }
        }

        /// <summary>
        /// Gets or sets the locales.
        /// </summary>
        /// <value>The locales.</value>
        public Dictionary<LocaleEnum, LocalizationData> Locales { get; set; }

        /// <summary>
        /// Gets the value for the given name.
        /// </summary>
        /// <returns>The value for the given name.</returns>
        /// <param name="name">Name.</param>
        public string GetValueForName (string name)
        {
            LocalizationData locale;
            if (!Locales.TryGetValue (CurrentLocale, out locale))
                throw new ArgumentOutOfRangeException (CurrentLocale.ToString (), "No such localization!");
            string s;
            if (!locale.GetElementValue (name, out s))
            {
                if (locale.Parent == LocaleEnum.None || !Locales [locale.Parent].GetElementValue (name, out s))
                    throw new ArgumentOutOfRangeException (name, "Element not found in localization strings!");
            }

            return s;
        }

        #region IMessageCreator implementation

        /// <summary>
        /// Occurs when a new message is created an is ready for processing
        /// </summary>
        public event MessageEvent MessageCreated;

        #endregion
    }
}
