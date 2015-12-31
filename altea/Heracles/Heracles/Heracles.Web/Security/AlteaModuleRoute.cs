namespace Heracles.Web.Security
{
    using System.Web.Routing;

    public class AlteaModuleRoute
    {
        public string Action { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

        public bool Visible { get; set; }

        public bool StatsVisible { get; set; }
        public bool HasController { get; set; }

        public bool HasStats { get; set; }

        public bool HasLevel { get; set; }

        public bool HasProLevel { get; set; }
    }
}