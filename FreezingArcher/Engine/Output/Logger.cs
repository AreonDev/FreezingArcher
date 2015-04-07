//
//  Logger.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FreezingArcher.Core;

namespace FreezingArcher.Output
{
    /// <summary>
    /// Event arguments for adding a log entry
    /// </summary>
    public class AddLogEntryEventArgs : EventArgs
    {
        /// <summary>
        /// The LogLevel
        /// </summary>
        public LogLevel Level;
        /// <summary>
        /// The timestamp
        /// </summary>
        public DateTime Timestamp;
        /// <summary>
        /// The module name
        /// </summary>
        public string ModuleName;
        /// <summary>
        /// The message to format
        /// </summary>
        public string Format;
        /// <summary>
        /// The arguments
        /// </summary>
        public object[] Args;

        /// <summary>
        /// Converts this instance to a <see cref="LogLine"/>
        /// </summary>
        /// <returns><see cref="LogLine"/> object representing this instance</returns>
        public LogLine ToLogLine ()
        {
            return new LogLine { LogLevel = Level, Timestamp = Timestamp, ModuleName = ModuleName,
                Format = Format, Param = Args };
        }
    }

    /// <summary>
    /// Represents a single entry in the log file
    /// </summary>
    public class LogLine
    {
        /// <summary>The log level</summary>
        internal LogLevel LogLevel;
        /// <summary>The timestamp</summary>
        internal DateTime Timestamp;
        /// <summary>The module name</summary>
        internal string ModuleName;
        /// <summary>The message</summary>
        internal string Format;
        /// <summary>The parameters</summary>
        internal object[] Param = new object[0];

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString (int maxLength)
        {
            string pre = "[" + LogLevel.ToString().PadRight(7) +"] "+Timestamp + " [" + ModuleName.PadRight(maxLength) + "]: ";
            var _fmt = Format.Replace ("\n", "\n" + pre + "--> ");
            return string.Concat (pre, string.Format (_fmt, Param));
        }

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToConsoleString(int maxLength)
        {
            string pre = "[" + ModuleName.PadRight(maxLength) + "]: ";
            var _fmt = Format.Replace ("\n", "\n" + pre + "--> ");
            return string.Concat (pre, string.Format (_fmt, Param));
        }

    }

    /// <summary>
    /// Wrapper around a log file
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Global logger instance.
        /// </summary>
        /// <value>The log.</value>
        public static Logger Log { get; private set; }

        /// <summary>
        /// Initialize a global logger with the specified logfile.
        /// </summary>
        /// <param name="logfile">Logfile.</param>
        public static void Initialize(string logfile)
        {
            if (Log != null) return;

            #if DEBUG
            Log = new Logger(new FileInfo(logfile + ".log"), LogLevel.Debug);
            #else
            Log = new Logger(new FileInfo(logfile + ".log"), LogLevel.Info);
            #endif
        }
        /// <summary>The module name</summary>
        private const string moduleName = "Log-System";
        /// <summary>The colors</summary>
        private Dictionary<LogLevel, ConsoleColor> colors;
        /// <summary>The lines to process</summary>
        private Queue<LogLine> linesToProcess;
        /// <summary>The file writer</summary>
        private StreamWriter fileWriter;
        /// <summary>true if the thread processing the messages should exit</summary>
        private bool exit = false;
        /// <summary>The t</summary>
        private Thread t;
        /// <summary>The minimum l</summary>
        private LogLevel minL;
        /// <summary>The maximum known moduleName length</summary>
        private int maxModNameLength;


