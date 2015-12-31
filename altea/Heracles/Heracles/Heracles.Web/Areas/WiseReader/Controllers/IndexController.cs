using System.Web.Mvc;
using Heracles.Web.ActionFilters;
using System.Threading.Tasks;
using AlteaLabs.WiseReader.Contracts;
using AlteaLabs.WiseReader.Persistence;

namespace Heracles.Web.Areas.WiseReader.Controllers
{
    

    [AlteaAuth(Modules = "WiseReader")]
    public class IndexController : AlteaController
    {
        readonly  IFolderRepository _repository;
        public IndexController()
        {
            var dbContext = new FolderDataContext(AppConfiguration.Configuration.Value.AlteaDataConnectionString);
            _repository = new FolderRepository(dbContext);
        }
        // GET: /WiseReader
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            await _repository.CreateDefaultFolder(this.AlteaUser.Id);

            return this.View("Index");
        }
    }
}