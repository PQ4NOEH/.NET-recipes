namespace Heracles.Web.Areas.Desks.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.Desks;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.Desks;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;
    using Heracles.Web.Areas.Desks.Resources;

    [AlteaAuth(Modules = "DesksIndex", MustHaveLevel = true)]
    public class IndexController : AlteaController
    {
        [HttpGet]
        public ActionResult Index(int? id)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

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

            if (levelId == 0)
            {
                return this.RedirectToAction("Index", "Home", new { area = string.Empty, id = (int?)null });
            }

            Level level;
            AlteaCache.GetOrInsert(
                "LEVEL_" + levelId,
                true,
                () => AppService.GetLevel(this.AlteaUser.From, levelId),
                AlteaCache.Scope.Instance,
                AlteaCache.Term.Largest,
                out level);

            IEnumerable<DesksIndexArea> areas;
            AlteaCache.GetOrInsert(
                "DESKS_INDEX_Areas_" + language,
                true,
                () => DesksService.GetIndexAreas(this.AlteaUser.From),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out areas);

            DesksIndexHomeModel model = new DesksIndexHomeModel
                {
                    Level = level,
                    Areas = areas
                };
            
            return this.View("Index", model);
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

            DesksIndexList list;
            AlteaCache.GetOrInsert(
                "DESKS_INDEX_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                true,
                () => DesksService.GetIndexList(this.AlteaUser.From, level, DesksIndexQuestionType.Student),
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

            IEnumerable<DesksIndexAssignment> assignments = DesksService.GetIndexAssignments(this.AlteaUser.Id, level);
            return this.JsonNet(assignments);
        }

        [HttpGet]
        public ActionResult Part(int level, DesksIndexExerciseType type, int area, int subject)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            int autoblockDays = Convert.ToInt32(this.AlteaUser["desks_autounblock_days"]);
            DesksService.CheckLastBlock(this.AlteaUser.Id, level, autoblockDays);

            long id = DesksService.GetIndexAssignmentId(this.AlteaUser.Id, level, type, area, subject, allowLocal, allowRemote);
            if (id == 0)
            {
                return level == this.AlteaUser.Level.Id
                    ? this.RedirectToAction("Index", "Index", new { area = "Desks" })
                    : this.RedirectToAction("Index", "Index", new { area = "Desks", id = level });
            }

            string subjectName;
            AlteaCache.GetOrInsert(
                "DESKS_INDEX_Subject_" + subject,
                true,
                () => DesksService.GetIndexSubjectName(this.AlteaUser.From, subject),
                AlteaCache.Scope.Instance,
                AlteaCache.Term.Largest,
                out subjectName);

            DesksExerciseModel model = new DesksExerciseModel
                {
                    Id = id,
                    Round = DesksService.GetIndexAssignmentRound(id),
                    ViewName = type.ToString().ToLowerInvariant(),
                    Title = DesksResources.ResourceManager.GetString(type.ToString()),
                    Subtitle = subjectName
                };
            return this.View("Part", model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Get(long id)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsIndexAssigned(this.AlteaUser.Id, id, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            DesksIndexData data = DesksService.GetIndexData(id, DesksIndexQuestionType.Student);

            if (data.ExerciseType == DesksIndexExerciseType.MultipleChoice)
            {
                foreach (DesksIndexQuestion question in data.Questions)
                {
                    foreach (DesksIndexAnswer answer in question.Answers)
                    {
                        answer.Valid = null;
                    }
                }
            }
            else
            {
                foreach (DesksIndexQuestion question in data.Questions)
                {
                    question.Length = question.Answers.Select(x => x.Answer.Length).Max();
                    question.Answers = null;
                }
            }

            data.Time = this.GetTimer(data.ExerciseType, data.Round, !isLocal);
            return this.JsonNet(data);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Start(long id, int offset)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsIndexAssigned(this.AlteaUser.Id, id, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            DesksIndexExerciseType exerciseType;
            int round, numQuestions;

            DesksService.GetIndexAssignmentData(id, out exerciseType, out round, out numQuestions);

            int codeValidFor = (int)Math.Ceiling(
                this.GetTimer(exerciseType, round, !isLocal)
                * numQuestions
                * DesksService.INDEX_CODE_TIME_MARGIN);

            string code = DesksService.StartIndex(id, !isLocal, offset, codeValidFor);
            return this.JsonNet(code);
        }

        private int GetTimer(DesksIndexExerciseType type, int round, bool remote)
        {
            string remoteText = remote ? "remote" : "local";
            string typeText = type.ToString().ToLowerInvariant();
            string questionTime = this.AlteaUser["desks_index_time_" + remoteText + "_" + typeText + "_" + round]
                ?? this.AlteaUser["desks_index_time_" + remoteText + "_" + typeText]
                ?? this.AlteaUser["desks_index_time_" + typeText + "_" + round]
                ?? this.AlteaUser["desks_index_time_" + typeText];

            int time = int.Parse(questionTime);
            return time;
        }

        [HttpPost, OnlyAjax]
        public ActionResult Check(long id, string code, int[] questions, string[][] answers, int executionTime)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsIndexAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            DesksCheckResult result = DesksService.CheckIndex(
                id,
                code,
                questions,
                answers,
                TimeSpan.FromSeconds(executionTime));

            return this.JsonNet(result);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Finish(long id, string code, int executionTime, int offsetDate)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsIndexAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            DesksFinishResult result = DesksService.FinishIndex(id, code, TimeSpan.FromSeconds(executionTime), offsetDate);
            return this.JsonNet(result);
        }

        [HttpGet]
        public ActionResult Analyse(long id, string code)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsIndexAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            return this.JsonNet(null);
        }

        [HttpPost, OnlyAjax]
        public ActionResult Report(long id, string code, int question, string feedback)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            bool allowLocal = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_local"] == "true";
            bool allowRemote = this.AlteaUser["desks_index_" + (isLocal ? "local" : "remote") + "_allow_remote"] == "true";

            if (!DesksService.IsIndexAssigned(this.AlteaUser.Id, id, code, allowLocal, allowRemote))
            {
                return this.JsonNet(null);
            }

            return this.JsonNet(null);
        }
    }
}