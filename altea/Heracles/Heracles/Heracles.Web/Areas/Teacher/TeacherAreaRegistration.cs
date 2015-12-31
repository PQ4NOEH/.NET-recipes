namespace Heracles.Web.Areas.Teacher
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class TeacherAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Teacher";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Teacher_level_data",
                url: "Teacher/GetLevel",
                defaults: new { controller = "Index", action = "GetLevel" });

            context.MapRoute(
                name: "Teacher_group_data",
                url: "Teacher/GetGroup",
                defaults: new { controller = "Index", action = "GetGroup" });

            context.MapRoute(
                name: "Teacher_index",
                url: "Teacher/",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "Teacher_level",
                url: "Teacher/{level}",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "Teacher_group",
                url: "Teacher/{level}/{group}",
                defaults: new { controller = "Index", action = "Index" });

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Teacher/Content/js/index").Include(
                    "~/Areas/Teacher/Content/js/index.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Teacher/Content/js/chronometer").Include(
                    "~/Areas/Teacher/Content/js/chronometer.js"));
        }
    }
}