#pragma warning disable 0420

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FreezingArcher.Math;

namespace Henge3D
{
    #region TaskException class

    /// <summary>
    /// Task exception.
    /// </summary>
    public class TaskException : Exception
    {
        Action<TaskParams> _task;

        internal Action<TaskParams> Task { get { return _task; } }

        internal TaskException (Action<TaskParams> task, Exception e)
            : base ("Unhandled exception in task ("
            + task.Target.ToString () + ", " + task.Method.ToString () + "): " + e.Message, e)
        {
            _task = task;
        }
    }

    #endregion

    #region TaskManagerException class

    /// <summary>
    /// Task manager exception.
    /// </summary>
    public class TaskManagerException : Exception
    {
        TaskException[] _exceptions;

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <value>The exceptions.</value>
        public TaskException[] Exceptions { get { return _exceptions; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.TaskManagerException"/> class.
        /// </summary>
        /// <param name="exceptions">Exceptions.</param>
        public TaskManagerException (TaskException[] exceptions)
        {
            _exceptions = exceptions;
        }
    }

    #endregion

    #region TaskParams struct

    /// <summary>
    /// Task parameters.
    /// </summary>
    public struct TaskParams
    {
        /// <summary>
        /// The param1.
        /// </summary>
        public object Param1;
        /// <summary>
        /// The param2.
        /// </summary>
        public object Param2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.TaskParams"/> struct.
        /// </summary>
        /// <param name="param1">Param1.</param>
        /// <param name="param2">Param2.</param>
        public TaskParams (object param1, object param2)
        {
            Param1 = param1;
            Param2 = param2;
        }
    }

    #endregion

    /// <summary>
    /// Facilitates the concurrent execution of tasks.
    /// </summary>
    public class TaskManager : IDisposable
    {
        private static TaskManager _current;
        private static int _threadCount;

        static TaskManager ()
        {
            _threadCount = System.Environment.ProcessorCount;
        }

        /// <summary>
        /// Gets a reference to the task manager currently managing tasks in the process.
        /// </summary>
        public static TaskManager Current
        {
            get
            {
                if (_current == null)
                    _current = new TaskManager ();
                return _current;
            }
        }


        /// <summary>
        /// Gets the supproted number of threads.
        /// </summary>
        public static int ThreadCount { get { return _threadCount; } }

        /// <summary>
        /// Gets the index of the calling thread.
        /// </summary>
        public static int CurrentThreadIndex
        {
            get
            {
                for (int i = 0; i < _threadCount; i++)
                {
                    if (_current._threads [i].ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        private volatile bool _disposing = false, _running = false;
        private volatile int _waitingThreadCount = 0, _currentTaskIndex = 0;
        private volatile Queue<Tuple<Action<TaskParams>, TaskParams>> _tasks;
        private Thread[] _threads;

        private AutoResetEvent _taskInitWaitHandle;
        private ManualResetEvent _managerWaitHandle;

        private TaskManager ()
        {
            _tasks = new Queue<Tuple<Action<TaskParams>, TaskParams>> ();
            _threads = new Thread[_threadCount];
            _taskInitWaitHandle = new AutoResetEvent (false);
            _managerWaitHandle = new ManualResetEvent (false);

            //_threads [0] = Thread.CurrentThread;

            for (int i = 0; i < _threadCount; i++)
            {
                _threads [i] = new Thread (() =>
                {
                    _taskInitWaitHandle.Set ();
                    ThreadProc ();
                });
                _threads [i].Start ();
                _taskInitWaitHandle.WaitOne ();
            }
        }

        /// <summary>
        /// Add a new task to the task manager to be executed at a later time.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <param name="parameters">Optinoal parameters to supply to the task.</param>
        public void AddTask (Action<TaskParams> task, TaskParams parameters)
        {
            lock (_tasks)
            {
                _tasks.Enqueue (new Tuple<Action<TaskParams>, TaskParams> (task, parameters));
            }
        }

        /// <summary>
        /// Add a new task to the task manager to be executed at a later time.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <param name="param1">The first optional parameter.</param>
        /// <param name="param2">The second optional parameter.</param>
        public void AddTask (Action<TaskParams> task, object param1, object param2)
        {
            this.AddTask (task, new TaskParams (param1, param2));
        }

        /// <summary>
        /// Add a new task to the task manager to be executed at a later time.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <param name="param">An optional parameter to supply to the task.</param>
        public void AddTask (Action<TaskParams> task, object param)
        {
            this.AddTask (task, new TaskParams (param, null));
        }

        /// <summary>
        /// Add a new task to the task manager to be executed at a later time.
        /// </summary>
        public void AddTask (Action<TaskParams> task)
        {
            this.AddTask (task, new TaskParams (null, null));
        }

        /// <summary>
        /// Executes all currently queued tasks. This method will not return until every task has completed.
        /// </summary>
        public void Execute ()
        {
            if (_disposing)
                throw new InvalidOperationException ();
            if (_running)
                throw new InvalidOperationException ();
            lock (_tasks)
            {
                if (_tasks.Count < 1) //nothing to do here
                return;
            }
            _running = true;

            try
            {
                _currentTaskIndex = 0;
                _waitingThreadCount = 0;
                _managerWaitHandle.Set ();

                //TaskPump ();

                while (_waitingThreadCount < _threadCount - 1)
                    Thread.Sleep (0);

            }
            finally
            {
                _managerWaitHandle.Reset ();
                _tasks.Clear ();
                _running = false;
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Henge3D.TaskManager"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Henge3D.TaskManager"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Henge3D.TaskManager"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Henge3D.TaskManager"/> so
        /// the garbage collector can reclaim the memory that the <see cref="Henge3D.TaskManager"/> was occupying.</remarks>
        /// <summary>
        /// Dispose the task manager.
        /// </summary>
        public void Dispose ()
        {
            _disposing = true;
            _managerWaitHandle.Set ();
        }

        private void ThreadProc ()
        {
            while (true)
            {
                Interlocked.Increment (ref _waitingThreadCount);
                _managerWaitHandle.WaitOne ();

                if (_disposing)
                    return;
                TaskPump ();
            }
        }

        private void TaskPump ()
        {
            Tuple<Action<TaskParams>, TaskParams> tuple;
            
            while (true)
            {
                lock (_tasks)
                {
                    if (_tasks.Count == 0)
                        return;
                    tuple = _tasks.Dequeue ();
                }
                tuple.Item1 (tuple.Item2);
            }
        }
    }
}
