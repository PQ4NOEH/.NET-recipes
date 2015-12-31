namespace Heracles.Worker
{
    using Quartz;

    public interface IWorkerJob : IJob
    {
        string CronSchedule { get; }
        void Run();
        void Stop();
    }
}
