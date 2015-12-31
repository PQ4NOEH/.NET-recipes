namespace Heracles.Web.Areas.Dean.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.Dean;
    using Altea.Classes.Desks;
    using Altea.Classes.Group;
    using Altea.Classes.Lists;
    using Altea.Classes.ProDesks;
    using Altea.Classes.WiseNet;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.Dean;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Roles = "Teacher", Modules = "Dean")]
    public class IndexController : AlteaController
    {
        [HttpGet]
        public ActionResult Index(Guid? id, DeanMemberType? type, int? pro, int? level, int? subLevel)
        {
            string language = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

            IEnumerable<Level> levels;
            IEnumerable<ProLevel> proLevels;

            AlteaCache.GetOrInsert(
                "LEVELS_" + language,
                true,
                () => AppService.GetLevels(this.AlteaUser.From),
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Largest,
                out levels);

            string columns = AppCore.AppSettings["dean_columns_" + language];
            string proColumns;

            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());

            string booksStudentColumns = RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "User Dean", isLocal)
                ? AppCore.AppSettings["books_columns_student_" + language]
                : null;

            string extraTeacherColumns = RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "Group Dean", isLocal)
                ? AppCore.AppSettings["extra_columns_teacher_" + language]
                : null;

            if (RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "PEC", isLocal))
            {
                AlteaCache.GetOrInsert(
                    "PROLEVELS_" + language,
                    true,
                    () => ProDesksService.GetLevels(this.AlteaUser.From),
                    AlteaCache.Scope.Altea,
                    AlteaCache.Term.Largest,
                    out proLevels);

                proColumns = AppCore.AppSettings["prodesks_columns_" + language];
            }
            else
            {
                proLevels = null;
                proColumns = null;
            }

            IEnumerable<DesksIndexArea> indexAreas;
            AlteaCache.GetOrInsert(
                "DESKS_INDEX_Areas_" + language,
                true,
                () => DesksService.GetIndexAreas(this.AlteaUser.From),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out indexAreas);

            DeanIndexModel model = new DeanIndexModel
                {
                    Sticky = new DeanIndexStickyModel
                        {
                            Id = id,
                            Type = type,
                            Pro = pro != 0,
                            Level = level,
                            SubLevel = subLevel
                        },
                    Levels = levels,
                    ProLevels = proLevels,
                    Columns = columns.FromJson<dynamic>(),
                    IndexAreas = indexAreas,
                    BooksStudentColumns = booksStudentColumns.FromJson<dynamic>(),
                    ExtraTeacherColumns = extraTeacherColumns.FromJson<dynamic>(),
                    ProColumns = proColumns.FromJson<dynamic>()
                };

            return this.View(model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetMembers()
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());

            bool hasUserDean = RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "User Dean", isLocal);
            bool hasGroupDean = RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "Group Dean", isLocal);

            IEnumerable<DeanUser> users = hasUserDean
                ? DeanService.GetUsers(
                    this.AlteaUser.Id,
                    AppCore.AppId,
                    this.AlteaUser.From,
                    this.AlteaUser.To)
                : null;

            if (users != null)
            {
                string[] userRoles = RoleProvider.GetRolesForUser(this.AlteaUser.Name);

                if (!userRoles.Contains("Developer"))
                {
                    List<string> elevatedUsers = new List<string>();
                    elevatedUsers.AddRange(RoleProvider.GetUsersInRole("Developer"));

                    DeanUser[] noLevelUsers = users.Where(x => !x.Levels.Any()).ToArray();
                    if (!userRoles.Contains("Admin"))
                    {
                        if (noLevelUsers.Length != 0)
                        {
                            elevatedUsers.AddRange(RoleProvider.GetUsersInRole("Teacher"));
                            elevatedUsers.AddRange(RoleProvider.GetUsersInRole("Admin"));
                        }
                    }

                    IEnumerable<DeanUser> excludedUsers = noLevelUsers.Where(x => elevatedUsers.Contains(x.UserName));

                    if (!userRoles.Contains("Admin"))
                    {
                        excludedUsers =
                            excludedUsers.Union(
                                new DeanUser[1] { users.SingleOrDefault(x => x.UserName == this.AlteaUser.Name) });
                    }

                    DeanUser[] excluded = excludedUsers.ToArray();

                    if (excluded.Length != 0)
                    {
                        users = users.Except(excluded).ToArray();
                    }
                }
            }

            IEnumerable<DeanGroup> groups = hasGroupDean
                ? DeanService.GetGroups(
                    this.AlteaUser.Id,
                    AppCore.AppId,
                    this.AlteaUser.From,
                    this.AlteaUser.To)
                : null;

            DeanMembersModel model = new DeanMembersModel
                {
                    Users = users,
                    Groups = groups
                };

            return this.JsonNet(model);
        }

        #region Members
        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean")]
        public ActionResult GetUserData(Guid id, int level)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            DeanUserDataModel model = this.GetUserBasicDataModel(id, isLocal);

            IEnumerable<DesksIndexAssignment> desksIndex = DesksService.GetIndexAssignments(id, level);
            model.DesksIndexAssignments = desksIndex;

            IEnumerable<IDesksExamAssignment> desksExams = DesksService.GetExamAssignments(id, level);
            model.DesksExamsAssignments = desksExams;

            IEnumerable<DesksBookAssignment> desksBooks = DesksService.GetBookAssignments(id, this.AlteaUser.From);
            model.DesksBooksAssignments = desksBooks;

            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:User Dean,Dean:PEC")]
        public ActionResult GetUserProData(Guid id, int level, int sublevel)
        {
            bool isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
            DeanUserDataModel model = this.GetUserBasicDataModel(id, isLocal);

            IEnumerable<IProDesksAssignment> proDesks = ProDesksService.GetAssignments(id, level, sublevel);
            model.ProDesksAssignments = proDesks;

            return this.JsonNet(model);
        }

        private DeanUserDataModel GetUserBasicDataModel(Guid id, bool isLocal)
        {
            IEnumerable<WiseNetSearchEngine> searchEngines =
                RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "WiseNet", isLocal)
                    ? WiseNetService.GetUserSearchEngines(id, this.AlteaUser.From)
                    : null;

            IEnumerable<WiseNetCarousel> magazines =
                RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "WiseNet", isLocal)
                    ? WiseNetService.GetUserMagazines(id, this.AlteaUser.From)
                    : null;

            IEnumerable<AssignedList> lists =
                RoleProvider.IsUserInPermission(this.AlteaUser.Name, "Dean", "WiseNet", isLocal)
                    ? ListsService.GetUserLists(id, ListType.All, this.AlteaUser.From)
                    : null;

            DeanUserDataModel model = new DeanUserDataModel
            {
                SearchEngines = searchEngines,
                Magazines = magazines,
                Lists = lists
            };

            return model;
        }

        [HttpPost, OnlyAjax, AlteaAuth(Roles = "Teacher", Modules = "Dean:Group Dean")]
        public ActionResult GetGroupData(Guid id, int level)
        {
            AlteaGroupPlanning planning = GroupService.GetPlanning(id, level);
            IEnumerable<DesksIndexAssignment> desksIndex = DesksService.GetIndexGroupAssignments(id, level);
            IEnumerable<IDesksExamAssignment> desksExams = DesksService.GetExamGroupAssignments(id, level);
            IEnumerable<DesksExtraAssignment> desksExtra = DesksService.GetExtraGroupAssignments(id, level);

            DeanGroupDataModel model = new DeanGroupDataModel
            {
                Planning = planning,
                DesksIndexAssignments = desksIndex,
                DesksExamsAssignments = desksExams,
                DesksExtraAssignments = desksExtra
            };

            return this.JsonNet(model);
        }
        #endregion

        #region Data
        [HttpPost, OnlyAjax]
        public ActionResult GetDesksIndex(int level, DesksIndexQuestionType type)
        {
            DesksIndexList list;

            switch (type)
            {
                case DesksIndexQuestionType.Student:
                    AlteaCache.GetOrInsert(
                        "DESKS_INDEX_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                        true,
                        () => DesksService.GetIndexList(this.AlteaUser.From, level, DesksIndexQuestionType.Student),
                        AlteaCache.Scope.Role,
                        AlteaCache.Term.Largest,
                        out list);
                    break;

                case DesksIndexQuestionType.Teacher:
                    AlteaCache.GetOrInsert(
                        "DESKS_INDEX_TEACHER_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                        true,
                        () => DesksService.GetIndexList(this.AlteaUser.From, level, DesksIndexQuestionType.Teacher),
                        AlteaCache.Scope.Role,
                        AlteaCache.Term.Largest,
                        out list);
                    break;

                default:
                    list = null;
                    break;
            }

            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetDesksExams(int level, DesksExamQuestionType type)
        {
            DesksExamList list;

            switch (type)
            {
                case DesksExamQuestionType.Student:
                    AlteaCache.GetOrInsert(
                        "DESKS_EXAMS_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                        true,
                        () => DesksService.GetExamsList(this.AlteaUser.From, level, DesksExamQuestionType.Student),
                        AlteaCache.Scope.Role,
                        AlteaCache.Term.Largest,
                        out list);
                    break;

                case DesksExamQuestionType.Teacher:
                    AlteaCache.GetOrInsert(
                        "DESKS_EXAMS_TEACHER_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                        true,
                        () => DesksService.GetExamsList(this.AlteaUser.From, level, DesksExamQuestionType.Teacher),
                        AlteaCache.Scope.Role,
                        AlteaCache.Term.Largest,
                        out list);
                    break;

                default:
                    list = null;
                    break;
            }

            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetDesksExtra(int level)
        {
            DesksExtraList list;

            AlteaCache.GetOrInsert(
                "DESKS_EXTRA_TEACHER_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level,
                true,
                () => DesksService.GetExtraList(this.AlteaUser.From, level, DesksExtraQuestionType.Teacher),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out list);

            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetDesksBooks()
        {
            DesksBookList list;

            AlteaCache.GetOrInsert(
                "DESKS_BOOKS_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName),
                true,
                () => DesksService.GetBooksList(this.AlteaUser.From, DesksBookQuestionType.Student),
                AlteaCache.Scope.Role,
                AlteaCache.Term.Largest,
                out list);

            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetProDesks(int level, int sublevel, ProDesksQuestionType type)
        {
            ProDesksList list;

            switch (type)
            {
                case ProDesksQuestionType.ProStudent:
                    AlteaCache.GetOrInsert(
                        "PRODESKS_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level
                        + sublevel,
                        true,
                        () =>
                        ProDesksService.GetList(this.AlteaUser.From, level, sublevel, ProDesksQuestionType.ProStudent),
                        AlteaCache.Scope.Role,
                        AlteaCache.Term.Largest,
                        out list);
                    break;

                case ProDesksQuestionType.ProTeacher:
                    AlteaCache.GetOrInsert(
                        "PRODESKS_TEACHER_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName) + "_" + level
                        + sublevel,
                        true,
                        () =>
                        ProDesksService.GetList(this.AlteaUser.From, level, sublevel, ProDesksQuestionType.ProTeacher),
                        AlteaCache.Scope.Role,
                        AlteaCache.Term.Largest,
                        out list);
                    break;

                default:
                    list = null;
                    break;
            }

            return this.JsonNet(list);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetWiseNet()
        {
            string languagePrefix = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

            IEnumerable<WiseNetSearchEngine> searchEngines;
            AlteaCache.GetOrInsert(
                "WISENET_SEARCHEGINES_" + languagePrefix,
                true,
                () => WiseNetService.GetSearchEngines(this.AlteaUser.From),
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Large,
                out searchEngines);

            IEnumerable<WiseNetCarousel> magazines;
            AlteaCache.GetOrInsert(
                "WISENET_MAGAZINES_" + languagePrefix,
                true,
                () => WiseNetService.GetMagazines(this.AlteaUser.From),
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Large,
                out magazines);

            DeanWiseNetModel model = new DeanWiseNetModel
                {
                    SearchEngines = searchEngines,
                    Magazines = magazines
                };

            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public ActionResult GetLists()
        {
            string languagePrefix = this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName);

            IEnumerable<ListCategory> categories;
            AlteaCache.GetOrInsert(
                "LISTS_CATEGORIES_" + ListType.All + "_" + languagePrefix,
                true,
                () => ListsService.GetCategories(this.AlteaUser.From, ListType.All),
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Medium,
                out categories);

            IEnumerable<AlteaList> lists;
            AlteaCache.GetOrInsert(
                "LISTS_" + ListType.All + "_" + languagePrefix,
                true,
                () => ListsService.GetLists(this.AlteaUser.From, ListType.All),
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Medium,
                out lists);

            DeanListsModel model = new DeanListsModel
                {
                    Categories = categories,
                    Lists = lists
                };

            return this.JsonNet(model);
        }
        #endregion
    }
}