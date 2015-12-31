namespace Heracles.Web.Areas.WiseTank.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.WiseTank;
    using Altea.Extensions;

    using Heracles.Models;
    using Heracles.Models.WiseTank;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "WiseTank")]
    public class IndexController : AlteaController
    {
        // POST: /WiseTank
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        // POST: /WiseTank/GetData
        [HttpPost, OnlyAjax]
        public ActionResult GetData(int offsetDate)
        {
            WiseTankService.CheckDefaultTimelines(this.AlteaUser.Id, AppCore.AppId, this.AlteaUser.From, offsetDate);

            IDictionary<string, int> areas = GetAreas();

            List<TankTimeline> timelines = this.GetTimelines() as List<TankTimeline>;
            List<TankStream> streams =
                WiseTankService.GetStreamData(this.AlteaUser.Id, this.AlteaUser.From) as List<TankStream>;

            IEnumerable<Guid> users =
                timelines.Where(x => x.CreatedById.HasValue)
                    .Select(x => x.CreatedById.Value)
                    .Union(
                        streams.SelectMany(
                            x => x.Boxes.Where(y => y.Type == TankBoxType.User).Select(y => new Guid(y.Query))));

            IDictionary<Guid, UserData> usersData = UserService.GetUsersData(users);

            foreach (TankTimeline timeline in timelines.Where(timeline => timeline.CreatedById.HasValue))
            {
                timeline.CreatedBy = usersData[timeline.CreatedById.Value].UserName;
            }

            foreach (TankBox box in streams.SelectMany(x => x.Boxes).Where(x => x.Type == TankBoxType.User))
            {
                box.Query = usersData[new Guid(box.Query)].UserName;
            }

            IEnumerable<TankRole> roles;
            AlteaCache.GetOrInsert(
                "__WISETANK__Roles",
                true,
                WiseTankService.GetRoles,
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Largest,
                out roles);

            WiseTankStreamDataModel model = new WiseTankStreamDataModel
                {
                    Areas = areas,
                    Timelines = timelines,
                    Streams = streams,
                    Roles = roles
                };

            return this.JsonNet(model);
        }

        // POST: /WiseTank/GetTimelines
        [HttpPost, OnlyAjax]
        public ActionResult GetTimelines(int offsetDate)
        {
            WiseTankService.CheckDefaultTimelines(this.AlteaUser.Id, AppCore.AppId, this.AlteaUser.From, offsetDate);

            IDictionary<string, int> areas = GetAreas();
            IEnumerable<TankTimeline> timelines = this.GetTimelines();

            WiseTankDataModel model = new WiseTankDataModel
                {
                    Areas = areas,
                    Timelines = timelines
                };

            return this.JsonNet(model);
        }

        private static IDictionary<string, int> GetAreas()
        {
            Dictionary<string, int> areas = Enum.GetValues(typeof(TankArea))
                .Cast<TankArea>()
                .ToDictionary(x => x, x => x.GetAttribute<TankAreaPropertiesAttribute>())
                .Where(x => x.Value.PlannerVisible)
                .OrderBy(x => x.Value.Position)
                .ToDictionary(x => x.Key.ToString(), x => (int)x.Key);

            return areas;
        }

        private IEnumerable<TankTimeline> GetTimelines()
        {
            IEnumerable<TankTimeline> timelines = WiseTankService.GetTimelines(
               this.AlteaUser.Id,
               AppCore.AppId,
               this.AlteaUser.From);

            return timelines;
        } 
    }
}