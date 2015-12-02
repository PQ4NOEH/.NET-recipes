using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Domain.Quota;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Quota
{
    public class ElapsedTimeQuotaSpec
    {
        [Fact]
        public void fullfills_returns_false_until_elapsed_time_has_taken()
        {
            var quota = new ElapsedTimeQuota(new Minute(1));
            Assert.False(quota.Fullfills(1));
        }
        [Fact]
        public async Task fullfills_returns_true_when_elapsed_time_has_taken()
        {
            var quota = new ElapsedTimeQuota(new Minute(1));
            await Task.Delay(TimeSpan.FromMinutes(1));
            Assert.True(quota.Fullfills(1));
        }
    }
}
