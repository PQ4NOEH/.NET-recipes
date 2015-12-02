using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.PortalPaciente.Messages.DisclaimerSignature
{
    public class Signature
    {
        public Guid Id { get; set; }

        [StringLength(8)]
        public String SafekeepingIdentifier { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public ICollection<DisclaimerSignatureAggregate> DisclaimerSignature { get; set; }
    }
}
