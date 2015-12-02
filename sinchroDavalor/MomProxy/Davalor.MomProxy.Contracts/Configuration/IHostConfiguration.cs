using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using System;
namespace Davalor.MomProxy.Domain.Configuration
{
    public interface IHostConfiguration : IValidable
    {
        ITopicConfiguration Topic(NotNullOrWhiteSpaceString topic);
        int WebListenerPort { get; set; }
        string ApplicationName { get; }
        string MachineName { get; }
    }
}
