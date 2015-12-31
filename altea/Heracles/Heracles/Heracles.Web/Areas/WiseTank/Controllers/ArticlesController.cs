namespace Heracles.Web.Areas.WiseTank.Controllers
{
    using System;
    using System.Web.Mvc;

    using Altea.Classes.WiseTank;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "WiseTank")]
    public class ArticlesController : AlteaController
    {
        // POST: /WiseTank/Articles/Create
        [HttpPost, OnlyAjax]
        public ActionResult Create(
            Guid timeline,
            int origin,
            string reference,
            string source,
            string favicon,
            string title,
            string lead,
            string description,
            string image,
            int offsetDate)
        {
            WiseTankError error = WiseTankService.CreateArticle(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                timeline,
                (TankOrigin)origin,
                reference,
                source,
                favicon,
                title,
                lead,
                description,
                image,
                offsetDate);

            return this.JsonNet(error);
        }

        // POST: /WiseTank/Articles/Vote
        [HttpPost, OnlyAjax]
        public ActionResult Vote(long article, bool? upvote)
        {
            int? votes = WiseTankService.Vote(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                article,
                upvote);

            return this.JsonNet(votes);
        }

        // POST: /WiseTank/Articles/Karma
        [HttpPost, OnlyAjax]
        public ActionResult Karma(long article, int? karma)
        {
            decimal? finalKarma = WiseTankService.Karma(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                article,
                karma);

            return this.JsonNet(finalKarma);
        }

        // GET: /WiseTank/Articles/Image
        [HttpGet]
        public ActionResult Image(long id)
        {
            byte[] image = WiseTankService.GetArticleImage(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                id);

            if (image == null)
            {
                return new HttpNotFoundResult();
            }

            return this.File(image, "image/jpeg");
        }
    }
}