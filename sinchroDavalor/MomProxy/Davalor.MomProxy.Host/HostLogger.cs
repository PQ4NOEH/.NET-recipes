using Davalor.Base.Contract.Library;
using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;
using Davalor.MomProxy.Domain;
using Davalor.MomProxy.Host.Configuration;
using System;

namespace Davalor.MomProxy.Host
{
    public class HostLogger
    {
        public HostLogger(NotNullable<IStringSerializer> serializer, NotNullable<IServiceEvents> serviceEvents)
        {
            serviceEvents.Value.SavedIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
            {
                string messageId = serializer.Value.Deserialize<BaseEvent>(s).EventID.ToString();
                MomProxyEventTracing.Log.Value.Message_saved_in_local_storage(
                    HostConfiguration.Instance.Value.ApplicationName, 
                    HostConfiguration.Instance.Value.MachineName,
                    messageId);
            }));
            serviceEvents.Value.SentIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
            {
                string messageId = serializer.Value.Deserialize<BaseEvent>(s).EventID.ToString();
                MomProxyEventTracing.Log.Value.Message_send_to_MOM(
                    HostConfiguration.Instance.Value.ApplicationName,
                    HostConfiguration.Instance.Value.MachineName,
                    messageId);
            }));
            serviceEvents.Value.DeletedIncommingMessageSequence.Subscribe(new Action<NotNullOrWhiteSpaceString>(s =>
            {
                string messageId = serializer.Value.Deserialize<BaseEvent>(s).EventID.ToString();
                MomProxyEventTracing.Log.Value.Message_deleted_from_local_storage(
                    HostConfiguration.Instance.Value.ApplicationName,
                    HostConfiguration.Instance.Value.MachineName,
                    messageId);
            }));

            serviceEvents.Value.ReceivedMessageSequence.Subscribe(new Action<NotNullable<BaseEvent>>(@event =>
            {
                MomProxyEventTracing.Log.Value.Message_received(
                        HostConfiguration.Instance.Value.ApplicationName,
                        HostConfiguration.Instance.Value.MachineName,
                        @event.Value.Topic,
                        @event.Value.EventID.ToString(),
                        HostConfiguration.Instance.Value.ApplicationName
                        );
            }));
            serviceEvents.Value.ReceivedInvalidMessageSequence.Subscribe(new Action<NotNullable<BaseEvent>>((message) =>
            {
                MomProxyEventTracing.Log.Value.Invalid_Message_received(
                        HostConfiguration.Instance.Value.ApplicationName,
                        HostConfiguration.Instance.Value.MachineName,
                        message.Value.Topic ?? string.Empty,
                        message.Value.EventID.ToString(),
                        Enum.GetName(typeof(EInvalidMessageReason), message.Value.InvalidReason.Value)
                        );
            }));
        }
    }
}
