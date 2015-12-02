
using Davalor.Base.Contract.Library;
namespace Davalor.MomProxy.Domain.Configuration
{
    public interface ITopicConfiguration : IValidable
    {
        IQuotaConfiguration Quota { get; set; }
        string TopicName { get; set; }
    }
}
