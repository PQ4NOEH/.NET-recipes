namespace Atenea.Worker.CronJobs
{
    using System;

    using Atenea.AllYouCanReadUpdater;

    public class AllYouCanReadMagazineUpdaterJob : AllYouCanReadUpdaterJob, ICronJob
    {
        private static readonly TimeSpan MinTimeSpan;

        static AllYouCanReadMagazineUpdaterJob()
        {
            // 3 days +- 0.001 (sampling error)
            AllYouCanReadMagazineUpdaterJob.MinTimeSpan = TimeSpan.FromTicks((long)(TimeSpan.FromDays(3d).Ticks * 0.999));
        }

        public AllYouCanReadMagazineUpdaterJob()
            : base(AllYouCanReadType.Magazine)
        {
        }

        public override string CronSchedule
        {
            get
            {
                // Job fires each 72 hours
                return "0 0 0 */3 * ? *";
            }
        }

        protected override string CloudBlockBlobName
        {
            get
            {
                return "wisenet_magazines_lease.lck";
            }
        }

        protected override TimeSpan WaitTime
        {
            get
            {
                return AllYouCanReadMagazineUpdaterJob.MinTimeSpan;
            }
        }
    }
}
