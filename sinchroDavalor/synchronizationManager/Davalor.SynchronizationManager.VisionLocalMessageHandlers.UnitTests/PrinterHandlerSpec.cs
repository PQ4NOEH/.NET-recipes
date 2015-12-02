using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using System;
using System.Collections.Generic;
using NSubstitute;
using Xunit;
using Davalor.SAP.Messages.Printer;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.MessageHandlers.UnitTests
{
    public class PrinterHandlerSpec
    {
        [Fact]
        public void GIVEN_no_System_is_configured_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<PrinterAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("PrinterMessageHandler", new List<ESynchroSystem>()));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<PrinterAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new PrinterMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredPrinter");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<PrinterAggregate>());
        }
        [Fact]
        public void GIVEN_null_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<PrinterAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<PrinterAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new PrinterMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredPrinter");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<PrinterAggregate>());
        }
        [Fact]
        public void GIVEN_no_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<PrinterAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("PrinterMessageHandler", null));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<PrinterAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new PrinterMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredPrinter");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<PrinterAggregate>());
        }
        [Fact]
        public void GIVEN_a_RegisteredPrinter_message_WHEN_listened_THEN_the_aggregate_is_saved_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<PrinterAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("PrinterMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<PrinterAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new PrinterMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredPrinter");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Save(aggregate);
        }
        [Fact]
        public void GIVEN_a_ChangedPrinter_message_WHEN_listened_THEN_the_aggregate_is_updated_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<PrinterAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("PrinterMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<PrinterAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new PrinterMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("ChangedPrinter");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Update(aggregate);
        }
        [Fact]
        public void GIVEN_a_UnregisteredPrinter_message_WHEN_listened_THEN_the_aggregate_is_removed_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<PrinterAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("PrinterMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<PrinterAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new PrinterMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("UnregisteredPrinter");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Delete(aggregate.Id);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<PrinterAggregate>());
        }
        PrinterAggregate GenerateRandomAggregate()
        {
            return new PrinterAggregate
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}
