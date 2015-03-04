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
using FreezingArcher.Output;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Job executer.
    /// </summary>
    public class JobExecuter
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "JobExecuter";

        /// <summary>
        /// Do reexec handler.
        /// </summary>
        public delegate void DoReexecHandler ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.JobExecuter"/> class.
        /// </summary>
        public JobExecuter ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Creating new job executer ...");
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
        protected List<Action> CurrentJobs;

        /// <summary>
        /// Inserts the job.
        /// </summary>
        /// <param name="job">Job.</param>
        public void InsertJob (Action job)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Inserting new job '{0}' into job executer",
                job.Method.Name);
            Jobs.Add (job);
        }

        /// <summary>
        /// Inserts the jobs.
        /// </summary>
        /// <param name="jobs">Jobs.</param>
        public void InsertJobs (List<Action> jobs)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Inserting {0} new jobs into job executer", jobs.Count);
            Jobs.AddRange (jobs);
        }

        /// <summary>
        /// Inserts the jobs.
        /// </summary>
        /// <param name="jobs">Jobs.</param>
        public void InsertJobs (Action[] jobs)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Inserting {0} new jobs into job executer", jobs.Length);
            Jobs.AddRange (jobs);
        }

        /// <summary>
        /// Execs the jobs parallel.
        /// </summary>
        /// <param name="load">Load.</param>
        public void ExecJobsParallel (int load)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                "Executing {0} jobs parallel with an average load of {1}", Jobs.Count, load);
            CurrentJobs = new List<Action> (Jobs);
            Jobs.Clear ();

            ParallelOptions ops = new ParallelOptions ();
            ops.MaxDegreeOfParallelism = load;

            if (CurrentJobs.Count > 0)
                Parallel.Invoke (ops, CurrentJobs.ToArray ());

            CurrentJobs.Clear ();
            CurrentJobs = null;
        }

        /// <summary>
        /// Execs the jobs sequential.
        /// </summary>
        public void ExecJobsSequential ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Executing {0} jobs sequentially", Jobs.Count);
            CurrentJobs = new List<Action> (Jobs);
            Jobs.Clear ();

            CurrentJobs.ForEach (a => a ());
            CurrentJobs.Clear ();
            CurrentJobs = null;
        }

        /// <summary>
        /// Handler for a NeedsReexec event or delegate.
        /// </summary>
        /// <param name="action">Action.</param>
        public void NeedsReexecHandler (Action action)
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                "Adding new job '{0}' to job executer via the NeedsReexecHandler", action.Method.Name);
            Jobs.Add (action);
            if (DoReexec != null)
                DoReexec ();
        }

        /// <summary>
        /// Occurs when job executer needs to be executed again.
        /// </summary>
        public event DoReexecHandler DoReexec;
    }
}
