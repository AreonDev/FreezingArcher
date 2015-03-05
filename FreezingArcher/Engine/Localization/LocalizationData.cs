//
//  LocalizationData.cs
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
using System.IO;
using System.Xml.Linq;
using FreezingArcher.Output;

namespace FreezingArcher.Localization
{
    /// <summary>
    /// Localization data class.
    /// </summary>
    public class LocalizationData
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "LocalizationData";

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
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading new localization from '{0}' ...",
                fileinfo.FullName);
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
