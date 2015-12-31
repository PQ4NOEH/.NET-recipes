namespace Heracles.Web.Areas.WiseReader
{
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class WiseReaderAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WiseReader";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "WiseReader_index",
                url: "WiseReader/",
                defaults: new { controller = "Index", action = "Index" });

            context.MapRoute(
                name: "WiseReader_upload",
                url: "WiseReader/Upload/{folder}",
                defaults: new { controller = "Files", action = "Upload" });

            context.MapRoute(
                name: "WiseReader_texteditor",
                url: "WiseReader/TextEditor/{id}",
                defaults: new { controller = "Files", action = "TextEditor", id = UrlParameter.Optional });

            context.MapRoute(
                name: "WiseReader_view",
                url: "WiseReader/View/{id}",
                defaults: new { controller = "Files", action = "View" });

            context.MapRoute(
                name: "WiseReader_file",
                url: "WiseReader/File/{id}",
                defaults: new { controller = "Files", action = "File" });

            context.MapRoute(
                name: "WiseReader_action",
                url: "WiseReader/{action}",
                defaults: new { controller = "Index" });

            context.MapRoute(
                name: "WiseReader_controller",
                url: "WiseReader/{controller}/{action}",
                defaults: new { action = "Index" });



            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/WiseReader/Content/js/wisereader").Include(
                    "~/Areas/WiseReader/Content/js/wisereader.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/WiseReader/Content/js/texteditor").Include(
                    "~/Areas/WiseReader/Content/js/texteditor.js"));

            BundleTable.Bundles.Add(
                new ScriptBundle("~/Areas/WiseReader/Content/js/viewer").Include(
                    "~/Areas/WiseReader/Content/js/viewer.js"));
        }
    }
}