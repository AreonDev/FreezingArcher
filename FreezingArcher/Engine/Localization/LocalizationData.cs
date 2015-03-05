using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace FreezingArcher.Localization
{
    /// <summary>
    /// Localization data class.
    /// </summary>
    public class LocalizationData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Localization.LocalizationData"/> class.
        /// </summary>
        /// <param name="translations">Translations.</param>
        /// <param name="parent">Parent.</param>
        public LocalizationData (Dictionary<string, string> translations, LocaleEnum parent = LocaleEnum.en_US)
        {
            Parent = parent;
            Values = translations;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Localization.LocalizationData"/> class.
        /// </summary>
        /// <param name="file">Path to localization file.</param>
        public LocalizationData (string file) : this (new FileInfo (file))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Localization.LocalizationData"/> class.
        /// </summary>
        /// <param name="fileinfo">Fileinfo of the localization file.</param>
        public LocalizationData (FileInfo fileinfo)
        {
            Values = new Dictionary<string, string> ();
            XElement e = XElement.Load (fileinfo.FullName);
            XAttribute a = e.Attribute ("parent");
            string parent = a == null ? "None" : a.Value;
            Parent = (LocaleEnum)Enum.Parse (typeof(LocaleEnum), parent);
            foreach (var locale in e.Elements ("string"))
            {
                Values.Add (locale.Attribute ("name").Value, locale.Value);
            }
        }

        /// <summary>
        /// Gets or sets the parent locale.
        /// </summary>
        /// <value>The parent locale.</value>
        public LocaleEnum Parent { get; set; }

        /// <summary>
        /// The values of the localization.
        /// </summary>
        protected Dictionary<string, string> Values;

        /// <summary>
        /// Gets the value of the given element.
        /// </summary>
        /// <returns><c>true</c>, if value was found, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="s">Output string.</param>
        public bool GetElementValue (string name, out string s)
        {
            return Values.TryGetValue (name, out s);
        }
    }
}
