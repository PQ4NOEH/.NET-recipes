namespace Heracles.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

    using Altea.Classes.Admin;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Admin")]
    public class UsersController : AlteaController
    {
        [HttpPost, OnlyAjax]
        public ActionResult Create(
            string firstName,
            string lastName,
            string mail,
            string password,
            bool autoPassword,
            string role,
            int from,
            int to)
        {
            password = autoPassword ? Membership.GeneratePassword(16, 4) : password;
            string username = UserService.CreateUser(
                AppCore.AppId,
                firstName,
                lastName,
                mail,
                password,
                role,
                from.ParseWordLanguageDatabaseId(),
                to.ParseWordLanguageDatabaseId());

            return this.JsonNet(username);
        }

        [HttpPost, OnlyAjax]
        public ActionResult SetLevels(Guid userId, IEnumerable<int> levels, int primaryLevel)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);
            AdminMemberLevel[] adminLevels = new AdminMemberLevel[levels.Count()];

            int i = 0;
            foreach (int level in levels)
            {
                Level lvl;
                AlteaCache.GetOrInsert(
                    "LEVEL_" + level,
                    true,
                    () => AppService.GetLevel(this.AlteaUser.From, level),
                    AlteaCache.Scope.Instance,
                    AlteaCache.Term.Largest,
                    out lvl);

                AdminMemberLevel l = new AdminMemberLevel()
                    {
                        LanguageFrom = this.AlteaUser.From,
                        LanguageTo = lvl.LanguageTo,
                        Level = level,
                        SubLevel = null,
                        Primary = level == primaryLevel
                    };

                adminLevels[i++] = l;
            }

            bool status = UserService.SetUserLevels(
                AppCore.AppId,
                userId,
                this.AlteaUser.From,
                this.AlteaUser.To,
                adminLevels);

            if (status)
            {
                string name = UserService.GetUserName(userId);
                AlteaCache.RemoveKey("USER_" + name, AlteaCache.Scope.Role, AlteaCache.Term.Medium);
            }

            return this.JsonNet(status);
        }
    }
}