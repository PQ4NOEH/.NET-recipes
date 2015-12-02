using Davalor.SAP.Messages.Country;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Repository.Country;
using Moq;
using System.Reflection;
using System.Linq;
using Xunit;
using System;

namespace Davalor.SynchronizationManager.Repository.UnitTests
{
    public class SynchroRepositoryFactorySpec
    {
        [Fact]
        public void Given_any_instance_when_request_a_repository_then_returns_an_instance_of_the_repository()
        {
            var mock = new Mock<IHostConfiguration>();
            mock.Setup(s => s.PortalPacienteConnectionString).Returns("Any string value");
            mock.Setup(s => s.VisionLocalConnectionString).Returns("Any string value");
            var sut = new SynchroRepositoryFactory(mock.Object);

            var repository = sut.CreateDataService<CountryAggregate>();
            Assert.NotNull(repository);
            Assert.Equal(repository.GetType().Name, typeof(CountryRepository).Name);
        }

        [Fact]
        public void Given_arepository_whith_two_posible_context_when_request_one_it_then_creates_the_repository_with_the_requested_context()
        {
            var mock = new Mock<IHostConfiguration>();
            mock.Setup(s => s.PortalPacienteConnectionString).Returns("Any string value");
            mock.Setup(s => s.VisionLocalConnectionString).Returns("Any string value");
            var sut = new SynchroRepositoryFactory(mock.Object);

            var repository = sut.CreateDataService<CountryAggregate>(ESynchroSystem.PortalPaciente);
            Assert.NotNull(repository);
            var dbContextField = repository
                            .GetType()
                            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(f => f.Name == "_dbContext").First();
            var dbContextValue = dbContextField.GetValue(repository);
            Assert.NotNull(dbContextField);
            repository.Delete(Guid.NewGuid());
        }
    }


}
