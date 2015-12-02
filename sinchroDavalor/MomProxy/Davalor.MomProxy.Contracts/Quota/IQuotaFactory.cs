using Davalor.Base.Library.Guards;
using System;
namespace Davalor.MomProxy.Domain.Quota
{
    public interface IQuotaFactory
    {
        NotNullable<IQuota> CreateQuota(NotNullOrWhiteSpaceString topic);
    }
}
