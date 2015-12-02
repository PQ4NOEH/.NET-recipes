
using Davalor.Base.Messaging.Contracts;
using System;
namespace Davalor.VisionLocal.Messages.AuditLogon
{
    public class RegisteredAuditLogon : BaseEvent
    {
        public RegisteredAuditLogon()
        {
            Topic = "AuditLogon";
        }
    }
}
