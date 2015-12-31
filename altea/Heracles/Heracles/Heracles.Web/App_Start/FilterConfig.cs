namespace Heracles.Web
{
    using System.Web.Mvc;

    using Heracles.Web.ActionFilters;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute(), 0);
            filters.Add(new HandleAjaxErrorAttribute(), 1);
            filters.Add(new RaygunLogErrorAttribute(), 2);
        }
    }
}
