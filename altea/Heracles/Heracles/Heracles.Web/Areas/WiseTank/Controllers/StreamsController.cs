namespace Heracles.Web.Areas.WiseTank.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.WiseTank;

    using Heracles.Models;
    using Heracles.Models.WiseTank;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "WiseTank")]
    public class StreamsController : AlteaController
    {
        // POST: /WiseTank/Streams/GetData
        [HttpPost, OnlyAjax]
        public ActionResult GetData(Guid stream, Guid box, long[] lastMessage, int direction)
        {
            WiseTankStreamBoxDataModel model = WiseTankService.GetStreamBoxData(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                stream,
                box,
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

        // POST: /WiseTank/Streams/EditBoxWidth
        [HttpPost, OnlyAjax]
        public ActionResult EditBoxWidth(Guid stream, int width)
        {
            WiseTankService.EditBoxWidth(this.AlteaUser.Id, this.AlteaUser.From, stream, width);
            return new EmptyResult();
        }

        // POST: /WiseTank/Streams/SetRefreshRate
        [HttpPost, OnlyAjax]
        public ActionResult SetRefreshRate(Guid stream, int refreshRate)
        {
            WiseTankService.EditBoxRefreshRate(this.AlteaUser.Id, this.AlteaUser.From, stream, refreshRate);
            return new EmptyResult();
        }

        // POST: /WiseTank/Streams/Create
        [HttpPost, OnlyAjax]
        public ActionResult Create(string name)
        {
            Guid id = WiseTankService.CreateStream(this.AlteaUser.Id, this.AlteaUser.From, name);
            WiseTankError status = id == Guid.Empty ? WiseTankError.UnknownError : WiseTankError.NoError;

            WiseTankCreateStatusModel model = new WiseTankCreateStatusModel
                {
                    Status = status,
                    Id = status == WiseTankError.NoError ? id : (Guid?)null
                };

            return this.JsonNet(model);
        }

        // POST: /WiseTank/Streams/Rename
        [HttpPost, OnlyAjax]
        public ActionResult Rename(Guid stream, string name)
        {
            WiseTankError status = WiseTankService.EditStreamName(this.AlteaUser.Id, this.AlteaUser.From, stream, name);
            return this.JsonNet(status);
        }

        // POST: /WiseTank/Streams/Reposition
        [HttpPost, OnlyAjax]
        public ActionResult Reposition(Guid stream, int position)
        {
            WiseTankService.RepositionStream(this.AlteaUser.Id, this.AlteaUser.From, stream, position);
            return new EmptyResult();
        }

        // POST: /WiseTank/Streams/Delete
        [HttpPost, OnlyAjax]
        public ActionResult Delete(Guid stream)
        {
            WiseTankError status = WiseTankService.DeleteStream(this.AlteaUser.Id, this.AlteaUser.From, stream);
            return this.JsonNet(status);
        }

        // POST: /WiseTank/Streams/CreateBox
        [HttpPost, OnlyAjax]
        public ActionResult CreateBox(Guid stream, TankBoxType type, string query)
        {
            Guid id = Guid.Empty;
            WiseTankError status = WiseTankError.NoError;

            if (!Enum.IsDefined(typeof(TankBoxType), type))
            {
                status = WiseTankError.UnknownError;
            }
            else
            {
                switch (type)
                {
                    case TankBoxType.Timeline:


                        break;
                        
                    case TankBoxType.Area:
                        TankArea area = (TankArea)Convert.ToInt32(query);

                        if (!Enum.IsDefined(typeof(TankArea), area))
                        {
                            status = WiseTankError.UnknownError;
                        }
                        break;

                    case TankBoxType.User:
                        Guid providerUserKey = MembershipProvider.GetProviderUserKey(query);
                        if (providerUserKey == Guid.Empty)
                        {
                            status = WiseTankError.UnknownError;
                        }
                        else
                        {
                            query = providerUserKey.ToString();
                        }

                        break;
                }
            }

            if (status == WiseTankError.NoError)
            { 
                id = WiseTankService.CreateBox(this.AlteaUser.Id, this.AlteaUser.From, stream, type, query);
                status = id == Guid.Empty ? WiseTankError.UnknownError : WiseTankError.NoError;
            }

            WiseTankCreateStatusModel model = new WiseTankCreateStatusModel
            {
                Status = status,
                Id = status == WiseTankError.NoError ? id : (Guid?)null
            };

            return this.JsonNet(model);
        }

        // POST: /WiseTank/Streams/RepositionBox
        [HttpPost, OnlyAjax]
        public ActionResult RepositionBox(Guid stream, Guid box, int position)
        {
            WiseTankService.RepositionBox(this.AlteaUser.Id, this.AlteaUser.From, stream, box, position);
            return new EmptyResult();
        }

        // POST: /WiseTank/Streams/DeleteBox
        [HttpPost, OnlyAjax]
        public ActionResult DeleteBox(Guid stream, Guid box)
        {
            WiseTankService.DeleteBox(this.AlteaUser.Id, this.AlteaUser.From, stream, box);
            return new EmptyResult();
        }
    }
}