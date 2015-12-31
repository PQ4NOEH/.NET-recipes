namespace Heracles.Web.Areas.Dean.Controllers
{
    using System;
    using System.Web.Mvc;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean")]
    public class DesksIndexController : AlteaController
    {
        #region User

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult Assign(
            Guid user,
            int level,
            int area,
            int subject,
            int type,
            int num,
            bool remote,
            bool status,
            int offsetDate)
        {
            DesksService.IndexAssign(
                user,
                AlteaMemberType.User,
                DesksIndexQuestionType.Student,
                level,
                area,
                subject,
                type,
                num,
                remote,
                status,
                false,
                this.AlteaUser.Id,
                offsetDate);

            return new EmptyResult();
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult Unblock(Guid user, int level, int area, int subject, int type, int offsetDate)
        {
            DesksService.IndexAssign(
                user,
                AlteaMemberType.User,
                DesksIndexQuestionType.Student,
                level,
                area,
                subject,
                type,
                0,
                false,
                false,
                true,
                this.AlteaUser.Id,
                offsetDate);

            return new EmptyResult();
        }

        #endregion

        #region Group

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:Group Dean")]
        public ActionResult GroupAssign(
            Guid group,
            int level,
            int area,
            int subject,
            int type,
            int num,
            bool status,
            int offsetDate)
        {
            DesksService.IndexAssign(
                group,
                AlteaMemberType.Group,
                DesksIndexQuestionType.Teacher,
                level,
                area,
                subject,
                type,
                num,
                false,
                status,
                false,
                this.AlteaUser.Id,
                offsetDate);

            return new EmptyResult();
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:Group Dean")]
        public ActionResult GroupUsersAssign(
            Guid group,
            int level,
            int area,
            int subject,
            int type,
            int num,
            bool status,
            int offsetDate)
        {
            DesksService.IndexAssign(
                group,
                AlteaMemberType.Group,
                DesksIndexQuestionType.Student,
                level,
                area,
                subject,
                type,
                num,
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