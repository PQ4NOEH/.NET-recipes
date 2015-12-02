using Davalor.MomProxy.Host.Configuration;
using System;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.UnitTest.Configuration
{
    public class TopicConfigurationSpec
    {
        [Fact]
        public void If_the_topicName_is_not_set_it_is_invalid()
        {
            var sut = new TopicConfiguration
            {
                Quota = new QuotaConfiguration
                {
                    ElapsedMinutes = 1
                }
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("The topic name can't be null", sut.BrokenRules.ElementAt(0));
        }
        [Fact]
        public void If_the_quota_is_not_set_it_is_invalid()
        {
            var sut = new TopicConfiguration
            {
                TopicName ="aTopic"
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("quota must be configured", sut.BrokenRules.ElementAt(0));
        }
        [Fact]
        public void If_the_quota_is_not_properly_set_it_is_invalid()
        {
            var sut = new TopicConfiguration
            {
                TopicName = "aTopic",
                Quota = new QuotaConfiguration()
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("One quota parameter has to be configured", sut.BrokenRules.ElementAt(0));
        }
    }
}
