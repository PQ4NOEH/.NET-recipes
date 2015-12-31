
namespace Heracles.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Heracles.Web.ActionFilters;

    public class CacheController : AlteaController
    {
        [HttpGet, AlteaAuth(Roles = "Developer", Modules = "Cache")]
        public ActionResult Index()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AlteaAuth(Roles = "Developer", Modules = "Clear Cache")]
        public ActionResult Clear(IDictionary<string, bool> cacheTypes)
        {
            foreach (
                AlteaCache.Scope scope in
                    cacheTypes.Where(x => x.Value)
                        .Select(x => (AlteaCache.Scope)Enum.Parse(typeof(AlteaCache.Scope), x.Key)))
            {
                AlteaCache.RemoveAllKeys(scope);
            }

            return new EmptyResult();
        }
    }
}