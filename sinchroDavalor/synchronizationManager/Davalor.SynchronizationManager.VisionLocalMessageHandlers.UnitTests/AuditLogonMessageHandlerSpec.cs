using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using Davalor.SynchronizationManager.Domain.MessageHandling;
using Davalor.SynchronizationManager.Domain.Repository;
using Davalor.SynchronizationManager.MessageHandlers;
using Davalor.SynchronizationManager.MessageHandlers.UnitTests;
using Davalor.VisionLocal.Messages.AuditLogon;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace Davalor.SynchronizationManager.VisionLocalMessageHandlers.UnitTests
{
    public class AuditLogonMessageHandlerSpec
    {
        [Fact]
        public void GIVEN_a_RegisteredAuditLogon_message_WHEN_listened_THEN_the_aggregate_is_saved_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory, 
                                dependencies.MessageDecrypter, 
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Save(aggregate);
        }

        [Fact]
        public void GIVEN_null_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<AuditLogonAggregate>());
        }
        [Fact]
        public void GIVEN_no_system_configured_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>() ));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<AuditLogonAggregate>());
        }

        public void GIVEN_no_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", null));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<AuditLogonAggregate>());
        }

        [Fact]
        public void GIVEN_the_handler_has_many_systems_configured_WHEN_processing_a_message_THEN_it_does_as_many_times_as_systems_are_configured()
        {
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente, ESynchroSystem.VisionLocal }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(2).Save(aggregate);
        }

        [Fact]
        public void GIVEN_no_synchroSystem_is_configured_WHEN_a_message_is_listened_THEN_no_action_executes()
        {
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>()));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(aggregate);
        }

        [Fact]
        public void GIVEN_the_aggregate_cant_be_decrypted_WHEN_the_message_is_listened_THEN_it_emits_ProcesedMessageException_event()
        {
            var exceptionsEmitted = 0;
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente, ESynchroSystem.VisionLocal }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(x => { throw new ArgumentNullException("The message is invalid"); });
            dependencies.HostServiceEvents.ProcesedMessageExceptionSequence.Subscribe(m =>
            {
                Assert.Contains("The message is invalid", m.Message);
                Assert.Equal(m.EventID, message.@event.EventID);
                Assert.Equal(m.MessageHandler, "AuditLogonMessageHandler");
                Assert.Equal(m.MessageType, message.@event.MessageType);
                Assert.Equal(m.InnerException.GetType(), typeof(ArgumentNullException));
                exceptionsEmitted++;
            });
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            Assert.Equal(exceptionsEmitted, 1);
        }
        [Fact]
        public void GIVEN_the_repository_throws_an_exception_WHEN_the_message_is_listened_THEN_it_emits_ProcesedMessageException_event()
        {
            var exceptionsEmitted = 0;
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente, ESynchroSystem.VisionLocal }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.Repository.Save(Arg.Any<AuditLogonAggregate>()).Returns(s =>
            {
                throw new InvalidOperationException("No valid");
            });
            dependencies.HostServiceEvents.ProcesedMessageExceptionSequence.Subscribe(m =>
            {
                Assert.Contains("No valid", m.Message);
                Assert.Equal(m.EventID, message.@event.EventID);
                Assert.Equal(m.MessageHandler, "AuditLogonMessageHandler");
                Assert.Equal(m.MessageType, message.@event.MessageType);
                Assert.Equal(m.InnerException.GetType(), typeof(InvalidOperationException));
                exceptionsEmitted++;
            });
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);


            dependencies.HostServiceEvents.AddIncommingEvent(message);
            Assert.Equal(exceptionsEmitted, 1);
        }

        [Fact]
        public void GIVEN_a_message_WHEN_it_is_processed_successfully_THEN_emits_a_ProcesedMessage_event()
        {
            var processedMessages = 0;
            var dependencies = new MessageHandlerDependenciesHolder<AuditLogonAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("AuditLogonMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<AuditLogonAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new AuditLogonMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredAuditLogon");
            dependencies.HostServiceEvents.ProcesedMessageEventSequence.Subscribe(m =>
            {
                Assert.Equal(m.EventID, message.@event.EventID);
                Assert.Equal(m.MessageHandler, "AuditLogonMessageHandler");
                Assert.Equal(m.MessageType, message.@event.MessageType);
                Assert.Equal(m.Topic, message.@event.Topic);
                Assert.Null(m.Aggregate);
                Assert.False(m.InvalidReason.HasValue);
                Assert.Null(m.MessageOriginator);
                processedMessages++;
            });
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            Assert.Equal(processedMessages, 1);
        }

        AuditLogonAggregate GenerateRandomAggregate()
        {
            return new AuditLogonAggregate
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}
