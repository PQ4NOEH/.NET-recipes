namespace Heracles.Web.Areas.WiseNet
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class WiseNetAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WiseNet";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "WiseNet_browser",
                url: "WiseNet/",
                defaults: new { action = "Index", controller = "Browser" });

            context.MapRoute(
                name: "WiseNet_proxy",
                url: "WiseNet/Load/{parser}",
                defaults: new { action = "Load", controller = "Browser" });

            context.MapRoute(
                name: "WiseNet_error",
                url: "WiseNet/Error",
                defaults: new { action = "ReportError", controller = "Browser" });

            context.MapRoute(
                name: "WiseNet_articles",
                url: "WiseNet/Articles/{action}",
                defaults: new { controller = "Articles" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/WiseNet/Content/js/wisenet").Include(
                    "~/Areas/WiseNet/Content/js/wisenet.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/WiseNet/Content/js/home").Include(
                    "~/Areas/WiseNet/Content/js/home.js"));
        }
    }
}