
namespace Davalor.MomProxy.Domain.Quota
{
    public interface IQuota
    {
        bool Fullfills(int numberOfElements);
    }
}
