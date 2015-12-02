using Davalor.Base.Library.Guards;
using System;
using System.Collections.Generic;
namespace Davalor.MomProxy.Domain
{
    public interface IIncommingMessageRepository
    {
        void Delete(NotNullOrWhiteSpaceString message);
        NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> GetPending();
        void Save(NotNullOrWhiteSpaceString message);
    }
}
