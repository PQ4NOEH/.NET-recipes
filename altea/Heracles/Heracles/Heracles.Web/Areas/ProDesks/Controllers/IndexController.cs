namespace Heracles.Web.Areas.ProDesks.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Altea.Classes.ProDesks;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.ProDesks;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "ProDesks", MustHaveProLevel = true)]
    public class IndexController : AlteaController
    {
        [HttpGet]
        public ActionResult Index(int? id, int? subId)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

            int levelId;

            if (id.HasValue
                && UserService.UserHasProLevel(
                    AppCore.AppId,
                    this.AlteaUser.Name,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    id.Value,
                    subId))
            {
                levelId = id.Value;
            }
            else
            {
                levelId = this.AlteaUser.ProLevel.Id;
            }

            ProLevel level;
            AlteaCache.GetOrInsert(
                "LEVEL_" + language + "_" + levelId + "_" + (subId ?? 0),
                true,
                () => ProDesksService.GetLevel(this.AlteaUser.From, levelId, subId),
                AlteaCache.Scope.Instance,
                AlteaCache.Term.Largest,
                out level);

            string columns = AppCore.AppSettings["prodesks_columns_" + language];

            ProDesksHomeModel model = new ProDesksHomeModel
            {
                Level = level,
                Columns = columns.FromJson<dynamic>()
            };

            return this.View("Index", model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult List(int id, int? level)
        {
            if (!UserService.UserHasProLevel(
                    AppCore.AppId,
                    this.AlteaUser.Name,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    id,
                    level))
            {
                return this.JsonNet(null);
            }

            ProDesksList list;
            AlteaCache.GetOrInsert(
                "PRODESKS_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + id + "_" + (level ?? 0),
                true,
                () => ProDesksService.GetList(this.AlteaUser.From, id, level, ProDesksQuestionType.ProStudent),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out list);

            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Assignments(int id, int? level)
        {
            if (!UserService.UserHasProLevel(
                    AppCore.AppId,
                    this.AlteaUser.Name,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    id,
                    level))
            {
                return this.JsonNet(null);
            }

            IEnumerable<IProDesksAssignment> assignments = ProDesksService.GetAssignments(this.AlteaUser.Id, id, level);
            return this.JsonNet(assignments);
        }
    }
}