namespace Heracles.Web.Areas.Desks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Altea.Classes.Desks;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.Desks;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "DesksExams", MustHaveLevel = true)]
    public class ExamsController : AlteaController
    {
        [HttpGet]
        public ActionResult Index(int? id)
        {
            int levelId;

            if (id.HasValue
                && UserService.UserHasLevel(
                    AppCore.AppId,
                    this.AlteaUser.Name,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    id.Value))
            {
                levelId = id.Value;
            }
            else
            {
                levelId = this.AlteaUser.Level.Id;
            }

            Level level;
            AlteaCache.GetOrInsert(
                "LEVEL_" + levelId,
                true,
                () => AppService.GetLevel(this.AlteaUser.From, levelId),
                AlteaCache.Scope.Instance,
                AlteaCache.Term.Largest,
                out level);

            DesksExamsHomeModel model = new DesksExamsHomeModel
            {
                Level = level
            };

            return this.View("Exams", model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult List(int level)
        {
            if (!UserService.UserHasLevel(
                    AppCore.AppId,
                    this.AlteaUser.Name,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    level))
            {
                return this.JsonNet(null);
            }

            int autoblockDays = Convert.ToInt32(this.AlteaUser["desks_autounblock_days"]);
            DesksService.AutoUnblock(this.AlteaUser.Id, level, autoblockDays);
            DesksService.CheckLastBlock(this.AlteaUser.Id, level, autoblockDays);

            DesksExamList list;
            AlteaCache.GetOrInsert(
                "DESKS_EXAMS_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                true,
                () => DesksService.GetExamsList(this.AlteaUser.From, level, DesksExamQuestionType.Student),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out list);
            
            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Assignments(int level)
        {
            if (!UserService.UserHasLevel(
                    AppCore.AppId,
                    this.AlteaUser.Name,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    level))
            {
                return this.JsonNet(null);
            }

            IEnumerable<IDesksExamAssignment> assignments = DesksService.GetExamAssignments(this.AlteaUser.Id, level);
            return this.JsonNet(assignments);
        }

        [HttpGet]
        public ActionResult Part(int level, int part, int vocabulary)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_exams_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            int autoblockDays = Convert.ToInt32(this.AlteaUser["desks_autounblock_days"]);
            DesksService.CheckLastBlock(this.AlteaUser.Id, level, autoblockDays);

            long id = DesksService.GetExamsAssignmentId(this.AlteaUser.Id, level, part, vocabulary != 0, allowLocal, allowRemote);
            if (id == 0)
            {
                return level == this.AlteaUser.Level.Id
                    ? this.RedirectToAction("Index", "Exams", new { area = "Desks" })
                    : this.RedirectToAction("Index", "Exams", new { area = "Desks", id = level });
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