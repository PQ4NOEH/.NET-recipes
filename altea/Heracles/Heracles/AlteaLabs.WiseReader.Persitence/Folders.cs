using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlteaLabs.WiseReader.Persistence
{
    public partial class Folders
    {
        public Folders()
        {
            WISEREADER_Files = new HashSet<Files>();
            WISEREADER_Folders1 = new HashSet<Folders>();
        }

        public Guid id { get; set; }

        public Guid user { get; set; }

        public Guid? parent { get; set; }

        [Required]
        [StringLength(64)]
        public string name { get; set; }

        [Required]
        [StringLength(64)]
        public string lowered_name { get; set; }

        public int position { get; set; }

        public virtual ICollection<Files> WISEREADER_Files { get; set; }

        public virtual ICollection<Folders> WISEREADER_Folders1 { get; set; }

        public virtual Folders WISEREADER_Folders2 { get; set; }
    }
}
