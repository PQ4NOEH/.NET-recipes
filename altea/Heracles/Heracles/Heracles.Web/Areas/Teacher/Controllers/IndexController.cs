using System.Web.Mvc;

namespace Heracles.Web.Areas.Teacher.Controllers
{
    using System;
    using System.Collections.Generic;

    using Altea.Classes.Desks;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.Teacher;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher")]
    public class IndexController : AlteaController
    {
        [HttpGet]
        public ActionResult Index(int? level, Guid? group)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

            IEnumerable<DesksIndexArea> areas;
            AlteaCache.GetOrInsert(
                "DESKS_INDEX_Areas_" + language,
                true,
                () => DesksService.GetIndexAreas(this.AlteaUser.From),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out areas);

            string extra = AppCore.AppSettings["extra_columns_teacher_" + language];

            TeacherHomeModel model = new TeacherHomeModel
                {
                    Areas = areas,
                    Extra = extra.FromJson<dynamic>(),
                    Level = level,
                    Group = group
                };

            return this.View("Index", model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetLevel(int level)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

            DesksIndexList indexList;
            AlteaCache.GetOrInsert(
                "DESKS_INDEX_TEACHER_" + language + "_" + level,
                true,
                () => DesksService.GetIndexList(this.AlteaUser.From, level, DesksIndexQuestionType.Teacher),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out indexList);

            DesksExamList examsList;
            AlteaCache.GetOrInsert(
                "DESKS_EXAMS_TEACHER_" + language + "_" + level,
                true,
                () => DesksService.GetExamsList(this.AlteaUser.From, level, DesksExamQuestionType.Teacher),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out examsList);

            DesksExtraList extraList;
            AlteaCache.GetOrInsert(
                "DESKS_EXTRA_TEACHER_" + language + "_" + level,
                true,
                () => DesksService.GetExtraList(this.AlteaUser.From, level, DesksExtraQuestionType.Teacher),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out extraList);

            TeacherLevelDataModel model = new TeacherLevelDataModel
                {
                    Index = indexList,
                    Exams = examsList,
                    Extra = extraList
                };

            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetGroup(int level, Guid group)
        {
            return this.JsonNet(null);
        }
    }
}