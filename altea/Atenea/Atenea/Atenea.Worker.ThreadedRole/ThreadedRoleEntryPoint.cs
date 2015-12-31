namespace Atenea.Worker.ThreadedRole
{
    using System;
    using System.Threading;

    using Microsoft.WindowsAzure.ServiceRuntime;

    public abstract class ThreadedRoleEntryPoint : RoleEntryPoint
    {
        private Thread[] threads;
        private WorkerEntryPoint[] activeWorkers;
        private int numWorkers;

        /// <summary>
        /// Thread safe stop signal, stops the Run main loop
        /// when set, allowing the Role to safely stop.
        /// </summary>
        private EventWaitHandle eventWaitHandle;

        public override bool OnStart()
        {
            throw new InvalidOperationException();
        }

        public bool OnStart(WorkerEntryPoint[] workers)
        {
            if (workers == null)
            {
                throw new InvalidOperationException();
            }

            this.activeWorkers = workers;

            foreach (WorkerEntryPoint worker in this.activeWorkers)
            {
                worker.OnStart();
            }

            this.numWorkers = this.activeWorkers.Length;
            this.threads = new Thread[this.numWorkers];
            this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            return base.OnStart();
        }

        public override void Run()
        {
            for (int i = 0; i < this.numWorkers; i++)
            {
                Thread thread = new Thread(this.activeWorkers[i].ProtectedRun);
                this.threads[i] = thread;
                thread.Start();
            }

            // Check thread health every ten minutes
            while (!this.eventWaitHandle.WaitOne(0))
            {
                // Restart dead threads
                for (int i = 0; i < this.numWorkers; i++)
                {
                    Thread thread = this.threads[i];
                    if (thread.IsAlive)
                    {
                        continue;
                    }

                    thread = new Thread(this.activeWorkers[i].ProtectedRun);
                    this.threads[i] = thread;
                    thread.Start();
                }

                this.eventWaitHandle.WaitOne(10 * 60 * 1000);
            }
        }

        public override void OnStop()
        {
            this.eventWaitHandle.Set();

            foreach (Thread thread in this.threads)
            {
                if (thread == null)
                {
                    continue;
                }

                while (thread.IsAlive)
                {
                    thread.Abort();
                }
            }

            // Make sure the threads are not running
            foreach (Thread thread in this.threads)
            {
                if (thread == null)
                {
                    continue;
                }

                while (thread.IsAlive)
                {
                    Thread.Sleep(10);
                }
            }

            // Stop the workers loop
            foreach (WorkerEntryPoint worker in this.activeWorkers)
            {
                if (worker == null)
                {
                    continue;
                }

                worker.OnStop();
            }

            base.OnStop();
        }
    }
}
