namespace Heracles.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.Admin;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean:Create Groups")]
    public class GroupsController : AlteaController
    {
        [HttpPost, OnlyAjax]
        public ActionResult SetLevels(Guid groupId, IEnumerable<int> levels, int primaryLevel)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);
            AdminMemberLevel[] adminLevels = new AdminMemberLevel[levels.Count()];

            int i = 0;
            foreach (int level in levels)
            {
                AdminMemberLevel l = new AdminMemberLevel()
                    {
                        Level = level,
                        SubLevel = null,
                        Primary = level == primaryLevel
                    };

                adminLevels[i++] = l;
            }

            bool status = GroupService.SetGroupLevels(
                AppCore.AppId,
                groupId,
                this.AlteaUser.From,
                this.AlteaUser.To,
                adminLevels);

            if (status)
            {
                AlteaCache.RemoveKey("GROUP_" + groupId, AlteaCache.Scope.Role, AlteaCache.Term.Medium);
            }

            return this.JsonNet(status);
        }
    }
}