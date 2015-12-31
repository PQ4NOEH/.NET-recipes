using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AlteaLabs.Common.Guards;
using AlteaLabs.WiseReader.Persistence;
using AlteaLabs.Common.Contracts;
using AlteaLabs.WiseReader.Contracts;
using System;
using System.Collections.Generic;

namespace AlteaLabs.WiseReader.Persistence
{

   

    public class FolderRepository : IFolderRepository
    {
        private readonly FolderDataContext _db;
        public FolderRepository(NotNullable<FolderDataContext> dbContext )
        {
            _db = dbContext;
        }

        public async Task CreateDefaultFolder(Guid userId)
        {
            var folder = await _db.Set<Folders>().FirstAsync(f => f.user == userId);
            if (folder == null)
            {
                _db.Set<Folders>()
                    .Add(
                        new Folders
                            {
                                id = Guid.NewGuid(),
                                user = userId,
                                parent = null,
                                name = "WiseReader",
                                lowered_name = "wisereader",
                                position = 0
                            });
                await _db.SaveChangesAsync();
            }
        }

        public async Task ChangeFolderName(Guid folderId, string newName)
        {
            var folder = await _db.Set<Folders>().FindAsync(folderId);
            if (folder != null)
            {
                folder.name = newName;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Guid> CreateFolder(Guid userId, Guid parentFolderId, string parentFolderIdname)
        {
            var parentFolder = await _db.Set<Folders>().FindAsync(parentFolderId);
            if (parentFolder == null)
            {
                throw new ArgumentOutOfRangeException("Parent folder does not exist");
            }
            var newFolderId = Guid.NewGuid();
            _db.Set<Folders>()
                .Add(
                    new Folders
                        {
                            id = newFolderId,
                            user = userId,
                            parent = parentFolderId,
                            name = parentFolderIdname,
                            lowered_name = parentFolderIdname.ToLowerInvariant(),
                            position = parentFolder.position + 1
                        });
            await _db.SaveChangesAsync();
            return newFolderId;
        }

        public async Task DeleteFolder( Guid folderId)
        {
            var folder = await _db.Set<Folders>().FindAsync(folderId);
            if (folder != null)
            {
                _db.Set<Folders>().Remove(folder);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BoardFile>> GetFolderFiles(Guid userId, Guid folderId)
        {
            IEnumerable<BoardFile> result = new List<BoardFile>();

            var folder = await _db.Set<Folders>().FindAsync(folderId);
            if (folder != null)
            {
                result = folder.WISEREADER_Files.Select(file => new BoardFile
                                                                    {
                                                                        Id = file.id,
                                                                        Type = (FileType)file.WISEREADER_StorageFiles.file_type,
                                                                        Uploaded = true,
                                                                        Language = file.language.HasValue 
                                                                                        ? (Language)file.language.Value 
                                                                                        : Language.English,
                                                                        Name = file.name,
                                                                        CreateDate = file.create_date,
                                                                        LastModifiedDate = file.last_modified_date,
                                                                        Processed = file.WISEREADER_StorageFiles.processed,
                                                                        Converted = file.WISEREADER_StorageFiles.converted,
                                                                        Opened = file.opened,
                                                                        Invalid = file.WISEREADER_StorageFiles.invalid
                                                                    });
            }
            return result;
        }

        public async Task<IEnumerable<Folder>> GetAllFolders(Guid userId)
        {
            var result =  await _db.Set<Folders>()
                            .AsQueryable()
                            .Where(f => f.user == userId)
                            .ToListAsync();
            return result.Select( f => new Folder
                                           {
                                               Id = f.id,
                                               Name = f.name,
                                               Position = f.position,
                                               Parent = f.parent
                                           });
        }
    }
}
