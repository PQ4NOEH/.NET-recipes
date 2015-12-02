using Davalor.Base.Library.Guards;
using Davalor.Base.Messaging.Contracts;
using System;

namespace Davalor.MomProxy.Domain
{
    public interface IServiceEvents
    {
        void SavedIncommingMessage(NotNullOrWhiteSpaceString incommingMessage);
        IObservable<NotNullOrWhiteSpaceString> SavedIncommingMessageSequence { get; }
        void SentIncommingMessage(NotNullOrWhiteSpaceString message);
        IObservable<NotNullOrWhiteSpaceString> SentIncommingMessageSequence { get; }
        void DeletedIncommingMessage(NotNullOrWhiteSpaceString message);
        IObservable<NotNullOrWhiteSpaceString> DeletedIncommingMessageSequence { get; }

        void ReceivedMessage(NotNullable<BaseEvent> message);
        IObservable<NotNullable<BaseEvent>> ReceivedMessageSequence { get; }

        void ReceivedInvalidMessage(NotNullable<BaseEvent> message);
        IObservable<NotNullable<BaseEvent>> ReceivedInvalidMessageSequence { get; }
        

        

    }
}
