using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

//using Altea.Classes.WiseReader;

//using Heracles.Models.WiseReader;
using Heracles.Services;
using Heracles.Web.ActionFilters;
using AlteaLabs.WiseReader.Contracts;
using AlteaLabs.WiseReader.Persistence;
using System.Threading.Tasks;
using AlteaLabs.WiseReader.Models;

namespace Heracles.Web.Areas.WiseReader.Controllers
{
    using Altea.Extensions;

    using WebGrease.Css.Extensions;

    [AlteaAuth(Modules = "WiseReader")]
    public class FoldersController : AlteaController
    {
        private readonly IFolderRepository _repository;

        public FoldersController()
        {
            var dbContext = new FolderDataContext(AppConfiguration.Configuration.Value.AlteaDataConnectionString);
            _repository = new FolderRepository(dbContext);
        }


        private List<Folder> InsertChildren(Guid parentId, List<Folder> elements)
        {
            List<Folder> result = new List<Folder>();

            result.AddRange(elements.Where(f=> f.Parent == parentId));
            result.ForEach(f => f.Children = InsertChildren(f.Id, elements));
            return result;
        }

    // POST: /WiseReader/Get
        [OnlyAjax]
        [HttpPost]
        public async Task<ActionResult> Get()
        {
            var userFolders = await _repository.GetAllFolders(this.AlteaUser.Id);
            FoldersModel model = new FoldersModel
            {
                FolderIds = userFolders.Select(f=> f.Id).ToList(),
                RootFolder = userFolders.First(f => !f.Parent.HasValue)
            };
            model.RootFolder.Children = InsertChildren(model.RootFolder.Id, userFolders.ToList());

            return this.JsonNet(model);
        }

        // POST: /WiseReader/Files
        [OnlyAjax]
        [HttpPost]
        public ActionResult Files(Guid folder)
        {
            IEnumerable<Altea.Classes.WiseReader.BoardFile> files = WiseReaderService.GetFolderFiles(this.AlteaUser.Id, folder);

            return this.JsonNet(files);
        }

        // POST: /WiseReader/Create
        [OnlyAjax]
        [HttpPost]
        public ActionResult Create(Guid parent, string name)
        {
            name = name.Trim();

            Guid status = name.Length > 64
                ? Guid.Empty
                : WiseReaderService.CreateFolder(this.AlteaUser.Id, parent, name);

            return this.JsonNet(status);
        }

        // POST: /WiseReader/Edit
        [OnlyAjax]
        [HttpPost]
        public ActionResult Edit(Guid folder, string name)
        {
            name = name.Trim();

            int status = name.Length > 64
                ? -1
                : WiseReaderService.EditFolder(this.AlteaUser.Id, folder, name);

            return this.JsonNet(status);
        }

        // POST: /WiseReader/Move
        [OnlyAjax]
        [HttpPost]
        public ActionResult Move(Guid folder, Guid parent, int position)
        {
            throw new NotImplementedException();
        }

        // POST: /WiseReader/Delete
        [OnlyAjax]
        [HttpPost]
        public ActionResult Delete(Guid folder)
        {
            int status = WiseReaderService.DeleteFolder(this.AlteaUser.Id, folder);
            return this.JsonNet(status);
        }
    }
}