namespace Heracles.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Extensions;

    using Heracles.Models.Admin;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Admin")]
    public class IndexController : AlteaController
    {
        // GET: /Admin
        [HttpGet]
        public ActionResult Index()
        {
            return this.View(this.CreateModel());
        }

        private AdminModel CreateModel()
        {
            AdminModel model = new AdminModel
                {
                    Roles = RoleProvider.GetAllRoles(AppCore.AppId),
                    Modules = RoleProvider.GetAllModules(AppCore.AppId),
                    Languages =
                        AppService.GetLanguages(AppCore.AppId).Select(
                            x =>
                            new KeyValuePair<int, IEnumerable<int>>(
                                x.Key.GetDatabaseId(),
                                x.Value.Select(y => y.GetDatabaseId())))
                        .ToDictionary(x => x.Key, x => x.Value)
                };

            return model;
        }
    }
}