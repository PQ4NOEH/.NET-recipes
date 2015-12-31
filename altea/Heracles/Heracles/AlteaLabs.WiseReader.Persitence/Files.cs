using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlteaLabs.WiseReader.Persistence
{
    public partial class Files
    {
        public Files()
        {
            WISEREADER_Articles = new HashSet<Articles>();
        }

        public Guid id { get; set; }

        public Guid user { get; set; }

        public Guid folder { get; set; }

        public Guid file { get; set; }

        public bool uploaded { get; set; }

        public int? language { get; set; }

        [Required]
        [StringLength(260)]
        public string name { get; set; }

        [Required]
        [StringLength(260)]
        public string lowered_name { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime create_date { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime last_modified_date { get; set; }

        public bool opened { get; set; }

        public virtual ICollection<Articles> WISEREADER_Articles { get; set; }

        public virtual Folders WISEREADER_Folders { get; set; }

        public virtual StorageFiles WISEREADER_StorageFiles { get; set; }
    }
}
