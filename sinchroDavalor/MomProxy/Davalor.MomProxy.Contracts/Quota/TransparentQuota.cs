
namespace Davalor.MomProxy.Domain.Quota
{
    public class TransparentQuota : IQuota
    {
        public bool Fullfills(int numberOfElements)
        {
            return numberOfElements > 0;
        }
    }
}
