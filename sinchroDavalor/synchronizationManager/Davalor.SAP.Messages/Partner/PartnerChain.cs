using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Partner
{
    public partial class PartnerChain
    {
        public PartnerChain()
        {
            Partner = new HashSet<PartnerAggregate>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        [Required]
        [StringLength(30)]
        public string SapCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual ICollection<PartnerAggregate> Partner { get; set; }
    }
}
