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

namespace FreezingArcher.Base.Utils
{
    /// <summary>
    /// Helper class for accurate timers
    /// <remarks>compatible to System.Timers.Timer</remarks>
    /// </summary>
    public class AccurateTimer
    {

        private Action<int> onTick;
        /// <summary>
        /// Gets a value indicating whether this <see cref="AccurateTimer"/> is running.
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
        public double Interval { get; private set; }

        /// <summary>
        /// The stopwatch
        /// </summary>
        private System.Diagnostics.Stopwatch sw;
        /// <summary>
        /// The interval to wait
        /// </summary>
        private double intervalToWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccurateTimer"/> class.
        /// </summary>
        /// <param name="interval_">The interval.</param>
        public AccurateTimer (double interval, Action<int> functor) //in µs
        {
            onTick = functor;
            sw = new System.Diagnostics.Stopwatch ();
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
            Debug.Print (Stopwatch.Frequency.ToString () + " ticks per second in System.Diagnostics.Stopwatch");
            sw.Start ();
            intervalToWait = Interval;

            while (Running) 
            {
                Thread.Sleep (1);
                if (intervalToWait <= 0) {
                    sw.Restart ();
                    onTick ((int)(-intervalToWait)); //pass interval 

                }
                intervalToWait = Interval - (((double)sw.ElapsedTicks / (double)Stopwatch.Frequency) * 1000000d); //inµs
            }
            sw.Stop ();
            sw.Reset ();
        }
    }
}
