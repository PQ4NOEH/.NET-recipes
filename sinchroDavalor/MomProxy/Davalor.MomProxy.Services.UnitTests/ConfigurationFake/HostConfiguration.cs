using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Davalor.MomProxy.Services.UnitTests.ConfigurationFake
{
    public class HostConfiguration : IHostConfiguration
    {
        List<string> _brokenRules = new List<string>();
        public int WebListenerPort { get; set; }
        public List<TopicConfiguration> Topics = new List<TopicConfiguration>();

        public string ApplicationName
        {
            get
            {
                return "Davalor.MomProxy";
            }
        }
        public string MachineName
        {
            get
            {
                return Environment.MachineName;
            }
        }
        public ITopicConfiguration Topic(NotNullOrWhiteSpaceString topic)
        {
            return Topics
                    .Where(t=> t.TopicName.Equals(topic.Value, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
        }

        public IEnumerable<string> BrokenRules
        {
            get 
            {
                return _brokenRules;
            }
        }

        public bool IsValid()
        {
            _brokenRules.Clear();
            Topics.ForEach(tConfiguration =>
                {
                    if (!tConfiguration.IsValid()) _brokenRules.AddRange(tConfiguration.BrokenRules);
                });
            return _brokenRules.Count == 0;
        }
    }

   
}
