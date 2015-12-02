
using Davalor.Base.Messaging.Contracts;
using System;
namespace Davalor.VisionLocal.Messages.Session
{
    public class FinisedEvaSession : BaseEvent 
    {
        public FinisedEvaSession()
        {
            Topic = "EvaSession";
        }
    }
}
