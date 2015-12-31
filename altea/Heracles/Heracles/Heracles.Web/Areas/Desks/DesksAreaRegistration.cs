namespace Heracles.Web.Areas.Desks
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class DesksAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Desks";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Desks_index_list",
                url: "Index/List",
                defaults: new { controller = "Index", action = "List" });

            context.MapRoute(
                name: "Desks_index_assignments",
                url: "Index/Assignments",
                defaults: new { controller = "Index", action = "Assignments" });

            context.MapRoute(
                name: "Desks_index_part",
                url: "Index/Part/{level}/{type}/{area}/{subject}",
                defaults: new { controller = "Index", action = "Part" });

            context.MapRoute(
                name: "Desks_index_load",
                url: "Index/Load",
                defaults: new { controller = "Index", action = "Get" });

            context.MapRoute(
                name: "Desks_index_start",
                url: "Index/Start",
                defaults: new { controller = "Index", action = "Start" });

            context.MapRoute(
                name: "Desks_index_check",
                url: "Index/Check",
                defaults: new { controller = "Index", action = "Check" });

            context.MapRoute(
                name: "Desks_index_finish",
                url: "Index/Finish",
                defaults: new { controller = "Index", action = "Finish" });

            context.MapRoute(
                name: "Desks_index_report",
                url: "Index/Report",
                defaults: new { controller = "Index", action = "Report" });

            context.MapRoute(
                name: "Desks_index",
                url: "Index/",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "Desks_index_level",
                url: "Index/{id}",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "Desks_exams_list",
                url: "Exams/List",
                defaults: new { controller = "Exams", action = "List" });

            context.MapRoute(
                name: "Desks_exams_assignments",
                url: "Exams/Assignments",
                defaults: new { controller = "Exams", action = "Assignments" });

            context.MapRoute(
                name: "Desks_exams_part",
                url: "Exams/Part/{level}/{part}/{vocabulary}",
                defaults: new { controller = "Index", action = "Part" });

            context.MapRoute(
                name: "Desks_exams_load",
                url: "Exams/Load",
                defaults: new { controller = "Exams", action = "Get" });

            context.MapRoute(
                name: "Desks_exams_start",
                url: "Exams/Start",
                defaults: new { controller = "Exams", action = "Start" });

            context.MapRoute(
                name: "Desks_exams_check",
                url: "Exams/Check",
                defaults: new { controller = "Exams", action = "Check" });

            context.MapRoute(
                name: "Desks_exams_finish",
                url: "Exams/Finish",
                defaults: new { controller = "Exams", action = "Finish" });

            context.MapRoute(
                name: "Desks_exams_report",
                url: "Exams/Report",
                defaults: new { controller = "Exams", action = "Report" });

            context.MapRoute(
                name: "Desks_exams",
                url: "Exams/",
                defaults: new { controller = "Exams", action = "Index" });

            context.MapRoute(
                name: "Desks_exams_level",
                url: "Exams/{id}",
                defaults: new { controller = "Exams", action = "Index", });

            context.MapRoute(
                name: "Desks_cover",
                url: "Books/Cover/{publication}/{collection}",
                defaults: new { controller = "Books", action = "Cover" });

            context.MapRoute(
                name: "Desks_books",
                url: "Books/",
                defaults: new { controller = "Books", action = "Index" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/index").Include(
                    "~/Areas/Desks/Content/js/index.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/exams").Include(
                    "~/Areas/Desks/Content/js/exams.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/books").Include(
                    "~/Areas/Desks/Content/js/books.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/engine/choosefromthebox").Include(
                    "~/Areas/Desks/Content/js/engines/choosefromthebox.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/engine/multiplechoice").Include(
                    "~/Areas/Desks/Content/js/engines/multiplechoice.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/engine/gapfilling").Include(
                    "~/Areas/Desks/Content/js/engines/gapfilling.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/engine/wordformation").Include(
                    "~/Areas/Desks/Content/js/engines/wordformation.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/Desks/Content/js/engine/rephrasing").Include(
                    "~/Areas/Desks/Content/js/engines/rephrasing.js"));
        }
    }
}