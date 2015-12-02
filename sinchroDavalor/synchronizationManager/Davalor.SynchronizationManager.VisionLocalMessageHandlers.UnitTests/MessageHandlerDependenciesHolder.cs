using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Serialization;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using Davalor.SynchronizationManager.Domain.Repository;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.MessageHandlers.UnitTests
{
    public class MessageHandlerDependenciesHolder<T> where T : class, ISynchroAggregateRoot
    {
        public readonly ServiceEvents HostServiceEvents;
        public readonly ISynchroRepositoryFactory RepositoryFactory;
        public readonly ISynchroRepository<T> Repository;
        public readonly IMessageDecrypter MessageDecrypter;
        public readonly IBinarySerializer Serializer;
        public readonly IHostConfiguration HostConfiguration;
        
        public MessageHandlerDependenciesHolder()
        {
            HostServiceEvents = new ServiceEvents();
            RepositoryFactory = Substitute.For<ISynchroRepositoryFactory>();
            Repository = Substitute.For<ISynchroRepository<T>>();
            RepositoryFactory.CreateDataService<T>().Returns(Repository);
            RepositoryFactory.CreateDataService<T>(Arg.Any<ESynchroSystem>()).Returns(Repository);
            MessageDecrypter = Substitute.For<IMessageDecrypter>();
            Serializer = new BinaryJsonSerializer();
            HostConfiguration = Substitute.For<IHostConfiguration>();
        }
    }
}
