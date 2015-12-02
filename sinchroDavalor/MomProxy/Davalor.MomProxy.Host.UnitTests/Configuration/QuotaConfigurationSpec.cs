using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Host.Configuration;
using System;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.UnitTest.Configuration
{
    public class QuotaConfigurationSpec
    {
        [Fact]
        public void If_non_of_the_quota_parameters_are_set_is_Invalid()
        {
            var sut = new QuotaConfiguration();
            Assert.False(sut.IsValid());
        }
        [Fact]
        public void If_two_or_more_parameters_are_configured_is_invalid()
        {
            var sut = new QuotaConfiguration
            {
                NumberOfELements = 1,
                ElapsedMinutes = 1
            };
            Assert.False(sut.IsValid());
            Assert.Contains("Only one quota parameter can be set per quota", sut.BrokenRules.ElementAt(0));

            sut = new QuotaConfiguration
            {
                NumberOfELements = 1,
                TimeRange = GenerateTimeRange()
            };
            Assert.False(sut.IsValid());
            Assert.Contains("Only one quota parameter can be set per quota", sut.BrokenRules.ElementAt(0));

            sut = new QuotaConfiguration
            {
                ElapsedMinutes = 1,
                TimeRange = GenerateTimeRange()
            };
            Assert.False(sut.IsValid());
            Assert.Contains("Only one quota parameter can be set per quota", sut.BrokenRules.ElementAt(0));
        }

        [Fact]
        public void If_TimeRange_is_configured_but_is_invalid_the_quota_configuration_is_invalid_too()
        {
            var sut = new QuotaConfiguration
            {
                TimeRange = new TimeRange()
            };
            Assert.False(sut.IsValid());
            Assert.Contains("both From and To has to be configured", sut.BrokenRules.ElementAt(0));
        }

        TimeRange GenerateTimeRange()
        {
            return new TimeRange
            {
                From = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(5)
                },
                To = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };
        }
    }
}
