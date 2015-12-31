namespace Heracles.Web.Areas.Admin
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Admin_index",
                url: "Admin/",
                defaults: new { action = "Index", controller = "Index" });

            context.MapRoute(
                name: "Admin_action",
                url: "Admin/{action}",
                defaults: new { controller = "Index" });

            context.MapRoute(
                name: "Admin_controller",
                url: "Admin/{controller}/{action}",
                defaults: new { action = "Index" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Admin/Content/js/admin").Include(
                    "~/Areas/Admin/Content/js/adduser.js",
                    "~/Areas/Admin/Content/js/edituser.js",
                    "~/Areas/Admin/Content/js/addgroup.js",
                    "~/Areas/Admin/Content/js/editgroup.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Admin/Content/js/adduser").Include(
                    "~/Areas/Admin/Content/js/adduser.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Admin/Content/js/edituser").Include(
                    "~/Areas/Admin/Content/js/edituser.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Admin/Content/js/addgroup").Include(
                    "~/Areas/Admin/Content/js/addgroup.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Admin/Content/js/editgroup").Include(
                    "~/Areas/Admin/Content/js/editgroup.js"));
        }
    }
}