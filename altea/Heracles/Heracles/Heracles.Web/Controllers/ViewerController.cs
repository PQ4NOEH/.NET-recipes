namespace Heracles.Web.Controllers
{
    using System.Web.Mvc;

    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Admin", Modules = "Viewer")]
    public class ViewerController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}