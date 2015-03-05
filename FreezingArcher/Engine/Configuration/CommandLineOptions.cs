//
//  CommandLineOptions.cs
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
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace FreezingArcher.Configuration
{
    public class CommandLineOptions
    {
        [Option ('o', "option", Required = true, HelpText = "Example option.", DefaultValue = "1234")]
        public string Option { get; set; }

        [ValueList (typeof (List<string>), MaximumElements = 3)]
        public List<string> ValueList { get; set; }

        [OptionList ('l', "option-list", ',', HelpText = "Option list example.", Required = true)]
        public List<string> OptionList { get; set; }

        [OptionArray ('a', "option-array", HelpText = "Option array example.",
            DefaultValue = new string[] {"a","b","c","d"}, Required = true)]
        public string[] OptionArray { get; set; }

        [HelpOption ('h', "help")]
        public string GetUsage ()
        {
            var help = new HelpText {
                Heading = new HeadingInfo ("progname", "version"),
                Copyright = new CopyrightInfo ("author", 2015),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPostOptionsLine ("post option");
            help.AddPreOptionsLine ("pre option");
            help.AddOptions (this);
            return help;
        }
    }
}

