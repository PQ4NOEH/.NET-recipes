namespace Heracles.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    using Altea.Extensions;

    using Heracles.Models.Home;
    using Heracles.Services;
    using Heracles.Web.Security;

    public class HomeController : AlteaController
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                bool hasLevel = this.AlteaUser.Level != null;
                bool hasProLevel = this.AlteaUser.ProLevel != null;

                IEnumerable<AlteaModule> modules = RoleProvider.GetModulesForUser(this.User.Identity.Name, this.Request.IsLocal);
                AlteaModule[] visibleModules =
                    modules.Where(
                        x =>
                        x.Route != null && x.Route.Visible && x.Route.HasController && (hasLevel || !x.Route.HasLevel)
                        && (hasProLevel || !x.Route.HasProLevel)).ToArray();

                AlteaModule finalModule;
                switch (visibleModules.Length)
                {
                    case 0:
                        return this.View("Stats");

                    case 1:
                        finalModule = visibleModules[0];
                        break;

                    default:
                        if (visibleModules.Any(x => x.Route.StatsVisible))
                        {
                            return this.View("Stats");
                        }

                        finalModule = visibleModules.OrderBy(x => x.Priority).First();
                        break;
                }

                return this.RedirectToAction(finalModule.Route.Action, finalModule.Route.RouteValues);
            }

            return this.View("Index");
        }

        [HttpPost]
        public ActionResult Index(LogInModel model, string returnUrl)
        {
            LogInStatusModel status = new LogInStatusModel();

            if (this.User.Identity.IsAuthenticated)
            {
                status.Status = false;
                status.Message = Resources.SiteResources.AlreadyAuthenticated;
            }
            else if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                status.Status = false;
                status.Message = Resources.SiteResources.FillUsernameAndPassword;
            }
            else if (!MembershipProvider.ValidateUser(model.Username, model.Password))
            {
                status.Status = false;
                status.Message = Resources.SiteResources.InvalidUsernameOrPassword;
            }
            else if (MembershipProvider.UserIsLocked(model.Username))
            {
                status.Status = false;
                status.Message = Resources.SiteResources.LockedAccount;
            }
            else if (!MembershipProvider.UserIsApproved(model.Username))
            {
                status.Status = false;
                status.Message = Resources.SiteResources.NoAccessPermission;
            }
            else if (!AppCore.IsLocal(this.Request.GetIpAddress()) && !UserService.CheckRemote(model.Username, AppCore.AppId))
            {
                status.Status = false;
                status.Message = Resources.SiteResources.NoRemoteAccessPermission;
            }
            else
            {
                status.Status = true;

                Guid userId = (Guid)MembershipProvider.GetUser(model.Username, true).ProviderUserKey;
                AlteaCache.RemoveKey("__USER__" + userId, AlteaCache.Scope.Instance, AlteaCache.Term.Medium);

                bool persistent;
                bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());

                if (model.Remember && isLocal)
                {
                    string[] roles = RoleProvider.GetRolesForUser(model.Username);
                    persistent = roles.Contains("Admin") || roles.Contains("Developer");
                }
                else
                {
                    persistent = false;
                }

                DateTime utcNow = DateTime.UtcNow;
                DateTime cookieExpiration = persistent ? utcNow.AddMonths(AppCore.PersistentTicketExpiration) : DateTime.MinValue;
                DateTime ticketExpiration = persistent
                                                ? cookieExpiration
                                                : (isLocal
                                                       ? utcNow.AddHours(AppCore.LocalTicketExpiration)
                                                       : utcNow.AddHours(AppCore.RemoteTicketExpiration));

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    model.Username,
                    utcNow,
                    ticketExpiration,
                    persistent,
                    (isLocal ? AppCore.LocalTicketData : AppCore.RemoteTicketData) + userId.ToString());

                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Value = FormsAuthentication.Encrypt(ticket),
                    Expires = cookieExpiration,
                    HttpOnly = true
                };

                this.Response.Cookies.Add(cookie);
            }

            return this.JsonNet(status);
        }
    }
}