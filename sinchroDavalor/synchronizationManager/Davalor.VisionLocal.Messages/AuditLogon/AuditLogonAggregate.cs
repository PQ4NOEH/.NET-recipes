using Davalor.SynchronizationManager.Domain.Repository;
using System;

namespace Davalor.VisionLocal.Messages.AuditLogon
{
    public class AuditLogonAggregate : ISynchroAggregateRoot
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
        public string Access { get; set; }
        public DateTimeOffset AccessDate { get; set; }
        public Guid PartnerId { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
