namespace Heracles.Web.Areas.Dean.Controllers
{
    using System;
    using System.Web.Mvc;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean")]
    public class DesksExamsController : AlteaController
    {
        #region User

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult Assign(Guid user, int level, int part, bool vocabulary, bool remote, bool status, int offsetDate)
        {
            DesksService.ExamAssign(
                user,
                AlteaMemberType.User,
                DesksExamQuestionType.Student,
                level,
                part,
                vocabulary,
                remote,
                status,
                false,
                this.AlteaUser.Id,
                offsetDate);

            return new EmptyResult();
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult AssignTest(Guid user, int level, int group, int paper, int test, int round, bool remote, bool status, int offsetDate)
        {
            DesksService.ExamTestAssign(
                user,
                AlteaMemberType.User,
                DesksExamQuestionType.Student,
                level,
                group,
                paper,
                test,
                round,
                null,
                remote,
                status,
                false,
                this.AlteaUser.Id,
                offsetDate);

            return new EmptyResult();
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult AssignMixedTest(Guid user, int level, int group, int paper, int? test, int? round, int[] parts, bool remote)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Group

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:Group Dean")]
        public ActionResult GroupAssign(Guid group, int level, int part, bool status, int offsetDate)
        {
            DesksService.ExamAssign(
                group,
                AlteaMemberType.Group,
                DesksExamQuestionType.Teacher,
                level,
                part,
                false,
                false,
                status,
                false,
                this.AlteaUser.Id,
                offsetDate);

            return new EmptyResult();
        }

        #endregion
    }
}