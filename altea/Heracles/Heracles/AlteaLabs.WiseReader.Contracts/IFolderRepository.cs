using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlteaLabs.WiseReader.Contracts
{

    public interface IFolderRepository
    {
        Task CreateDefaultFolder(Guid userId);

        Task ChangeFolderName(Guid folderId, string newName);

        Task<Guid> CreateFolder(Guid userId, Guid parentFolderId, string parentFolderIdname);

        Task DeleteFolder(Guid folderId);

        Task<IEnumerable<BoardFile>> GetFolderFiles(Guid userId, Guid folderId);

        Task<IEnumerable<Folder>> GetAllFolders(Guid userId);
    }
}
