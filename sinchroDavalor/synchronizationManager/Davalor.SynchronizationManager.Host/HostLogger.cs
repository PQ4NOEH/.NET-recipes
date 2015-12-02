using Davalor.Base.Library.Guards;
using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Events;
using System;

namespace Davalor.SynchronizationManager.Host
{
    public class HostLogger
    {

        readonly SynchronizationManagerEventTracing _eventTracing;
        readonly IHostConfiguration _configuration;
        public HostLogger( 
            NotNullable<IServiceEvents> serviceEvents, 
            NotNullable<SynchronizationManagerEventTracing> eventTracing, 
            NotNullable<IHostConfiguration> configuration)
        {
            _eventTracing = eventTracing;
            serviceEvents.Value.IncommingEventSequence.Subscribe(LogIncomeMessage);
            serviceEvents.Value.ReceivedMessageEventSequence.Subscribe(LogReceivedMessage);
            serviceEvents.Value.ProcesedMessageEventSequence.Subscribe(LogProcessedMessage);
            serviceEvents.Value.ProcesedMessageExceptionSequence.Subscribe(LogProcessedMessageException);

        }

        public void LogAppStarted()
        {
            _eventTracing.Service_started(
                _configuration.InstanceName,
                _configuration.MachineName
                );
        }

        public void LogAppStopped()
        {
            _eventTracing.Service_stopped(
                _configuration.InstanceName,
                _configuration.MachineName
            );
        }

        void LogIncomeMessage(IncommingEvent message)
        {
            _eventTracing.Message_received(
                _configuration.InstanceName, 
                _configuration.MachineName, 
                message.@event.MessageType, 
                message.@event.EventID.ToString(), 
                string.Empty);
        }

        void LogReceivedMessage(ReceivedMessage message)
        {
            _eventTracing.Message_received(
                _configuration.InstanceName, 
                _configuration.MachineName, 
                message.MessageType, 
                message.EventID.ToString(), 
                message.MessageHandler
            );
        }

        void LogProcessedMessage(ReceivedMessage message)
        {
            _eventTracing.Message_Processed(
                _configuration.InstanceName, 
                _configuration.MachineName, 
                message.MessageType, 
                message.EventID.ToString(), 
                message.MessageHandler
            );
        }

        void LogProcessedMessageException(ProcesedMessageException exception)
        {

            Exception ex = exception;
            while(ex.InnerException != null) 
            {
                ex = ex.InnerException;
            }
            _eventTracing.Message_processing_Exception(
                _configuration.InstanceName, 
                _configuration.MachineName, 
                exception.MessageType, 
                exception.EventID.ToString(), 
                exception.MessageHandler,
                ex.Message,
                ex.StackTrace
            );
        }
    }
}
