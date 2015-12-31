namespace Heracles.Web.ActionFilters
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    using Altea.Extensions;

    using Heracles.Services;
    using Heracles.Web.Security;

    using Mindscape.Raygun4Net.Messages;

    public class AlteaAuthAttribute : AuthorizeAttribute
    {
        private string[] _rolesSplit = new string[0];
        private string _roles;

        private string[][] _modulesSplit = new string[0][];
        private string _modules;

        public new string Roles
        {
            get
            {
                return this._roles ?? string.Empty;
            }

            set
            {
                this._roles = value;
                this._rolesSplit = AlteaAuthAttribute.SplitString(value);
            }
        }

        public string Modules
        {
            get
            {
                return this._modules ?? string.Empty;
            }

            set
            {
                this._modules = value;
                this._modulesSplit = AlteaAuthAttribute.SplitModuleString(value);
            }
        }

        public bool MustHaveLevel { get; set; }

        public bool MustHaveProLevel { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            IPrincipal principal = filterContext.HttpContext.User;
            AlteaRoleProvider roleProvider = (AlteaRoleProvider)System.Web.Security.Roles.Provider;
            AlteaMembershipProvider membershipProvider = (AlteaMembershipProvider)System.Web.Security.Membership.Provider;

            HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket == null || authTicket.Expired)
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            bool isLocal = AppCore.IsLocal(filterContext.HttpContext.Request.GetIpAddress());
            if ((isLocal && authTicket.UserData[0] != AppCore.LocalTicketData)
                || (!isLocal && authTicket.UserData[0] != AppCore.RemoteTicketData))
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            if (!principal.Identity.IsAuthenticated)
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            string name = principal.Identity.Name;

            if (membershipProvider.UserIsLocked(name) || !membershipProvider.UserIsApproved(name))
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            if (this._rolesSplit.Length != 0 && this._rolesSplit.All(role => !roleProvider.IsUserInRole(name, role)))
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }


            if (this.MustHaveLevel && !UserService.UserHasLevel(AppCore.AppId, name))
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            if (this.MustHaveProLevel && !UserService.UserHasProLevel(AppCore.AppId, name))
            {
                this.HandleUnauthorizedRequest(filterContext);
                return;
            }

            if (this._modulesSplit.Length != 0
                && this._modulesSplit.All(
                    module =>
                    (module.Length == 1 && !roleProvider.IsUserInModule(name, module[0], isLocal))
                    || (module.Length == 2 && !roleProvider.IsUserInPermission(name, module[0], module[1], isLocal))))
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }

        private static string[] SplitString(string original)
        {
            return string.IsNullOrWhiteSpace(original)
                ? new string[0]
                : original.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        private static string[][] SplitModuleString(string original)
        {
            return string.IsNullOrWhiteSpace(original)
                ? new string[0][]
                : original.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(
                        x => x.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Take(2).ToArray())
                    .ToArray();
        }
    }
}