namespace Heracles.Web.ActionFilters
{
    using System.Web.Mvc;

#if DEBUG
    using System.Diagnostics;
#else
    using System;
    using System.Web.Security;

    using Mindscape.Raygun4Net;
    using Mindscape.Raygun4Net.Messages;

    using Heracles.Web.Security;
#endif

    public class RaygunLogErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
#if DEBUG
            Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
            Debug.WriteLine(filterContext.Exception.Message);
#else
            Exception exception = filterContext.Exception;
            RaygunClient client = new RaygunClient();

            bool userAuthenticated = filterContext.HttpContext.User.Identity.IsAuthenticated;
            string userIdentifier = userAuthenticated ? filterContext.HttpContext.User.Identity.Name : string.Empty;

            client.User = userIdentifier;
            client.UserInfo = new RaygunIdentifierMessage(userIdentifier)
                {
                    IsAnonymous = !userAuthenticated,
                    UUID =
                        userAuthenticated
                            ? ((AlteaMembershipProvider)Membership.Provider).GetProviderUserKey(userIdentifier).ToString()
                            : string.Empty,
                        FullName = userIdentifier
                };
            
            client.Send(exception);
#endif
        }
    }
}