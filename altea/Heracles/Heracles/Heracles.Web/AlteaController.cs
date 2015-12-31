namespace Heracles.Web
{
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;

    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Services;
    using Heracles.Web.Security;

    public abstract class AlteaController : Controller
    {
        protected static AlteaMembershipProvider MembershipProvider { get; private set; }
        protected static AlteaRoleProvider RoleProvider { get; private set; }

        private User alteaUser;

        private CultureInfo cultureInfo;

        public User AlteaUser
        {
            get
            {
                if (this.alteaUser == null)
                {
                    string name = this.User.Identity.Name;
                    this.alteaUser = this.GetUser(name);
                }

                return this.alteaUser;
            }
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipProvider == null)
            {
                MembershipProvider = (AlteaMembershipProvider)Membership.Provider;
            }

            if (RoleProvider == null)
            {
                RoleProvider = (AlteaRoleProvider)Roles.Provider;
            }


            // Globalization: culture and language
            string culture = null;
            this.cultureInfo = null;

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                this.alteaUser = this.GetUser(requestContext.HttpContext.User.Identity.Name);
                culture = this.alteaUser.From.GetAttribute<LanguagePrefixesAttribute>().Culture;
            }
            else
            {
                string[] languages = requestContext.HttpContext.Request.UserLanguages;
                if (languages != null)
                {
                    culture = languages.FirstOrDefault(x => AppCore.IsAcceptedLanguage(x, out cultureInfo));
                }
            }

            if (this.cultureInfo == null && culture == null)
            {
                culture = AppCore.Setting_DefaultCulture;
            }

            try
            {
                if (this.cultureInfo == null)
                {
                    this.cultureInfo = new CultureInfo(culture);
                }

                if (!this.cultureInfo.IsNeutralCulture)
                {
                    this.cultureInfo = this.cultureInfo.Parent;
                }

                Thread.CurrentThread.CurrentCulture = this.cultureInfo;
                Thread.CurrentThread.CurrentUICulture = this.cultureInfo;
            }
            catch
            {
                // ignored
            }

            base.Initialize(requestContext);
        }

        protected JsonNetResult JsonNet(object data)
        {
            return new JsonNetResult
            {
                Data = data
            };
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            this.ViewData["AlteaViewUser"] = this.User.Identity.IsAuthenticated ? this.AlteaUser : null;
            this.ViewData["CultureInfo"] = this.cultureInfo;
            return base.View(viewName, masterName, model);
        }

        protected override ViewResult View(IView view, object model)
        {
            this.ViewData["AlteaViewUser"] = this.AlteaUser;
            this.ViewData["CultureInfo"] = this.cultureInfo;
            return base.View(view, model);
        }

        private User GetUser(string name)
        {
            User user;

            AlteaCache.GetOrInsert(
                "USER_" + name,
                null,
                () => UserService.GetUser(
                    MembershipProvider.GetUser(name, true),
                    AppCore.AppId,
                    AlteaController.UpdateAlteaUserCache),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Medium,
                out user);

            return user;
        }

        private static void UpdateAlteaUserCache(User user)
        {
            AlteaCache.InsertOrUpdate(
                "USER_" + user.Name,
                () => user,
                AlteaCache.Scope.Role,
                AlteaCache.Term.Medium);
        }
    }
}