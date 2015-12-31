namespace Heracles.Web.Areas.Desks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
#if !DEBUG
    using System.Web.UI;
#endif

    using Altea.Classes.Desks;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.Desks;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "DesksBooks")]
    public class BooksController : AlteaController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return this.View("Books");
        }

        [HttpPost, OnlyAjax]
        public ActionResult List()
        {

            int autoblockDays = Convert.ToInt32(this.AlteaUser["desks_autounblock_days"]);
            DesksService.AutoUnblock(this.AlteaUser.Id, 0, autoblockDays);
            DesksService.CheckLastBlock(this.AlteaUser.Id, 0, autoblockDays);

            DesksBookList list;

            AlteaCache.GetOrInsert(
                "DESKS_BOOKS_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName),
                true,
                () => DesksService.GetBooksList(this.AlteaUser.From, DesksBookQuestionType.Student),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out list);

            IEnumerable<DesksBookAssignment> assignments = DesksService.GetBookAssignments(this.AlteaUser.Id, this.AlteaUser.From);
            list.SetAssignments(assignments);
            
            return this.JsonNet(list);
        }

        [HttpGet]
#if !DEBUG
        [OutputCache(Duration = 31536000, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "publication;collection")]
#endif
        public ActionResult Cover(long publication, int collection)
        {
            byte[] cover;

            bool exists = AlteaCache.Get(
                "DESKS_BOOKS_Cover_" + publication + "_" + (collection != 0),
                true,
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out cover);

            if (!exists)
            {
                AlteaCache.LockKey("DESKS_BOOKS_Covers_" + (collection != 0) + "_En");

                exists = AlteaCache.Get(
                    "DESKS_BOOKS_Cover_" + publication + "_" + (collection != 0),
                    true,
                    AlteaCache.Scope.Role,
                    AlteaCache.Term.Largest,
                    out cover);

                if (!exists)
                {
                    IDictionary<long, byte[]> covers = DesksService.GetBookCovers(this.AlteaUser.From, collection != 0);

                    foreach (KeyValuePair<long, byte[]> c in covers)
                    {
                        AlteaCache.InsertOrUpdate(
                            "DESKS_BOOKS_Cover_" + c.Key + "_" + (collection != 0),
                            () => c.Value ?? new byte[] { 0 },
                            AlteaCache.Scope.Role,
                            AlteaCache.Term.Largest);

                        if (c.Key == publication)
                        {
                            cover = c.Value;
                        }
                    }
                }

                AlteaCache.UnlockKey("DESKS_BOOKS_Covers_" + (collection != 0) + "_En");
            }

            if (cover == null || (cover.Length == 1 && cover[0] == 0))
            {
                return this.File(this.Url.Content("~/Content/img/nocover.svg"), "image/svg+xml");
            }

            return this.File(cover, "image/jpeg");
        }

        [HttpGet]
        public ActionResult Part(int level, int part, int vocabulary)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            int autoblockDays = Convert.ToInt32(this.AlteaUser["desks_autounblock_days"]);
            DesksService.CheckLastBlock(this.AlteaUser.Id, 0, autoblockDays);

            long id = DesksService.GetExamsAssignmentId(this.AlteaUser.Id, level, part, vocabulary != 0, allowLocal, allowRemote);
            if (id == 0)
            {
                return this.RedirectToAction("Index", "Exams", new { area = "Desks" });
            }

            DesksExerciseModel model = new DesksExerciseModel
            {
                Id = id,
                Round = 0, // TODO,
                ViewName = "aaa", // TODO,
                Title = "Paper Name", // TODO,
                Subtitle = "Part Name" // TODO,
            };

            return this.View("Part", model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Get(long id)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsExamAssigned(this.AlteaUser.Id, id, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            DesksExamData data = DesksService.GetExamsData(id);
            return this.JsonNet(data);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Start(long id)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsExamAssigned(this.AlteaUser.Id, id, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            //TODO
            int codeValidFor = 0;

            string code = DesksService.StartExam(id, codeValidFor);
            return this.JsonNet(code);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Check(long id, string code, int[] questions, string[][] answers)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsExamAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            DesksCheckResult result = DesksService.CheckExam(id, code, questions, answers);
            return this.JsonNet(result);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Finish(long id, string code)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsExamAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            bool hasAnalyse = DesksService.FinishExam(id, code);
            return this.JsonNet(hasAnalyse);
        }

        [HttpGet]
        public ActionResult Analyse(long id, string code)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsExamAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            return this.JsonNet(null);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Report(long id, string code, int? question, string feedback)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsExamAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            return this.JsonNet(null);
        }
    }
}