using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlteaLabs.WiseReader.Persistence
{

    public partial class StorageFiles
    {
        public StorageFiles()
        {
            WISEREADER_Files = new HashSet<Files>();
        }

        public Guid id { get; set; }

        public bool uploaded { get; set; }

        [Required]
        [StringLength(32)]
        public string name { get; set; }

        public int? language { get; set; }

        [Required]
        [MaxLength(20)]
        public byte[] checksum { get; set; }

        public int file_type { get; set; }

        public Guid uploaded_by { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime upload_date { get; set; }

        public bool exists { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? delete_date { get; set; }

        public bool processed { get; set; }

        public bool converted { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? process_date { get; set; }

        public bool invalid { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? invalid_date { get; set; }

        public virtual ICollection<Files> WISEREADER_Files { get; set; }

        public virtual FileTypes WISEREADER_FileTypes { get; set; }
    }
}