        /// <summary>Initializes a new instance of the <see cref="Logger"/> class.</summary>
        /// <param name="logFile">The log file.</param>
        /// <param name="minLvl">The minimum level to log.</param>
        /// <param name="append">if set to <c>true</c> appends to the file if exists.</param>
        public Logger (FileInfo logFile, LogLevel minLvl = LogLevel.Fine, bool append = false)
        {
            maxModNameLength = 16;
            minL = minLvl;
            t = new Thread (run);
            linesToProcess = new Queue<LogLine> ();
            colors = new Dictionary<LogLevel, ConsoleColor> ();
            colors.Add (LogLevel.Debug, ConsoleColor.DarkCyan);
            colors.Add (LogLevel.Fine, ConsoleColor.DarkGreen);
            colors.Add (LogLevel.Info, ConsoleColor.DarkBlue);
            colors.Add (LogLevel.Warning, ConsoleColor.Yellow);
            colors.Add (LogLevel.Error, ConsoleColor.Red);
            colors.Add (LogLevel.Severe, ConsoleColor.DarkRed);
            colors.Add (LogLevel.Fatal, ConsoleColor.Magenta);
            colors.Add (LogLevel.Crash, ConsoleColor.DarkMagenta);

            fileWriter = new StreamWriter (logFile.FullName, append);
            fileWriter.AutoFlush = true;
            AddLogEntry (LogLevel.Info, moduleName, "Logfile opened.");
            if (Debugger.IsAttached)
                AddLogEntry (LogLevel.Info, moduleName, "Debugger attached, using DebugWriter");
            t.Start ();
        }

        /// <summary>
        /// Registers a new logging module for consistent output
        /// </summary>
        /// <param name="moduleName">Module name.</param>
        public void RegisterLogModule(string moduleName)
        {
            maxModNameLength = Math.Max (maxModNameLength, moduleName.Length);
        }

        /// <summary>
        /// Sets the log level which will be written to the file
        /// </summary>
        /// <param name="newLevel">New level.</param>
        public void SetLogLevel(LogLevel newLevel)
        {
            minL = newLevel;
        }

        /// <summary>Runs this instance.</summary>
        void run ()
        {
            try {
                while (!exit) {
                    while (linesToProcess.Count > 0) {
                        LogLine l;
                        lock (linesToProcess)
                            l = linesToProcess.Dequeue ();
                        string s = l.ToString (maxModNameLength);
                        if ((int)l.LogLevel >= (int)minL)
                            ConsoleExtension.WriteLine (l.ToConsoleString(maxModNameLength), colors [l.LogLevel]);
                        fileWriter.WriteLine (s);
                    }
                    Thread.Sleep (1);
                }
                Thread.Sleep (10);
                lock (linesToProcess) {
                    while (linesToProcess.Count > 0) {
                        var l = linesToProcess.Dequeue ();
                        string s = l.ToString (maxModNameLength);
                        if ((int)l.LogLevel >= (int)minL)
                            ConsoleExtension.WriteLine (l.ToConsoleString(maxModNameLength), colors [l.LogLevel]);
                        fileWriter.WriteLine (s);
                    }
                }
            } catch (Exception e) {
                AddLogEntry (LogLevel.Crash, moduleName, "Run failed: " + e.Message);
            }
        }

        /// <summary>Adds the log entry.</summary>
        /// <param name="level">The level.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="format">Message.</param>
        /// <param name="args">The arguments.</param>
        public void AddLogEntry (LogLevel level, string moduleName, string format, params object[] args)
        {
            var x = new LogLine { LogLevel = level, ModuleName = moduleName, Format = format, Param = args, Timestamp = DateTime.Now };
            AddLogEntry (x);
        }
            
        /// <summary>Adds the log entry.</summary>
        /// <param name="x">The line to add.</param>
        public void AddLogEntry (LogLine x)
        {
            maxModNameLength = Math.Max (maxModNameLength, x.ModuleName.Length);
            lock (linesToProcess) 
                linesToProcess.Enqueue (x);

        }

        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="moduleName">Module name.</param>
        /// <param name="status">Status code.</param>
        /// <param name="additionalMessage">Additional message.</param>
        /// <param name="args">Arguments.</param>
        public void AddLogEntry(LogLevel level, string moduleName, Status status, string additionalMessage = null, params object[] args)
        {
            var fd = StatusDescription.Descriptions[status];
            var x = new LogLine{ LogLevel = level, ModuleName = moduleName, Format = (additionalMessage ?? "Code:" + (int)status + "\n" + fd), Param = args, Timestamp = DateTime.Now };
            AddLogEntry(x);
        }



        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        public void Dispose ()
        {
            exit = true;
        }
    }
}