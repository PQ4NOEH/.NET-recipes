using Davalor.SAP.Messages.Country;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using System;
using System.Collections.Generic;
using NSubstitute;
using Xunit;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.MessageHandlers.UnitTests
{
    public class CountryMessageHandlerSpec
    {
        [Fact]
        public void GIVEN_no_system_configured_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<CountryAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("CountryMessageHandler", new List<ESynchroSystem>()));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<CountryAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new CountryMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredCountry");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<CountryAggregate>());
        }
        [Fact]
        public void GIVEN_no_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<CountryAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("CountryMessageHandler", null));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<CountryAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new CountryMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredCountry");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<CountryAggregate>());
        }
        [Fact]
        public void GIVEN_null_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<CountryAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<CountryAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new CountryMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredCountry");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<CountryAggregate>());
        }
        [Fact]
        public void GIVEN_a_RegisteredCountry_message_WHEN_listened_THEN_the_aggregate_is_saved_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<CountryAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("CountryMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<CountryAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new CountryMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredCountry");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Save(aggregate);
        }
        [Fact]
        public void GIVEN_a_ChangedCountry_message_WHEN_listened_THEN_the_aggregate_is_updated_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<CountryAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("CountryMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<CountryAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new CountryMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("ChangedCountry");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Update(aggregate);
        }
        [Fact]
        public void GIVEN_a_UnregisteredCountry_message_WHEN_listened_THEN_the_aggregate_is_removed_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<CountryAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("CountryMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<CountryAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new CountryMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("UnregisteredCountry");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Delete(aggregate.Id);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<CountryAggregate>());
        }
        CountryAggregate GenerateRandomAggregate()
        {
            return new CountryAggregate
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}
