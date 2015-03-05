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
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using System.Collections;

namespace FreezingArcher.Configuration
{
    public class CommandLineInterface
    {
        public CommandLineInterface ()
        {
        }

        object options;

        /// <summary>
        /// Parses the arguments from the command line.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void ParseArguments (string[] args)
        {
            if (Parser.Default.ParseArguments (args, options))
            {
                // read values from options and call option handlers with parsed data.
            }
        }

        /// <summary>
        /// Adds an option to the command line interface. (-e, --example VALUE)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed value from the command line.</param>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="helpText">The help text displayed in the help message.</param>
        /// <param name="required">If set to <c>true</c> this option is required.</param>
        /// <param name="defaultValue">The default value of this option.</param>
        /// <typeparam name="T">This type specifies of which type the parsed value will be.</typeparam>
        public void AddOption<T> (Action<T> handler, char shortName, string longName = null, string helpText = null,
            bool required = false, T defaultValue = default (T))
        {}

        /// <summary>
        /// Sets a value list to the command line interface. (program val1 val2 val3 ...)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed values from the command line.</param>
        /// <param name="maximumElements">Maximum number of elements in the value list.
        /// A number of -1 allows an unlimited number of values.</param>
        /// <typeparam name="T">This type specifies of which type the parsed values will be.</typeparam>
        public void SetValueList<T> (Action<T> handler, int maximumElements = -1) where T : IList
        {}

        /// <summary>
        /// Adds an option list to the command line interface. (-e, --example val1,val2,val3...)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed values from the command line.</param>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="separator">The separator of the given values.</param>
        /// <param name="helpText">The help text displayed in the help message.</param>
        /// <param name="required">If set to <c>true</c> this option is required.</param>
        /// <typeparam name="T">This type specifies of which type the parsed values will be.</typeparam>
        public void AddOptionList<T> (Action<T> handler, char shortName, string longName = null, char separator = ',',
            string helpText = null, bool required = false) where T : IList
        {
            AddOptionArray ((int[] a) => {return;}, 'a');
        }

        /// <summary>
        /// Adds an option array to the command line interface. (-e, --example val1 val2 val3 ...)
        /// </summary>
        /// <param name="handler">The handler which handles the parsed values from the command line.</param>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="helpText">The help text displayed in the help message.</param>
        /// <param name="required">If set to <c>true</c> this option is required.</param>
        /// <param name="defaultValue">The default values of this option.</param>
        /// <typeparam name="T">This type specifies of which type the parsed values will be.</typeparam>
        public void AddOptionArray<T> (Action<T[]> handler, char shortName, string longName = null,
            string helpText = null, bool required = false, T[] defaultValue = default (T[]))
        {}

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
        {}
    }
}
