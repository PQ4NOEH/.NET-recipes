using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain.Configuration;

namespace Davalor.MomProxy.Domain.Quota
{
    public class QuotaFactory : IQuotaFactory
    {
        readonly IHostConfiguration _hostConfiguration;
        public QuotaFactory(NotNullable<IHostConfiguration> hostConfiguration)
        {
            _hostConfiguration = hostConfiguration.Value;
        }

        public NotNullable<IQuota> CreateQuota(NotNullOrWhiteSpaceString topic)
        {
            ITopicConfiguration config = _hostConfiguration.Topic(topic);
            if (config == null || config.Quota == null) return new TransparentQuota();
            else if (config.Quota.NumberOfELements != default(int)) return new NumberOfElementsQuota((uint)config.Quota.NumberOfELements);
            else if (config.Quota.ElapsedMinutes != default(int)) return new ElapsedTimeQuota(new Minute(config.Quota.ElapsedMinutes));
            else if (config.Quota.TimeRange != null) return new TimeRangeQuota(config.Quota.TimeRange);
            return new TransparentQuota();
        }
    }
}
