namespace Heracles.Web
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/raygun").Include("~/Scripts/raygun.min.js"));

            bundles.Add(
                new ScriptBundle("~/Content/js/libs").Include(
                    "~/Content/js/lib/i18next.min.js",
                    "~/Content/js/lib/xregexp.js",
                    "~/Content/js/lib/moment.js",
                    "~/Content/js/lib/jquery.mousewheel.min.js",
                    "~/Content/js/lib/jquery.mCustomScrollbar.min.js",
                    "~/Content/js/lib/jquery.transit.js",
                    "~/Content/js/lib/jquery.tipsy.min.js",
                    "~/Content/js/lib/noty/jquery.noty.packaged.min.js",
                    "~/Content/js/lib/noty/layouts/*.js",
                    "~/Content/js/lib/noty/themes/*.js",
                    "~/Content/js/lib/soundmanager2-nodebug-jsmin.js",
                    "~/Content/js/lib/jquery.fileupload.js",
                    "~/Content/js/lib/jquery.knob.js",
                    "~/Content/js/lib/selectize.js",
                    "~/Content/js/lib/jquery.nouislider.all.min.js",
                    "~/Content/js/lib/uri.js",
                    "~/Content/js/lib/context.js",
                    "~/Content/js/lib/d3.min.js",
                    "~/Content/js/lib/nv.d3.min.js",
                    "~/Content/js/lib/jquery.timepicker.min.js",
                    "~/Content/js/lib/datepair.min.js"));

            bundles.Add(
                new ScriptBundle("~/Content/js/lib/ckeditor/ckeditor").Include(
                    "~/Content/js/lib/ckeditor/ckeditor.js"));

            bundles.Add(
                new ScriptBundle("~/Content/js/core").Include(
                    "~/Content/js/corelib/extend.js",
                    "~/Content/js/corelib/mpd.js",
                    "~/Content/js/corelib/pagination.js",
                    "~/Content/js/corelib/shake.js",
                    "~/Content/js/corelib/internalerror.js",
                    "~/Content/js/corelib/dom.js",
                    "~/Content/js/corelib/createnoty.js",
                    "~/Content/js/corelib/sm.js",
                    "~/Content/js/corelib/wordcounter.js",
                    "~/Content/js/corelib/highlight.js",
                    "~/Content/js/corelib/selectiondata.js",
                    "~/Content/js/corelib/arrays.js",
                    "~/Content/js/corelib/number.js",
                    "~/Content/js/corelib/date.js",
                    "~/Content/js/corelib/base64.js",
                    "~/Content/js/corelib/time.js",
                    "~/Content/js/corelib/achievements.js"));

            bundles.Add(new ScriptBundle("~/Content/js/stats").Include("~/Content/js/stats.js"));

            bundles.Add(new ScriptBundle("~/Content/js/stax").Include("~/Content/js/stax.js"));
            bundles.Add(new ScriptBundle("~/Content/js/staxindex").Include("~/Content/js/staxindex.js"));
            bundles.Add(new ScriptBundle("~/Content/js/listmanager").Include("~/Content/js/listmanager.js"));
            
            bundles.Add(new ScriptBundle("~/Content/js/vocabulary").Include("~/Content/js/vocabulary.js"));
            bundles.Add(new ScriptBundle("~/Content/js/vocabularyindex").Include("~/Content/js/vocabularyindex.js"));

            bundles.Add(new ScriptBundle("~/Content/js/wiselab").Include("~/Content/js/wiselab.js"));
            bundles.Add(new ScriptBundle("~/Content/js/wisetank").Include("~/Content/js/wisetank.js"));

            bundles.Add(new ScriptBundle("~/Content/js/desks/engine").Include("~/Content/js/desks/engine.js"));
            bundles.Add(new ScriptBundle("~/Content/js/desks/index").Include("~/Content/js/desks/index.js"));
            bundles.Add(new ScriptBundle("~/Content/js/desks/exams").Include("~/Content/js/desks/exams.js"));
            bundles.Add(new ScriptBundle("~/Content/js/desks/books").Include("~/Content/js/desks/books.js"));
            bundles.Add(new ScriptBundle("~/Content/js/desks/extra").Include("~/Content/js/desks/extra.js"));
            bundles.Add(new ScriptBundle("~/Content/js/prodesks").Include("~/Content/js/prodesks.js"));

            bundles.Add(
                new ScriptBundle("~/Content/js/developer").Include(
                    "~/Content/js/dev/cache.js",
                    "~/Content/js/dev/clearcache.js",
                    "~/Content/js/dev/clearsettings.js"));


            bundles.Add(
                new StyleBundle("~/Content/css/libs").Include(
                    "~/Content/css/lib/jquery.mCustomScrollbar.css",
                    "~/Content/css/lib/jquery.tipsy.css",
                    "~/Content/css/lib/animate.css",
                    "~/Content/css/lib/font-awesome.css",
                    "~/Content/css/lib/selectize.default.css",
                    "~/Content/css/lib/jquery.nouislider.min.css",
                    "~/Content/css/lib/context.standalone.css",
                    "~/Content/css/lib/nv.d3.css",
                    "~/Content/css/lib/jquery.timepicker.css"));



            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
