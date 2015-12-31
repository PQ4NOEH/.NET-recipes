namespace Heracles.Web.Areas.WiseNet.Controllers
{
    using System.Web.Mvc;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "WiseNet")]
    public class ArticlesController : AlteaController
    {
        // POST: /WiseNet/Articles/Get
        [HttpPost]
        [OnlyAjax]
        public ActionResult Get(string uri)
        {
            int reference = WiseNetService.GetArticle(this.AlteaUser.Id, this.AlteaUser.From, uri);
            return this.JsonNet(reference);
        }

        // POST: /WiseNet/Articles/Create
        [HttpPost]
        [OnlyAjax]
        public ActionResult Create(string uri, int offsetDate)
        {
            int reference = WiseNetService.CreateArticle(this.AlteaUser.Id, this.AlteaUser.From, uri, offsetDate);
            return this.JsonNet(reference);
        }
    }
}