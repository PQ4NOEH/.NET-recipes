using Altea.Extensions;
using System;
using System.Web;
using System.Web.Mvc;

namespace Heracles.Web
{
    public class JsonNetResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(Data.ToJson());
        }
    }
}