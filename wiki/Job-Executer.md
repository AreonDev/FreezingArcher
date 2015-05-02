---
layout: wikipage
title: Job Executer
wikiPageName: Job-Executer
menu: wiki
---

```c#
namespace FreezingArcher.Core
```

The `JobExecuter` executes jobs of type `Action`. This can either be done in a sequential, single threaded way or in a
parallel, multi threaded way. The job executer handles a `NeedsReexec` event if a job needs to be executed again for
some reason.

## Example:

```c#
List<Action> jobs = new List<Action> ();
// add jobs
...

exec = new JobExecuter ();
exec.InsertJobs (jobs);
// handle if the job executer recognizes that some jobs need to be executed again
exec.DoReexec += () => { ExecAgain = true; };
exec.ExecJobsParallel (Environment.ProcessorCount);
...

while (run)
{
    ...
    if (ExecAgain)
        exec.ExecJobsParallel (Environment.ProcessorCount);
    ...
}
```

