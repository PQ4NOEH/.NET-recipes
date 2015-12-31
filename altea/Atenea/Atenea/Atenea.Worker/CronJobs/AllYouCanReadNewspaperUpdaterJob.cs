namespace Atenea.Worker.CronJobs
{
    using System;

    using Atenea.AllYouCanReadUpdater;

    public class AllYouCanReadNewspaperUpdaterJob : AllYouCanReadUpdaterJob, ICronJob
    {
        private static readonly TimeSpan MinTimeSpan;

        static AllYouCanReadNewspaperUpdaterJob()
        {
            // 15 days +- 0.001 (sampling error)
            AllYouCanReadNewspaperUpdaterJob.MinTimeSpan = TimeSpan.FromTicks((long)(TimeSpan.FromDays(15d).Ticks * 0.999));
        }

        public AllYouCanReadNewspaperUpdaterJob()
            : base(AllYouCanReadType.Newspaper)
        {
        }

        public override string CronSchedule
        {
            get
            {
                // Job fires each 15 days
                return "0 0 0 */15 * ? *";
            }
        }

        protected override string CloudBlockBlobName
        {
            get
            {
                return "wisenet_newspapers_lease.lck";
            }
        }

        protected override TimeSpan WaitTime
        {
            get
            {
                return AllYouCanReadNewspaperUpdaterJob.MinTimeSpan;
            }
        }
    }
}
