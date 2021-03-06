﻿//
//  ConfigFile.cs
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
using System.Collections.Generic;
using System.IO;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;
using FreezingArcher.Core;

namespace FreezingArcher.Configuration
{
    /// <summary>
    /// Config file.
    /// </summary>
    public class ConfigFile : IManageable, IMessageCreator
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "ConfigFile";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.ConfigFile"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="defaults">Defaults values.</param>
        /// <param name="messageProvider">Message manager.</param>
        public ConfigFile (string name, Dictionary<string, Section> defaults, MessageProvider messageProvider)
        {
            Name = name;
            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Reading {0}.conf", Name);
            messageProvider += this;
            IniConfig = new IniConfig (name + ".conf");
            Defaults = defaults;
            Overrides = new Dictionary<string, Section> ();

            // write default config to file if non existent
            if (!File.Exists (Name + ".conf"))
            {
                foreach (var section in Defaults)
                {
                    foreach (var value in section.Value)
                    {
                        switch (value.Value.Type)
                        {
                        case ValueType.Boolean:
                            IniConfig.SetValue (section.Key, value.Key, value.Value.Boolean);
                            break;
                        case ValueType.Integer:
                            IniConfig.SetValue (section.Key, value.Key, value.Value.Integer);
                            break;
                        case ValueType.Double:
                            IniConfig.SetValue (section.Key, value.Key, value.Value.Double);
                            break;
                        case ValueType.String:
                            IniConfig.SetValue (section.Key, value.Key, value.Value.String);
                            break;
                        case ValueType.Bytes:
                            IniConfig.SetValue (section.Key, value.Key, value.Value.Bytes);
                            break;
                        default:
                            Logger.Log.AddLogEntry (LogLevel.Severe, ClassName,
                                "The type '{0}' of the given value is not supported! This type shouldn't even exist!",
                                value.Value.Type.ToString ());
                            break;
                        }
                    }
                }
                Save ();
            }
        }

        /// <summary>
        /// The ini config.
        /// </summary>
        protected IniConfig IniConfig;

        /// <summary>
        /// The configuration sections.
        /// </summary>
        protected Dictionary<string, Section> Defaults;

        /// <summary>
        /// The configuration overrides.
        /// </summary>
        protected Dictionary<string, Section> Overrides;

        /// <summary>
        /// Save this config to file.
        /// </summary>
        public void Save ()
        {
            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Saving {0}.conf ...", Name);
            IniConfig.Flush ();
            if (MessageCreated != null)
                MessageCreated (new ConfigFileSavedMessage (this));
        }

