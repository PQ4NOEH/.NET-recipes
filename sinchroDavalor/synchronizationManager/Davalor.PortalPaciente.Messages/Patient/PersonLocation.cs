
using System;

namespace Davalor.PortalPaciente.Messages.Patient
{
    public partial class PersonLocation
    {
        public Guid Id { get; set; }

        public Guid PersonId { get; set; }

        public Guid LocationId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
        
        public virtual Person Person { get; set; }
    }
}
