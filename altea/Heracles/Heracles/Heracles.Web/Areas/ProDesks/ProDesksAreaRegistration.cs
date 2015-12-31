namespace Heracles.Web.Areas.ProDesks
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class ProDesksAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ProDesks";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "ProDesks_list",
                url: "PEC/List",
                defaults: new { controller = "Index", action = "List" });

            context.MapRoute(
                name: "ProDesks_assignments",
                url: "PEC/Assignments",
                defaults: new { controller = "Index", action = "Assignments" });


            context.MapRoute(
                name: "ProDesks_default",
                url: "PEC/{id}/",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "ProDesks_level",
                url: "PEC/{id}",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "ProDesks_sublevel",
                url: "PEC/{id}/{subId}",
                defaults: new { controller = "Index", action = "Index" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/ProDesks/Content/js/index").Include(
                    "~/Areas/ProDesks/Content/js/index.js"));
        }
    }
}