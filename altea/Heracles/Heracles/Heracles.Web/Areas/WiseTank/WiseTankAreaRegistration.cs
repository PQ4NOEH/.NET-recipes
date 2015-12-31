namespace Heracles.Web.Areas.WiseTank
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class WiseTankAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WiseTank";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "WiseTank_index",
                url: "WiseTank/",
                defaults: new { action = "Index", controller = "Index" });

            context.MapRoute(
                name: "WiseTank_action",
                url: "WiseTank/{action}",
                defaults: new { controller = "Index" });

            context.MapRoute(
                name: "WiseTank_controller",
                url: "WiseTank/{controller}/{action}",
                defaults: new { action = "Index" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/WiseTank/Content/js/wisetank").Include(
                    "~/Areas/WiseTank/Content/js/wisetank.js"));
        }
    }
}