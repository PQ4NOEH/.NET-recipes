namespace Heracles.Web.Areas.WiseTank.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

    using Altea.Classes.WiseTank;

    using Heracles.Models;
    using Heracles.Models.WiseTank;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;
    using Heracles.Web.Security;

    [AlteaAuth(Modules = "WiseTank")]
    public class TimelinesController : AlteaController
    {
        // POST: /WiseTank/Timelines/GetData
        [HttpPost, OnlyAjax]
        public ActionResult GetData(Guid timeline, long[] lastMessage, int direction)
        {
            WiseTankStreamBoxDataModel model = WiseTankService.GetTimelineData(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline,
                Convert.ToInt32(AppCore.AppSettings["wisetank_stream_numarticles"]),
                lastMessage,
                direction);

            IEnumerable<Guid> users = model.Articles.SelectMany(
                x =>
                {
                    bool hasAssigned = x.AssignedById.HasValue;
                    Guid[] ids = new Guid[hasAssigned ? 3 : 2];
                    ids[0] = x.UserId;
                    ids[1] = x.AuthorId;
                    if (hasAssigned)
                    {
                        ids[2] = x.AssignedById.Value;
                    }
                    return ids;
                });
            IDictionary<Guid, UserData> usersData = UserService.GetUsersData(users);

            foreach (TankStreamArticle article in model.Articles)
            {
                article.User = usersData[article.UserId].UserName;
                article.UserFullName = usersData[article.UserId].FirstName + " " + usersData[article.UserId].LastName;
                article.Author = usersData[article.AuthorId].UserName;
                article.AuthorFullName = usersData[article.AuthorId].FirstName + " " + usersData[article.AuthorId].LastName;
                if (article.AssignedById.HasValue)
                {
                    article.AssignedBy = usersData[article.AssignedById.Value].UserName;
                    article.AssignedFullName = usersData[article.AssignedById.Value].FirstName + " " + usersData[article.AssignedById.Value].LastName;
                }
            }

            return this.JsonNet(model);
        }

        // POST: /WiseTank/Timelines/GetUsers
        [HttpPost, OnlyAjax]
        public ActionResult GetUsers(Guid timeline)
        {
            WiseTankUserDataModel model = WiseTankService.GetTimelineUserData(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline);

            IEnumerable<Guid> users = model.UserData.Values.SelectMany(x => x.Select(y => y.UserId));
            IDictionary<Guid, UserData> usersData = UserService.GetUsersData(users);

            foreach (TankTimelineUser user in model.UserData.Values.SelectMany(x => x as List<TankTimelineUser>))
            {
                user.UserName = usersData[user.UserId].UserName;
                user.FullName = usersData[user.UserId].FirstName + " " + usersData[user.UserId].LastName;
            }

            return this.JsonNet(model);
        }

        // POST: /WiseTank/Timelines/AddUser
        [HttpPost, OnlyAjax]
        public ActionResult AddUser(Guid timeline, string username, int thinktankLevel, int role, int permissions)
        {
            Guid userId = ((AlteaMembershipProvider)Membership.Provider).GetProviderUserKey(username);

            if (userId == Guid.Empty)
            {
                return this.JsonNet(WiseTankError.InvalidUser);
            }

            WiseTankError error = WiseTankService.AddTimelineUser(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline,
                userId,
                thinktankLevel,
                role,
                permissions);

            return this.JsonNet(error);
        }

        // POST: /WiseTank/Timelines/EditUser
        [HttpPost, OnlyAjax]
        public ActionResult EditUser(Guid timeline, Guid user, int thinktankLevel, int role, int permissions)
        {
            WiseTankError error = WiseTankService.EditTimelineUser(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline,
                user,
                thinktankLevel,
                role,
                permissions);

            return this.JsonNet(error);
        }

        // POST: /WiseTank/Timelines/Get
        [HttpPost, OnlyAjax]
        public ActionResult Get(TankArea area)
        {
            List<TankTimeline> timelines = WiseTankService.GetAreaTimelines(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                area) as List<TankTimeline>;

            IEnumerable<Guid> users = timelines.Where(x => x.CreatedById.HasValue).Select(x => x.CreatedById.Value);
            IDictionary<Guid, UserData> usersData = UserService.GetUsersData(users);

            foreach (TankTimeline timeline in timelines.Where(timeline => timeline.CreatedById.HasValue))
            {
                timeline.CreatedBy = usersData[timeline.CreatedById.Value].UserName;
            }

            return this.JsonNet(timelines);
        }

        // POST: /WiseTank/Timelines/Create
        [HttpPost, OnlyAjax]
        public ActionResult Create(
            string name,
            string description,
            TankAccessType accessType,
            bool moderatedArticles,
            bool moderatedComments,
            bool writeOwnArticles,
            IEnumerable<WiseTankRolePermissionTypeModel> permissionTypes,
            TankArea area,
            int offsetDate)
        {
            Guid id = WiseTankService.CreateTimeline(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                name,
                description,
                accessType,
                moderatedArticles,
                moderatedComments,
                writeOwnArticles,
                permissionTypes.ToDictionary(x => x.Role, x => x.PermissionType),
                area,
                offsetDate);

            WiseTankError status = id == Guid.Empty ? WiseTankError.UnknownError : WiseTankError.NoError;

            WiseTankCreateStatusModel model = new WiseTankCreateStatusModel
                {
                    Status = status,
                    Id = status == WiseTankError.NoError ? id : (Guid?)null
                };

            return this.JsonNet(model);
        }

        // POST: /WiseTank/Timelines/Edit
        [HttpPost, OnlyAjax]
        public ActionResult Edit(
            Guid timeline,
            string name,
            string description,
            TankAccessType accessType,
            bool moderatedArticles,
            bool moderatedComments,
            bool writeOwnArticles,
            IEnumerable<WiseTankRolePermissionTypeModel> permissionTypes)
        {
            WiseTankError status = WiseTankService.EditTimeline(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline,
                name,
                description,
                accessType,
                moderatedArticles,
                moderatedComments,
                writeOwnArticles,
                permissionTypes.ToDictionary(x => x.Role, x => x.PermissionType));

            return this.JsonNet(status);
        }

        // POST: /WiseTank/Timelines/AddColumn
        [HttpPost, OnlyAjax]
        public ActionResult AddColumn(
            Guid timeline,
            string name,
            int ttlevel,
            TankAccessType accessType,
            bool moderatedArticles,
            bool moderatedComments,
            bool writeOwnArticles,
            int offsetDate)
        {
            if (accessType == TankAccessType.Protected)
            {
                WiseTankCreateStatusModel error = new WiseTankCreateStatusModel
                {
                    Status = WiseTankError.UnknownError,
                    Id = null
                };

                return this.JsonNet(error);
            }

            Guid id = WiseTankService.AddTimelineColumn(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline,
                name,
                ttlevel,
                accessType,
                moderatedArticles,
                moderatedComments,
                writeOwnArticles,
                offsetDate);

            WiseTankError status = id == Guid.Empty ? WiseTankError.UnknownError : WiseTankError.NoError;

            WiseTankCreateStatusModel model = new WiseTankCreateStatusModel
                {
                    Status = status,
                    Id = status == WiseTankError.NoError ? id : (Guid?)null
                };

            return this.JsonNet(model);
        }

        // POST: /WiseTank/Timelines/EditGroupBoxWidth
        [HttpPost, OnlyAjax]
        public ActionResult EditGroupBoxWidth(Guid timeline, int width)
        {
            WiseTankService.EditGroupBoxWidth(this.AlteaUser.Id, AppCore.AppId, this.AlteaUser.From, timeline, width);
            return new EmptyResult();
        }
    }
}