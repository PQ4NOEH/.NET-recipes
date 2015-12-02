using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.VisionLocal.Messages.AuditLogon
{
    public class AuditLogonAggregate
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
    }
}
