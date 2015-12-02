using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;
using Davalor.MomProxy.Domain;
using System;
using System.Reactive.Subjects;

namespace Davalor.MomProxy
{
    public class ServiceEvents : IServiceEvents
    {
        public static Lazy<ServiceEvents> Instance = new Lazy<ServiceEvents>(() => new ServiceEvents());

        #region incomming
        readonly Subject<NotNullOrWhiteSpaceString> _savedIncommingMessageSequence = new Subject<NotNullOrWhiteSpaceString>();
        public IObservable<NotNullOrWhiteSpaceString> SavedIncommingMessageSequence
        {
            get
            {
                return _savedIncommingMessageSequence;
            }
        }

        public void SavedIncommingMessage(NotNullOrWhiteSpaceString incommingMessage)
        {
            _savedIncommingMessageSequence.OnNext(incommingMessage);
        }
        #endregion incomming

        #region Sent
        readonly Subject<NotNullOrWhiteSpaceString> _sentIncommingMessageSequence = new Subject<NotNullOrWhiteSpaceString>();
        public IObservable<NotNullOrWhiteSpaceString> SentIncommingMessageSequence
        {
            get
            {
                return _sentIncommingMessageSequence;
            }
        }
        public void SentIncommingMessage( NotNullOrWhiteSpaceString incommingMessage)
        {
            _sentIncommingMessageSequence.OnNext(incommingMessage);
        }
        #endregion Sent

        #region  DeletedIncommingMessage
        readonly Subject<NotNullOrWhiteSpaceString> _deletedIncommingMessageSequence = new Subject<NotNullOrWhiteSpaceString>();
        public void DeletedIncommingMessage(NotNullOrWhiteSpaceString message)
        {
            _deletedIncommingMessageSequence.OnNext(message);
        }
        public IObservable<NotNullOrWhiteSpaceString> DeletedIncommingMessageSequence
        {
            get
            {
                return _deletedIncommingMessageSequence;
            }
        }
        #endregion

        #region  ReceivedMessageSequence
        readonly Subject<NotNullable<BaseEvent>> _receivedMessageSequence = new Subject<NotNullable<BaseEvent>>();
        public void ReceivedMessage(NotNullable<BaseEvent> message)
        {
            _receivedMessageSequence.OnNext(message);
        }
        public IObservable<NotNullable<BaseEvent>> ReceivedMessageSequence
        {
            get
            {
                return _receivedMessageSequence;
            }
        }
        #endregion

        #region  ReceivedInvalidMessage
        readonly Subject<NotNullable<BaseEvent>> _receivedInvalidMessageSequence = new Subject<NotNullable<BaseEvent>>();
        public void ReceivedInvalidMessage(NotNullable<BaseEvent> message)
        {
            _receivedInvalidMessageSequence.OnNext(message);
        }
        public IObservable<NotNullable<BaseEvent>> ReceivedInvalidMessageSequence
        {
            get
            {
                return _receivedInvalidMessageSequence;
            }
        }
        #endregion
    }
}
