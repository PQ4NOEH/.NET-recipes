namespace Heracles.Web.Areas.Dean.Controllers
{
    using System;
    using System.Web.Mvc;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean")]
    public class DesksExtraController : AlteaController
    {
        #region User

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult Assign()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Group

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:Group Dean")]
        public ActionResult GroupAssign(Guid group, int level, int part, int type, bool status)
        {
            DesksService.ExtraAssign(
                group,
                AlteaMemberType.Group,
                DesksExtraQuestionType.Teacher,
                level,
                part,
                type,
                false,
                status,
                false,
                this.AlteaUser.Id);

            return new EmptyResult();
        }

        #endregion
    }
}