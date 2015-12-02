using Davalor.Base.Library.Guards;

namespace Davalor.MomProxy.Domain.Quota
{
    public class NumberOfElementsQuota : IQuota
    {
        readonly uint _numberOfElements;
        public NumberOfElementsQuota(NotDefault<uint> numberOfElements)
        {
            _numberOfElements = numberOfElements.Value;
        }
        public bool Fullfills(int numberOfElements)
        {
            return numberOfElements >= _numberOfElements;
        }
    }
}
