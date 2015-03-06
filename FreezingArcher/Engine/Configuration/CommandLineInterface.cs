//
//  CommandLineInterface.cs
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
using System.Collections;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using FreezingArcher.Core;
using FreezingArcher.Reflection;
using Attribute = FreezingArcher.Reflection.Attribute;

namespace FreezingArcher.Configuration
{
    /// <summary>
    /// Command line interface class.
    /// </summary>
    public class CommandLineInterface
    {
        /// <summary>
        /// The global instance.
        /// </summary>
        public static CommandLineInterface Instance;

        static CommandLineInterface ()
        {
            Instance = new CommandLineInterface ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Configuration.CommandLineInterface"/> class.
        /// </summary>
        public CommandLineInterface ()
        {
            DynamicClassBuilder = new DynamicClassBuilder ("Options");
            Handlers = new Dictionary<string, Pair<Action<object>, Type>> ();
        }

        /// <summary>
        /// The dynamic class builder.
        /// </summary>
        protected DynamicClassBuilder DynamicClassBuilder;

        /// <summary>
        /// The handlers.
        /// </summary>
        protected Dictionary<string, Pair<Action<object>, Type>> Handlers;

        // <summary>
        // Parses the arguments from the command line.
        // </summary>
        /// <returns><c>true</c>, if arguments were parsed, <c>false</c> otherwise.</returns>
        /// <param name="args">The arguments.</param>
        public bool ParseArguments (string[] args)
        {
            Type t = DynamicClassBuilder.CreateType ();

            object options = Activator.CreateInstance (t);
            if (Parser.Default.ParseArguments (args, options))
            {
                foreach (var handler in Handlers)
                {
                    var read = typeof (Property).GetMethod ("Read");
                    var readT = read.MakeGenericMethod (handler.Value.B);
                    handler.Value.A.Invoke (readT.Invoke (null, new object[] {options, handler.Key}));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an option to the command line interface. (-e, --example VALUE)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed value from the command line.</param>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="helpText">The help text displayed in the help message.</param>
        /// <param name="metaValue">The meta value string displayed in the help message.</param>
        /// <param name="required">If set to <c>true</c> this option is required.</param>
        /// <param name="defaultValue">The default value of this option.</param>
        /// <typeparam name="T">This type specifies of which type the parsed value will be.</typeparam>
        public void AddOption<T> (Action<T> handler, char shortName, string longName = "", string helpText = "",
            string metaValue = "", bool required = false, T defaultValue = default (T))
        {
            Attribute attr = new Attribute (typeof (OptionAttribute));
            attr.CallConstructor (shortName, longName);
            attr.AddNamedParameters (new Pair<string, object> ("HelpText", helpText),
                new Pair<string, object> ("Required", required),
                new Pair<string, object> ("DefaultValue", defaultValue),
                new Pair<string, object> ("MetaValue", metaValue));
            DynamicClassBuilder.AddProperty (new Property (longName, typeof (T), attr));
            Handlers.Add (longName, new Pair<Action<object>, Type> (j => handler ((T) j), typeof (T)));
        }

        /// <summary>
        /// The value list property.
        /// </summary>
        protected Property ValueListProperty;

        /// <summary>
        /// Sets a value list to the command line interface. (program val1 val2 val3 ...)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed values from the command line.</param>
        /// <param name="metaValue">The meta value string displayed in the help message.</param>
        /// <param name="maximumElements">Maximum number of elements in the value list.
        /// A number of -1 allows an unlimited number of values.</param>
        /// <typeparam name="T">This type specifies of which type the parsed values will be.</typeparam>
        public void SetValueList<T> (Action<T> handler, string metaValue = "", int maximumElements = -1) where T : IList
        {
            Attribute attr = new Attribute (typeof (ValueListAttribute));
            attr.CallConstructor (typeof (List<T>));
            attr.AddNamedParameters (new Pair<string, object> ("MaximumElements", maximumElements),
                new Pair<string, object> ("MetaValue", metaValue));
            if (ValueListProperty != null)
                DynamicClassBuilder.RemoveProperty (ValueListProperty);
            ValueListProperty = new Property ("ValueList", typeof (T), attr);
            Handlers.Add ("ValueList", new Pair<Action<object>, Type> (j => handler ((T) j), typeof (T)));
        }

        /// <summary>
        /// Adds an option list to the command line interface. (-e, --example val1,val2,val3...)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed values from the command line.</param>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="separator">The separator of the given values.</param>
        /// <param name="helpText">The help text displayed in the help message.</param>
        /// <param name="metaValue">The meta value string displayed in the help message.</param>
        /// <param name="required">If set to <c>true</c> this option is required.</param>
        /// <typeparam name="T">This type specifies of which type the parsed values will be.</typeparam>
        public void AddOptionList<T> (Action<T> handler, char shortName, string longName = "", char separator = ',',
            string helpText = "", string metaValue = "", bool required = false) where T : IList
        {
            Attribute attr = new Attribute (typeof (OptionListAttribute));
            attr.CallConstructor (shortName, longName, separator);
            attr.AddNamedParameters (new Pair<string, object> ("HelpText", helpText),
                new Pair<string, object> ("Required", required),
                new Pair<string, object> ("MetaValue", metaValue));
            DynamicClassBuilder.AddProperty (new Property (longName, typeof (T), attr));
            Handlers.Add (longName, new Pair<Action<object>, Type> (j => handler ((T) j), typeof (T)));
        }

        /// <summary>
        /// Adds an option array to the command line interface. (-e, --example val1 val2 val3 ...)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed values from the command line.</param>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="helpText">The help text displayed in the help message.</param>
        /// <param name="metaValue">The meta value string displayed in the help message.</param>
        /// <param name="required">If set to <c>true</c> this option is required.</param>
        /// <param name="defaultValue">The default values of this option.</param>
        /// <typeparam name="T">This type specifies of which type the parsed values will be.</typeparam>
        public void AddOptionArray<T> (Action<T[]> handler, char shortName, string longName = "",
            string helpText = "", string metaValue = "", bool required = false, T[] defaultValue = default (T[]))
        {
            Attribute attr = new Attribute (typeof (OptionArrayAttribute));
            attr.CallConstructor (shortName, longName);
            attr.AddNamedParameters (new Pair<string, object> ("HelpText", helpText),
                new Pair<string, object> ("Required", required),
                new Pair<string, object> ("DefaultValue", defaultValue),
                new Pair<string, object> ("MetaValue", metaValue));
            DynamicClassBuilder.AddProperty (new Property (longName, typeof (T[]), attr));
            Handlers.Add (longName, new Pair<Action<object>, Type> (j => handler ((T[]) j), typeof (T[])));
        }

        /// <summary>
        /// Configure help message.
        /// </summary>
        /// <param name="programName">Program name.</param>
        /// <param name="version">Version.</param>
        /// <param name="author">Author.</param>
        /// <param name="year">Year.</param>
        /// <param name="shortName">Short name of the help option.</param>
        /// <param name="longName">Long name of the help option.</param>
        /// <param name="additionalNewLineAfterOption">If set to <c>true</c> insert additional new line after options
        /// section.</param>
        /// <param name="addDashesToOptions">If set to <c>true</c> add dashes to options.</param>
        /// <param name="preOptionsLines">Lines to appear before options section.</param>
        /// <param name="postOptionsLines">Lines to appear after options section.</param>
        public void SetHelp (string programName, string version, string author, int year, char shortName = 'h',
            string longName = "help", bool additionalNewLineAfterOption = true, bool addDashesToOptions = true,
            IEnumerable<string> preOptionsLines = null, IEnumerable<string> postOptionsLines = null)
        {
            Attribute attr = new Attribute (typeof (HelpOptionAttribute));
            attr.CallConstructor (shortName, longName);
            var function = new Func<Object, string> ((Object instance) =>
            {
                var help = new HelpText {
                    Heading = new HeadingInfo (programName, version),
                    Copyright = new CopyrightInfo (author, year),
                    AdditionalNewLineAfterOption = additionalNewLineAfterOption,
                    AddDashesToOption = addDashesToOptions
                };                

                if (preOptionsLines != null)
                    foreach (string s in preOptionsLines)
                        help.AddPreOptionsLine (s);

                if (postOptionsLines != null)
                    foreach (string s in postOptionsLines)
                        help.AddPostOptionsLine (s);

                help.AddOptions (instance);
                return help;
            });
            Method m = new Method ("GetUsage", function, attr);

            DynamicClassBuilder.AddMethod (m);
        }
    }
}
