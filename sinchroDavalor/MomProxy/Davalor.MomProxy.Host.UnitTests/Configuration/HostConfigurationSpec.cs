using Davalor.MomProxy.Host.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.MomProxy.UnitTest.Configuration
{
    public class HostConfigurationSpec
    {
        [Fact]
        public void If_any_of_the_topics_configuration_is_not_valid_HostConfiguration_is_not_valid_either()
        {
            var sut = new  HostConfiguration();
            sut.Topics.Add(new TopicConfiguration());
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 2);
            Assert.Contains("topic name can't be null", sut.BrokenRules.ElementAt(0));
            Assert.Contains("a quota must be configured", sut.BrokenRules.ElementAt(1));
        }

        [Fact]
        public void Topic_returns_the_matching_topic()
        {
            var sut = new HostConfiguration();
            sut.Topics.Add(new TopicConfiguration() { TopicName = "Atopic"});
            sut.Topics.Add(new TopicConfiguration() { TopicName = "othertopic" });
            Assert.Equal(sut.Topic("Atopic").TopicName, "Atopic");
            Assert.Equal(sut.Topic("othertopic").TopicName, "othertopic");
        }

        [Fact]
        public void Instance_loads_the_configuration_from_file()
        {
            File.WriteAllText("HostConfiguration.json", "{\"Topics\":[{\"TopicName\":\"Atopic\",\"RawQuota\":{\"NumberOfELements\":13}}], \"WebListenerPort\":4325}");
            try
            {
                var config = HostConfiguration.Instance.Value;
                Assert.NotNull(config);
                Assert.Equal(config.WebListenerPort, 4325);
                Assert.NotEmpty(config.Topics);
                Assert.Equal(config.Topics.Count(), 1);
                var topic = config.Topics.ElementAt(0);
                Assert.Equal(topic.TopicName, "Atopic");
                Assert.NotNull(topic.Quota);
                Assert.Equal(topic.Quota.NumberOfELements, 13);
                
            }
            finally
            {
                File.Delete("HostConfiguration");
            }
        }
    }
}
