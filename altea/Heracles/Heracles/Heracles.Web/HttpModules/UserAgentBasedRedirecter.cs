namespace Heracles.Web.HttpModules
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    public class UserAgentBasedRedirecter : IHttpModule
    {
        private static readonly IEnumerable<Regex> UserAgents;

        static UserAgentBasedRedirecter()
        {
            string userAgents = ConfigurationManager.AppSettings["UserAgentBasedRedirecter"];
            UserAgentBasedRedirecter.UserAgents = string.IsNullOrEmpty(userAgents)
                ? null
                : userAgents.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Regex(x, RegexOptions.IgnoreCase | RegexOptions.Compiled));
        }

        #region Implementation of IHttpModule
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += RedirectMatchedUserAgents;
        }

        private static void RedirectMatchedUserAgents(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;

            if (app != null && UserAgents != null && !string.IsNullOrEmpty(app.Request.UserAgent))
            {
                if (UserAgentBasedRedirecter.UserAgents.Any(x => x.Match(app.Request.UserAgent).Success))
                {
                    app.Response.Clear();
                    app.Response.Close();
                }
            }
        }

        public void Dispose()
        {
        }
        #endregion
    }
}