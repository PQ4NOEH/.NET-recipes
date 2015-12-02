﻿using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using System;
using System.Collections.Generic;
using NSubstitute;
using Xunit;
using Davalor.SAP.Messages.Media;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;

namespace Davalor.SynchronizationManager.MessageHandlers.UnitTests
{
    public class MediaHandlerSpec
    {
        [Fact]
        public void GIVEN_no_System_is_configured_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<MediaAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("MediaMessageHandler", new List<ESynchroSystem>()));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<MediaAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new MediaMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredMedia");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<MediaAggregate>());
        }
        [Fact]
        public void GIVEN_null_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<MediaAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<MediaAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new MediaMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredMedia");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<MediaAggregate>());
        }
        [Fact]
        public void GIVEN_no_configuration_is_provided_WHEN_listened_a_message_THEN_it_does_nothing()
        {
            var dependencies = new MessageHandlerDependenciesHolder<MediaAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("MediaMessageHandler", null));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<MediaAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new MediaMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredMedia");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<MediaAggregate>());
        }
        [Fact]
        public void GIVEN_a_RegisteredMedia_message_WHEN_listened_THEN_the_aggregate_is_saved_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<MediaAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("MediaMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<MediaAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new MediaMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("RegisteredMedia");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Save(aggregate);
        }
        [Fact]
        public void GIVEN_a_ChangedMedia_message_WHEN_listened_THEN_the_aggregate_is_updated_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<MediaAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("MediaMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<MediaAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new MediaMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("ChangedMedia");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Update(aggregate);
        }
        [Fact]
        public void GIVEN_a_UnregisteredMedia_message_WHEN_listened_THEN_the_aggregate_is_removed_in_the_repository()
        {
            var dependencies = new MessageHandlerDependenciesHolder<MediaAggregate>();
            var aggregate = GenerateRandomAggregate();
            var handlersConfig = new List<MessageHandlerConfiguration>();
            handlersConfig.Add(new MessageHandlerConfigurationFake("MediaMessageHandler", new List<ESynchroSystem>() { ESynchroSystem.PortalPaciente }));
            dependencies.HostConfiguration.MessagesHandlers.Returns(handlersConfig);
            dependencies.MessageDecrypter.Decrypt<MediaAggregate>(Arg.Any<NotNullable<BaseEvent>>()).Returns(aggregate);
            var sut = new MediaMessageHandler(
                                dependencies.RepositoryFactory,
                                dependencies.MessageDecrypter,
                                dependencies.HostConfiguration,
                                dependencies.HostServiceEvents);

            var message = TestsHelpers.GenerateRandomMessage("UnregisteredMedia");
            dependencies.HostServiceEvents.AddIncommingEvent(message);
            dependencies.Repository.Received(1).Delete(aggregate.Id);
            dependencies.Repository.DidNotReceive().Save(Arg.Any<MediaAggregate>());
        }
        MediaAggregate GenerateRandomAggregate()
        {
            return new MediaAggregate
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}