        /// <summary>
        /// Gets the value from a given section and value name.
        /// </summary>
        /// <returns>The value. If an error occurs null will be returned.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        public Value GetValue (string section, string valueName)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Getting value '{0}:{1}' from {2}.conf ...",
                section, valueName, Name);

            Section s;
            if (!Defaults.TryGetValue (section, out s))
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName,
                    "Section '{0}' is not registered!", section);
                return null;
            }

            Value value;
            if (!s.TryGetValue (valueName, out value))
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName, Status.DataNotAvailable,
                    "There is no default value registered for '{0}'!", valueName);
                return null;
            }

            Section os; // override section
            if (Overrides.TryGetValue (section, out os))
            {
                Value ov; // override value
                if (os.TryGetValue (valueName, out ov))
                {
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                        "Using command line override for '{0}:{1}' from {2}.conf ...", section, valueName, Name);
                    return ov;
                }
            }

            switch (value.Type)
            {
            case ValueType.Boolean:
                return new Value (IniConfig.GetValue (section, valueName, value.Boolean));
            case ValueType.Integer:
                return new Value (IniConfig.GetValue (section, valueName, value.Integer));
            case ValueType.Double:
                return new Value (IniConfig.GetValue (section, valueName, value.Double));
            case ValueType.String:
                return new Value (IniConfig.GetValue (section, valueName, value.String));
            case ValueType.Bytes:
                return new Value (IniConfig.GetValue (section, valueName, value.Bytes));
            default:
                Logger.Log.AddLogEntry (LogLevel.Severe, ClassName, Status.UnexpectedData,
                    "The type '{0}' of the given value is not supported! This type shouldn't even exist!",
                    value.Type.ToString ());
                return null;
            }
        }

        /// <summary>
        /// Gets a boolean value. CAUTION: This method will throw an exception when the requested value is not of type
        /// bool!
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        public bool GetBool (string section, string valueName)
        {
            Value v = GetValue (section, valueName);
            if (v.Type == ValueType.Boolean)
                return v.Boolean;

            Logger.Log.AddLogEntry (LogLevel.Fatal, ClassName, Status.BadData,
                "The requested value '{0}:{1}' is not of type bool!", section, valueName);
            throw new InvalidDataException ("The requested value is of wrong type!");
        }

        /// <summary>
        /// Gets an integer value. CAUTION: This method will throw an exception when the requested value is not of type
        /// int!
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        public int GetInteger (string section, string valueName)
        {
            Value v = GetValue (section, valueName);
            if (v.Type == ValueType.Integer)
                return v.Integer;

            Logger.Log.AddLogEntry (LogLevel.Fatal, ClassName, Status.BadData,
                "The requested value '{0}:{1}' is not of type int!", section, valueName);
            throw new InvalidDataException ("The requested value is of wrong type!");
        }

        /// <summary>
        /// Gets a double value. CAUTION: This method will throw an exception when the requested value is not of type
        /// double!
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        public double GetDouble (string section, string valueName)
        {
            Value v = GetValue (section, valueName);
            if (v.Type == ValueType.Double)
                return v.Double;

            Logger.Log.AddLogEntry (LogLevel.Fatal, ClassName, Status.BadData,
                "The requested value '{0}:{1}' is not of type double!", section, valueName);
            throw new InvalidDataException ("The requested value is of wrong type!");
        }

        /// <summary>
        /// Gets a string value. CAUTION: This method will throw an exception when the requested value is not of type
        /// string!
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        public string GetString (string section, string valueName)
        {
            Value v = GetValue (section, valueName);
            if (v.Type == ValueType.String)
                return v.String;

            Logger.Log.AddLogEntry (LogLevel.Fatal, ClassName, Status.BadData,
                "The requested value '{0}:{1}' is not of type string!", section, valueName);
            throw new InvalidDataException ("The requested value is of wrong type!");
        }

        /// <summary>
        /// Gets a byte array value. CAUTION: This method will throw an exception when the requested value is not of
        /// type byte[]!
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        public byte[] GetBytes (string section, string valueName)
        {
            Value v = GetValue (section, valueName);
            if (v.Type == ValueType.Bytes)
                return v.Bytes;

            Logger.Log.AddLogEntry (LogLevel.Fatal, ClassName, Status.BadData,
                "The requested value '{0}:{1}' is not of type byte[]!", section, valueName);
            throw new InvalidDataException ("The requested value is of wrong type!");
        }

        /// <summary>
        /// Adds the override for the configuration.
        /// </summary>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public void AddOverride (string section, string valueName, Value value)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Adding new override for '{0}:{1}' in {2}.conf",
                section, valueName, Name);
            Section s;
            if (!Overrides.TryGetValue (section, out s))
            {
                s = new Section ();
                Overrides.Add (section, s);
            }
            s.Add (valueName, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <returns><c>true</c>, if value was set, <c>false</c> otherwise.</returns>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public bool SetValue (string section, string valueName, Value value)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Setting value '{0}:{1}' from {2}.conf ...",
                section, valueName, Name);

            Section s;
            if (!Defaults.TryGetValue (section, out s))
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName, Status.BadArgument,
                    "Section '{0}' is not registered!", section);
                return false;
            }

            Value defValue;
            if (!s.TryGetValue (valueName, out defValue))
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName, Status.BadArgument,
                    "'{0}' is not registered!", valueName);
                return false;
            }

            if (value.Type != defValue.Type)
            {
                Logger.Log.AddLogEntry (LogLevel.Error, ClassName, Status.BadArgument,
                    "The given value type '{0}' is not equal with the default value type!", value.Type.ToString ());
                return false;
            }

            switch (value.Type)
            {
            case ValueType.Boolean:
                IniConfig.SetValue (section, valueName, value.Boolean);
                ValueSet (value);
                return true;
            case ValueType.Integer:
                IniConfig.SetValue (section, valueName, value.Integer);
                ValueSet (value);
                return true;
            case ValueType.Double:
                IniConfig.SetValue (section, valueName, value.Double);
                ValueSet (value);
                return true;
            case ValueType.String:
                IniConfig.SetValue (section, valueName, value.String);
                ValueSet (value);
                return true;
            case ValueType.Bytes:
                IniConfig.SetValue (section, valueName, value.Bytes);
                ValueSet (value);
                return true;
            default:
                Logger.Log.AddLogEntry (LogLevel.Severe, ClassName, Status.UnexpectedData,
                    "The type '{0}' of the given value is not supported! This type shouldn't even exist!",
                    value.Type.ToString ());
                return false;
            }
        }

        void ValueSet (Value value)
        {
            if (MessageCreated != null)
                MessageCreated (new ConfigFileValueSetMessage (value));
        }

        /// <summary>
        /// Sets a bool value.
        /// </summary>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public void SetBool (string section, string valueName, bool value)
        {
            SetValue (section, valueName, new Value (value));
        }

        /// <summary>
        /// Sets an integer value.
        /// </summary>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public void SetInteger (string section, string valueName, int value)
        {
            SetValue (section, valueName, new Value (value));
        }

        /// <summary>
        /// Sets a double value.
        /// </summary>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public void SetDouble (string section, string valueName, double value)
        {
            SetValue (section, valueName, new Value (value));
        }

        /// <summary>
        /// Sets a string value.
        /// </summary>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public void SetString (string section, string valueName, string value)
        {
            SetValue (section, valueName, new Value (value));
        }

        /// <summary>
        /// Sets a byte array value.
        /// </summary>
        /// <param name="section">Section.</param>
        /// <param name="valueName">Value name.</param>
        /// <param name="value">Value.</param>
        public void SetBytes (string section, string valueName, byte[] value)
        {
            SetValue (section, valueName, new Value (value));
        }

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        #region IMessageCreator implementation

        /// <summary>
        /// Occurs when a new message is created an is ready for processing
        /// </summary>
        public event MessageEvent MessageCreated;

        #endregion
    }
}
