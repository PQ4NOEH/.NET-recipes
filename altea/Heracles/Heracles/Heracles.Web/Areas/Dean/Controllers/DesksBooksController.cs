namespace Heracles.Web.Areas.Dean.Controllers
{
    using System;
    using System.Web.Mvc;

    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean")]
    public class DesksBooksController : AlteaController
    {
        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult Assign()
        {
            throw new NotImplementedException();
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:Group Dean")]
        public ActionResult GroupAssign()
        {
            throw new NotImplementedException();
        }
    }
}