
using Davalor.MomProxy.Domain.Configuration;
using System;
namespace Davalor.MomProxy.Domain.Quota
{
    public class ElapsedTimeQuota : IQuota
    {
        readonly int _elapsedMinutesQuota;
        DateTime _lastTime;
        public ElapsedTimeQuota(Minute elapsedMinutesQuota)
        {
            _elapsedMinutesQuota = elapsedMinutesQuota.Value;
            _lastTime = DateTime.Now;
        }
        public bool Fullfills(int numberOfElements)
        {
            var now = DateTime.Now;
            var fullfills = now.Subtract(_lastTime).Minutes >= _elapsedMinutesQuota;
            if (fullfills) _lastTime = now;
            return fullfills;
        }
    }
}
