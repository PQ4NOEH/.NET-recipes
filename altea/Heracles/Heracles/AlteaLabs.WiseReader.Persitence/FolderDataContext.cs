namespace AlteaLabs.WiseReader.Persistence
{
    using System.Data.Entity;

    public partial class FolderDataContext : DbContext
    {
        public FolderDataContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<FolderDataContext>(null);
        }

        public virtual DbSet<Articles> WISEREADER_Articles { get; set; }
        public virtual DbSet<Files> WISEREADER_Files { get; set; }
        public virtual DbSet<FileTypes> WISEREADER_FileTypes { get; set; }
        public virtual DbSet<Folders> WISEREADER_Folders { get; set; }
        public virtual DbSet<StorageFiles> WISEREADER_StorageFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Files>().ToTable("WISEREADER_Files");
            modelBuilder.Entity<Articles>().ToTable("WISEREADER_Articles");
            modelBuilder.Entity<FileTypes>().ToTable("WISEREADER_FileTypes");
            modelBuilder.Entity<Folders>().ToTable("WISEREADER_Folders");
            modelBuilder.Entity<StorageFiles>().ToTable("WISEREADER_StorageFiles");

            modelBuilder.Entity<Files>()
                .Property(e => e.create_date)
                .HasPrecision(2);

            modelBuilder.Entity<Files>()
                .Property(e => e.last_modified_date)
                .HasPrecision(2);

            modelBuilder.Entity<Files>()
                .HasMany(e => e.WISEREADER_Articles)
                .WithRequired(e => e.WISEREADER_Files)
                .HasForeignKey(e => e.file)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<FileTypes>()
                .HasMany(e => e.WISEREADER_StorageFiles)
                .WithRequired(e => e.WISEREADER_FileTypes)
                .HasForeignKey(e => e.file_type)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Folders>()
                .HasMany(e => e.WISEREADER_Files)
                .WithRequired(e => e.WISEREADER_Folders)
                .HasForeignKey(e => e.folder)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Folders>()
                .HasMany(e => e.WISEREADER_Folders1)
                .WithOptional(e => e.WISEREADER_Folders2)
                .HasForeignKey(e => e.parent);

            modelBuilder.Entity<StorageFiles>()
                .Property(e => e.checksum)
                .IsFixedLength();

            modelBuilder.Entity<StorageFiles>()
                .Property(e => e.upload_date)
                .HasPrecision(2);

            modelBuilder.Entity<StorageFiles>()
                .Property(e => e.delete_date)
                .HasPrecision(2);

            modelBuilder.Entity<StorageFiles>()
                .Property(e => e.process_date)
                .HasPrecision(2);

            modelBuilder.Entity<StorageFiles>()
                .Property(e => e.invalid_date)
                .HasPrecision(2);

            modelBuilder.Entity<StorageFiles>()
                .HasMany(e => e.WISEREADER_Files)
                .WithRequired(e => e.WISEREADER_StorageFiles)
                .HasForeignKey(e => e.file)
                .WillCascadeOnDelete(false);
        }
    }
}
