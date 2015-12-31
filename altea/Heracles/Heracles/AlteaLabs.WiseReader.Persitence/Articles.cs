namespace AlteaLabs.WiseReader.Persistence
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Articles
    {
        [Key]
        [Column(Order = 0)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid file { get; set; }

        public virtual Files WISEREADER_Files { get; set; }
    }
}
