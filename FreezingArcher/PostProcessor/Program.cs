//
//  Program.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2015 Martin Koppehel
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
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PostProcessor
{
    class MainClass
    {
        public static int Main (string[] args)
        {

            if (args.Length < 2)
                return 1;

            List<IPostProcessingStep> postProcessingSteps = new List<IPostProcessingStep> ();
            var file = new FileInfo (args [1]);

            var types = AppDomain.CurrentDomain.GetAssemblies ().Select (j => j.GetTypes ());
            foreach (var tArr in types) {
                var pps = tArr.Where (j => !j.IsInterface && !j.IsAbstract && typeof(IPostProcessingStep).IsAssignableFrom (j)).Select(Activator.CreateInstance).Cast<IPostProcessingStep>();
                postProcessingSteps.AddRange (pps);
            }

            if (file.Extension != ".dll" && file.Extension != ".exe")
                return -1;

            Console.WriteLine ("Processing file: {0}", file.Name);

            try
            {
                var asm = Assembly.LoadFrom (file.FullName);
                if(asm == null) Console.WriteLine("File {0} is no Assembly.", file.Name);

                foreach (var item in postProcessingSteps) {
                    var result = item.DoPostProcessing(asm);
                    if(result != 0)
                    {
                        Console.WriteLine("Post Processor failed.");
                        return result;
                    }
                }
            }
            catch(Exception)
            {
                Console.WriteLine("File {0} is no Assembly", file.Name);
            }
            Console.WriteLine ("Success!");
            return 0;
        }
    }
}
