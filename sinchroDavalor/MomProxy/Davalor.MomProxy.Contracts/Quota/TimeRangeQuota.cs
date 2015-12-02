using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain.Configuration;
using System;

namespace Davalor.MomProxy.Domain.Quota
{
    public class TimeRangeQuota : IQuota
    {
        readonly TimeRange _timeRange;
        public TimeRangeQuota(NotNullable<TimeRange> timeRange)
        {
            _timeRange = timeRange.Value;
        }
        public bool Fullfills(int numberOfElements)
        {
            return _timeRange.InRange(DateTime.Now);
        }
    }
}
