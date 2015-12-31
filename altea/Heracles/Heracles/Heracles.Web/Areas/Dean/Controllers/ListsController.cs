namespace Heracles.Web.Areas.Dean.Controllers
{
    using System.Web.Mvc;

    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean")]
    public class ListsController : AlteaController
    {
    }
}