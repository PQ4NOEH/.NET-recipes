using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Domain.Quota;
using System;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Quota
{
    public class TimeRangeQuotaSpec
    {
        [Fact]
        public void Fullfill_returns_false_if_currentTime_is_out_of_the_configured_TimeRange()
        {
            var currrentTime = DateTime.Now;
            TimeRange timeRange = new TimeRange
            {
                From = new Time()
                {
                    Hour = new Hour(currrentTime.Hour + 1),
                    Minute = new Minute(3)
                },
                To = new Time()
                {
                    Hour = new Hour(currrentTime.Hour + 1),
                    Minute = new Minute(4)
                }
            };
            var sut = new TimeRangeQuota(timeRange);
            Assert.False(sut.Fullfills(1));
        }

        [Fact]
        public void Fullfill_returns_true_if_currentTime_is_within_the_configured_TimeRange()
        {
            var currrentTime = DateTime.Now;
            TimeRange timeRange = new TimeRange
            {
                From = new Time()
                {
                    Hour = new Hour(currrentTime.Hour - 1),
                    Minute = new Minute(3)
                },
                To = new Time()
                {
                    Hour = new Hour(currrentTime.Hour + 1),
                    Minute = new Minute(4)
                }
            };
            var sut = new TimeRangeQuota(timeRange);
            Assert.True(sut.Fullfills(1));
        }
    }
}
