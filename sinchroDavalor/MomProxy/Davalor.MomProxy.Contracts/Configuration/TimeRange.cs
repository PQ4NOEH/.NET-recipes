using Davalor.Base.Contract.Library;
using System;
using System.Collections.Generic;

namespace Davalor.MomProxy.Domain.Configuration
{
    public class TimeRange : IValidable
    {
        public Time From { get; set; }
        public Time To { get; set; }

        public bool InRange(DateTime date)
        {
            return ComplyWithLowerBound(date) && ComplyWithUpperBound(date);
        }
        bool ComplyWithLowerBound(DateTime date)
        {
            return date.Hour > From.Hour.Value || (date.Hour == From.Hour.Value && date.Minute >= From.Minute.Value);
        }
        bool ComplyWithUpperBound(DateTime date)
        {
            return date.Hour < To.Hour.Value || (date.Hour == To.Hour.Value && date.Minute <= To.Minute.Value);
        }
        List<string> _brokenRules = new List<string>();

        public System.Collections.Generic.IEnumerable<string> BrokenRules
        {
            get { return _brokenRules; }
        }

        public bool IsValid()
        {
            _brokenRules.Clear();
            if(From == null || To == null )
            {
                _brokenRules.Add("If TimeRange is configured both From and To has to be configured.");
            }
            else if (From != null && To != null && From >= To)
            {
                _brokenRules.Add("From has to be lower than To.");
            }
            return _brokenRules.Count == 0;
        }
    }
}
