using Davalor.MomProxy.Domain.Configuration;
using Davalor.MomProxy.Domain.Quota;
using Davalor.MomProxy.Domain.UnitTests.ConfigurationFake;
using System;
using System.Threading.Tasks;
using Xunit;


namespace Davalor.MomProxy.Domain.UnitTests.Quota
{
    public class QuotaFactorySpec
    {
        [Fact]
        public void If_configuration_is_null_creates_a_transparentQuota()
        {
            var hostConfiguration = new HostConfiguration();
            var sut = new QuotaFactory(hostConfiguration);
            Assert.IsType<TransparentQuota>(sut.CreateQuota("testTopic").Value);
        }

        [Fact]
        public void If_configuration_for_topic_exists_but_has_not_configured_any_quota_creates_a_transparentQuota()
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.Topics.Add(new TopicConfiguration
            {
                TopicName = "testTopic"
            });
            var sut = new QuotaFactory(hostConfiguration);
            Assert.IsType<TransparentQuota>(sut.CreateQuota("testTopic").Value);
        }
        [Fact]
        public void If_configuration_for_topic_exists_but_the_quota_has_not_any_parameter_configured_creates_a_transparentQuota()
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.Topics.Add(new TopicConfiguration
            {
                TopicName = "testTopic",
                Quota = new QuotaConfiguration()
            });
            var sut = new QuotaFactory(hostConfiguration);
            Assert.IsType<TransparentQuota>(sut.CreateQuota("testTopic").Value);
        }

        [Fact]
        public void If_the_requested_topic_has_configured_a_quota_with_numberOfElements_creates_a_NumberOfElementsQuota()
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.Topics.Add(new TopicConfiguration
                {
                    TopicName = "testTopic",
                    Quota = new QuotaConfiguration
                    {
                        NumberOfELements = 2
                    }
                });
            var sut = new QuotaFactory(hostConfiguration);
            Assert.IsType<NumberOfElementsQuota>(sut.CreateQuota("testTopic").Value);
        }
        [Fact]
        public void If_the_requested_topic_has_configured_a_quota_with_ElapsedMinutes_creates_an_ElapsedTimeQuota()
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.Topics.Add(new TopicConfiguration
            {
                TopicName = "testTopic",
                Quota = new QuotaConfiguration
                {
                    ElapsedMinutes = 2
                }
            });
            var sut = new QuotaFactory(hostConfiguration);
            Assert.IsType<ElapsedTimeQuota>(sut.CreateQuota("testTopic").Value);
        }
        [Fact]
        public void If_the_requested_topic_has_configured_a_quota_with_TimeRange_creates_an_TimeRangeQuota()
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.Topics.Add(new TopicConfiguration
            {
                TopicName = "testTopic",
                Quota = new QuotaConfiguration
                {
                    TimeRange = new TimeRange
                     {
                        From = new Time()
                        {
                            Hour = new Hour(10),
                            Minute = new Minute(3)
                        },
                        To = new Time()
                        {
                            Hour = new Hour(10),
                            Minute = new Minute(4)
                        }
                    }
                }
            });
            var sut = new QuotaFactory(hostConfiguration);
            Assert.IsType<TimeRangeQuota>(sut.CreateQuota("testTopic").Value);
        }
    }
}
