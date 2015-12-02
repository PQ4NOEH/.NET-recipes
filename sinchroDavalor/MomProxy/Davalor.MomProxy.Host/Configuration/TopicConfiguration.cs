using Davalor.MomProxy.Domain.Configuration;
using System.Collections.Generic;

namespace Davalor.MomProxy.Host.Configuration
{
    public class TopicConfiguration : ITopicConfiguration
    {
        List<string> _brokenRules = new List<string>();
        public string TopicName { get; set; }
        public IQuotaConfiguration Quota 
        { 
            get
            {
                return RawQuota;
            } 
            set
            {
                RawQuota = value as QuotaConfiguration;
            } 
        }

        public QuotaConfiguration RawQuota { get; set; }

        public IEnumerable<string> BrokenRules
        {
            get { return _brokenRules; }
        }

        public bool IsValid()
        {
            _brokenRules.Clear();
            if (string.IsNullOrWhiteSpace(TopicName)) _brokenRules.Add("The topic name can't be null empty or white space.");
            if (Quota == null) _brokenRules.Add("If topic is configured a quota must be configured too.");
            else if (!Quota.IsValid()) _brokenRules.AddRange(Quota.BrokenRules);
            return _brokenRules.Count == 0;
        }
    }
}
