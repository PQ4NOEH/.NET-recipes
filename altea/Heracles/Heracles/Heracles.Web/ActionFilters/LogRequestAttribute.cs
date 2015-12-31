using System.Web.Mvc;

namespace Heracles.Web.ActionFilters
{
    public class LogRequestAttribute : ActionFilterAttribute
    {
        public string TableName { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //@TODO: LOG!!!
            
            base.OnActionExecuting(filterContext);
        }
    }
}