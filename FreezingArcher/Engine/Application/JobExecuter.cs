//
//  JobExecuter.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
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
using System.Threading.Tasks;

namespace FreezingArcher.Application
{
    /// <summary>
    /// Job executer.
    /// </summary>
    public class JobExecuter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Application.JobExecuter"/> class.
        /// </summary>
        public JobExecuter ()
        {
            Jobs = new List<Action> ();
        }

        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        /// <value>The jobs.</value>
        public List<Action> Jobs { get; set; }

        /// <summary>
        /// The currently active jobs
        /// </summary>
        protected List<Action> currentJobs;

        /// <summary>
        /// Inserts the job.
        /// </summary>
        /// <param name="job">Job.</param>
        public void InsertJob (Action job)
        {
            Jobs.Add (job);
        }

        /// <summary>
        /// Inserts the jobs.
        /// </summary>
        /// <param name="jobs">Jobs.</param>
        public void InsertJobs (List<Action> jobs)
        {
            Jobs.AddRange (jobs);
        }

        /// <summary>
        /// Inserts the jobs.
        /// </summary>
        /// <param name="jobs">Jobs.</param>
        public void InsertJobs (Action[] jobs)
        {
            Jobs.AddRange (jobs);
        }

        /// <summary>
        /// Execs the jobs parallel.
        /// </summary>
        /// <param name="load">Load.</param>
        public void ExecJobsParallel (int load)
        {
            currentJobs = new List<Action> (Jobs);
            Jobs.Clear ();

            ParallelOptions ops = new ParallelOptions ();
            ops.MaxDegreeOfParallelism = load;

            if (currentJobs.Count > 0)
                Parallel.Invoke (ops, currentJobs.ToArray ());

            currentJobs.Clear ();
            currentJobs = null;
        }

        /// <summary>
        /// Execs the jobs sequential.
        /// </summary>
        public void ExecJobsSequential ()
        {
            currentJobs = new List<Action> (Jobs);
            Jobs.Clear ();

            currentJobs.ForEach (a => a ());
            currentJobs.Clear ();
            currentJobs = null;
        }

        /// <summary>
        /// Handler for a NeedsReexec event or delegate.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="args">Arguments.</param>
        public void NeedsReexecHandler (object action, EventArgs args)
        {
            Action act = action as Action;

            if (act == null)
                throw new ArgumentException ("Action should be of type Action!");

            Jobs.Add (act);
        }
    }
}
