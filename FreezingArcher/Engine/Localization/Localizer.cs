using System;
using System.Collections.Generic;

namespace FreezingArcher.Localization
{
    /// <summary>
    /// Localizer.
    /// </summary>
    public class Localizer
    {
        /// <summary>
        /// The global localizer instance.
        /// </summary>
        public static Localizer Instance;

        /// <summary>
        /// Initializes the <see cref="FreezingArcher.Localization.Localizer"/> class.
        /// </summary>
        static Localizer ()
        {
            Dictionary<LocaleEnum, LocalizationData> dic = new Dictionary<LocaleEnum, LocalizationData> ();
            dic.Add (LocaleEnum.en_US, new LocalizationData ("Localization/en_US.xml"));
            dic.Add (LocaleEnum.de_DE, new LocalizationData ("Localization/de_DE.xml"));
            Instance = new Localizer (dic);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Localization.Localizer"/> class.
        /// </summary>
        /// <param name="locales">Locales.</param>
        /// <param name="initialLocale">Initial locale.</param>
        public Localizer (Dictionary<LocaleEnum, LocalizationData> locales, LocaleEnum initialLocale = LocaleEnum.en_US)
        {
            Locales = locales;
            CurrentLocale = initialLocale;
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
                CurLocale = value;
                if (UpdateLocale != null)
                    UpdateLocale ();
            }
        }

        /// <summary>
        /// Gets or sets the locales.
        /// </summary>
        /// <value>The locales.</value>
        public Dictionary<LocaleEnum, LocalizationData> Locales { get; set; }

        /// <summary>
        /// Occurs when locale is updated.
        /// </summary>
        public event Action UpdateLocale;

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
    }
}
