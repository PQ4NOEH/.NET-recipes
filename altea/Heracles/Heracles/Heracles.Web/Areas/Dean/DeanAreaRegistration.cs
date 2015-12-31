namespace Heracles.Web.Areas.Dean
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class DeanAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Dean";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Dean_index",
                url: "Dean/",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "Dean_member",
                url: "Dean/{id}/{type}/{pro}/{level}/{sublevel}",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "Dean_action",
                url: "Dean/{action}",
                defaults: new { controller = "Index" });

            context.MapRoute(
                name: "Dean_controller",
                url: "Dean/{controller}/{action}",
                defaults: new { action = "Index" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Dean/Content/js/dean").Include(
                    "~/Areas/Dean/Content/js/index.js",
                    "~/Areas/Dean/Content/js/groups.js",
                    "~/Areas/Dean/Content/js/groupplanner.js",
                    "~/Areas/Dean/Content/js/desksindex.js",
                    "~/Areas/Dean/Content/js/desksexams.js",
                    "~/Areas/Dean/Content/js/desksextra.js",
                    "~/Areas/Dean/Content/js/desksbooks.js",
                    "~/Areas/Dean/Content/js/prodesks.js",
                    "~/Areas/Dean/Content/js/wisenet.js",
                    "~/Areas/Dean/Content/js/lists.js"));
        }
    }
}