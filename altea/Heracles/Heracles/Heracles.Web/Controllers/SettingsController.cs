namespace Heracles.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    using Heracles.Web.ActionFilters;

    public class SettingsController : AlteaController
    {
        [HttpGet, AlteaAuth(Roles = "Developer", Modules = "Settings")]
        public ActionResult Index()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [AlteaAuth(Roles = "Developer", Modules = "Clear Settings")]
        public ActionResult Clear(IEnumerable<AlteaSettingsType> settings)
        {
            foreach (
                MethodInfo method in
                    settings.Where(setting => Enum.IsDefined(typeof(AlteaSettingsType), setting))
                        .Select(
                            setting =>
                            typeof(AppCore).GetMethod("Clear" + setting, BindingFlags.Public | BindingFlags.Static))
                        .Where(method => method != null))
            {
                method.Invoke(null, null);
            }

            return new EmptyResult();
        }
    }
}