using Davalor.Base.Library.Guards;
using System;
using System.Collections.Generic;
namespace Davalor.MomProxy.Domain
{
    public interface IMomRepository
    {
        void SendMessages(NotNullOrWhiteSpaceString topic, NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> messages);
    }
}
