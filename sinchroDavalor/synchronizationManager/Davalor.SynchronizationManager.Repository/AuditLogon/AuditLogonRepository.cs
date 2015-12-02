using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.VisionLocal.Messages.AuditLogon;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Davalor.SynchronizationManager.Repository.AuditLogon
{
    public class AuditLogonRepository : GenericDataService<AuditLogonAggregate>
    {
        public AuditLogonRepository(DbContext context)
            : base(context) 
        { }

        
    }
}
