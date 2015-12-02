using Davalor.Base.Contract.Library;

namespace Davalor.MomProxy.Domain.Configuration
{
    public interface IQuotaConfiguration : IValidable
    {
        int NumberOfELements { get; set; }
        int ElapsedMinutes { get; set; }
        TimeRange TimeRange { get; set; }
    }
}
