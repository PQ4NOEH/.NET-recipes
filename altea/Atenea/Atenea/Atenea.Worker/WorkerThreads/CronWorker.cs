namespace Atenea.Worker.WorkerThreads
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Atenea.Worker.CronJobs;
    using Atenea.Worker.ThreadedRole;

    using Microsoft.WindowsAzure.ServiceRuntime;

    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Triggers;

    internal class CronWorker : WorkerEntryPoint, IWorkerThread
    {
        private const string JobSuffix = "_Job";
        private const string TriggerSuffix = "_Trigger";
        private IScheduler scheduler;
        private IEnumerable<ICronJob> workers;
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override bool OnStart()
        {
            WorkerEntryPoint.TraceInformation(
                "cron worker is starting ({1})",
                DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            this.SetupQuartz();
            this.FindWorkers();

            bool result = base.OnStart();

            WorkerEntryPoint.TraceInformation("cron worker has been started");

            return result;
        }

        public override void Run()
        {
            foreach (ICronJob worker in this.workers)
            {
                try
                {
                    this.Schedule(worker);

                    WorkerEntryPoint.TraceInformation(
                        "cron worker is scheduling {1} ({2})",
                        RoleEnvironment.CurrentRoleInstance.Id,
                        worker.GetType().Name,
                        DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
                }
                catch (Exception ex)
                {
                    WorkerEntryPoint.TraceError(
                        "cron worker can't schedule {1} - {2}: {3} ({4})",
                        worker.GetType().Name,
                        ex.GetType().Name,
                        ex.Message,
                        DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
                }
            }

            this.runCompleteEvent.WaitOne();
        }

        public override void OnStop()
        {
            WorkerEntryPoint.TraceInformation("cron worker is stopping");

            this.scheduler.PauseAll();

            foreach (ICronJob worker in this.workers)
            {
                worker.Stop();
            }

            this.scheduler.Standby();
            this.runCompleteEvent.Set();

            base.OnStop();

            WorkerEntryPoint.TraceInformation("cron worker has stopped");
        }

        private void SetupQuartz()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            this.scheduler = schedulerFactory.GetScheduler();
            this.scheduler.Start();
        }

        private void FindWorkers()
        {
            Type jobInterface = typeof(ICronJob);
            IEnumerable<Type> workerTypes =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => jobInterface.IsAssignableFrom(x) && x.Name != jobInterface.Name)
                    .ToList();

            this.workers = workerTypes.Select(x => (ICronJob)Activator.CreateInstance(x)).ToArray();
        }

        private void Schedule(ICronJob worker)
        {
            if (worker == null)
            {
                return;
            }

            Type workerType = worker.GetType();
            string jobKey = string.Concat(workerType.Name, JobSuffix),
                   triggerKey = string.Concat(workerType.Name, TriggerSuffix);


            if (this.scheduler.CheckExists(new TriggerKey(jobKey)))
            {
                return;
            }

            IJobDetail job = JobBuilder.Create(workerType)
                .WithIdentity(jobKey)
                .Build();

            //ITrigger trigger =
            //    TriggerBuilder.Create()
            //        .WithIdentity(triggerKey)
            //        .ForJob(job)
            //        .StartNow()
            //        .WithCronSchedule(worker.CronSchedule)
            //        .Build();

            CronTriggerImpl trigger = new CronTriggerImpl(triggerKey)
                {
                    JobKey = job.Key,
                    StartTimeUtc = DateTime.UtcNow,
                    CronExpressionString = worker.CronSchedule,
                    TimeZone = TimeZoneInfo.Utc,
                };

            this.scheduler.ScheduleJob(job, trigger);
        }
    }
}
