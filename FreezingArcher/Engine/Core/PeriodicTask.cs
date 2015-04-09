//
//  PeriodicTask.cs
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
using System.Diagnostics;
using System.Threading;
using FreezingArcher.Output;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Helper class for accurate timers
    /// <remarks>compatible to System.Timers.Timer</remarks>
    /// </summary>
    public class PeriodicTask
    {
        private Action onTick;

        /// <summary>
        /// Gets a value indicating whether this <see cref="PeriodicTask"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        public bool Running { get; private set; }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public long Interval { get; private set; }

        /// <summary>
        /// The stopwatch
        /// </summary>
        private Stopwatch sw;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicTask"/> class.
        /// </summary>
        /// <param name="interval">The interval in ms.</param>
        /// <param name="functor">Callback when period is over.</param>
        public PeriodicTask (long interval, Action functor)
        {
            if (interval < 0)
            {
                Logger.Log.AddLogEntry (LogLevel.Severe, "PeriodicTask", Status.BadArgument,
                    "Your interval period '{0}' is negative!", interval);
                throw new ArgumentOutOfRangeException ("interval",
                    "Your interval period '" + interval + "' is negative!");
            }

            if (functor == null)
            {
                Logger.Log.AddLogEntry (LogLevel.Severe, "PeriodicTask", Status.YouShallNotPassNull,
                    "Your period callback shall not be null!");
                throw new ArgumentNullException ("functor", "You shall not pass!");
            }

            onTick = functor;
            sw = new Stopwatch ();
            Running = false;
            Interval = interval;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start ()
        {
            Running = true;
            Thread thread = new Thread (Run);
            thread.Priority = ThreadPriority.Normal;
            thread.Start ();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop ()
        {
            Running = false;
        }

        /// <summary>
        /// method the thread will be running
        /// </summary>
        private void Run ()
        {
            int toSleep;
            sw.Start ();

            while (Running) 
            {
                toSleep = (int) (Interval - sw.ElapsedMilliseconds);
                #if DEBUG_PERFORMANCE
                Logger.Log.AddLogEntry (LogLevel.Debug, "PeriodicTask", "Task took {0} ticks to execute.",
                    sw.ElapsedTicks);
                #endif
                if (toSleep > 0)
                    Thread.Sleep (toSleep);
                sw.Restart ();
                onTick ();
            }
            sw.Stop ();
            sw.Reset ();
        }
    }
}
