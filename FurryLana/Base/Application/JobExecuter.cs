using FurryLana.Engine.Graphics.Interfaces;
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

namespace FurryLana.Base.Application
{
    public class JobExecuter
    {
        public JobExecuter ()
        {
            Jobs = new List<Action> ();
        }

        public List<Action> Jobs { get; protected set; }

        public void InsertJob (Action job)
        {
            Jobs.Add (job);
        }

        public void InsertJobs (List<Action> jobs)
        {
            Jobs.AddRange (jobs);
        }

        public void InsertJobs (Action[] jobs)
        {
            Jobs.AddRange (jobs);
        }

        public void ExecJobsParallel (int load)
        {
            ParallelOptions ops = new ParallelOptions ();
            ops.MaxDegreeOfParallelism = load;
            Parallel.Invoke (ops, Jobs.ToArray ());
            Jobs.Clear ();
        }

        public void ExecJobsSequential ()
        {
            Jobs.ForEach (a => a ());
            Jobs.Clear ();
        }

        public void NeedsReexecHandler (object action, EventArgs args)
        {
            Action act = action as Action;

            if (act == null)
                throw new ArgumentException ("Action should be of type Action!");

            Jobs.Add (act);
        }
    }
}
