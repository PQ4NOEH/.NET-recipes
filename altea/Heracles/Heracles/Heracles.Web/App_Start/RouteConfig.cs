namespace Heracles.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Stats",
                url: "Stats/",
                defaults: new { controller = "Stats", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                name: "Speech",
                url: "WiseLab/Speech/{from}/{word}",
                defaults: new { controller = "WiseLab", action = "Speech" });

            routes.MapRoute(
                name: "Parameter",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" });
        }
    }
}
