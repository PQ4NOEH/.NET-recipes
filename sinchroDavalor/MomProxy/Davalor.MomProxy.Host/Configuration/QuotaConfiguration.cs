using Davalor.MomProxy.Domain.Configuration;
using System.Collections.Generic;

namespace Davalor.MomProxy.Host.Configuration
{
    public class QuotaConfiguration : IQuotaConfiguration
    {
        List<string> _brokenRules = new List<string>();
        public int NumberOfELements { get; set; }
        public int ElapsedMinutes { get; set; }
        public TimeRange TimeRange { get; set; }

        public IEnumerable<string> BrokenRules
        {
            get { return _brokenRules; }
        }

        public bool IsValid()
        {
            _brokenRules.Clear();

            if (default(int) == NumberOfELements && default(int) == ElapsedMinutes && TimeRange == null)
            {
                _brokenRules.Add("If Quota is configured One quota parameter has to be configured.");
            }
            if (TimeRange != null && !TimeRange.IsValid())
            {
                _brokenRules.AddRange(TimeRange.BrokenRules);
            }
            if (    
                    (default(int) != NumberOfELements && default(int) != ElapsedMinutes) ||
                    (default(int) != NumberOfELements && TimeRange != null) ||
                    (default(int) != ElapsedMinutes && TimeRange != null)
                )
            {
                _brokenRules.Add("Only one quota parameter can be set per quota");
            }

            return _brokenRules.Count == 0;
        }
    }

    
}
