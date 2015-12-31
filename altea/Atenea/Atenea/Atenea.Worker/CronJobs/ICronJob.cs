namespace Atenea.Worker.CronJobs
{
    using Quartz;

    public interface ICronJob : IJob
    {
        string CronSchedule { get; }

        void Run();

        void Stop();
    }
}