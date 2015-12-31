namespace AlteaLabs.WiseReader.Persistence
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class FileTypes
    {
        public FileTypes()
        {
            WISEREADER_StorageFiles = new HashSet<StorageFiles>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(64)]
        public string name { get; set; }

        [Required]
        [StringLength(64)]
        public string lowered_name { get; set; }

        public virtual ICollection<StorageFiles> WISEREADER_StorageFiles { get; set; }
    }
}
