namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.WiseTank;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models;
    using Altea.Models.WiseTank;

    using Heracles.Models.WiseTank;

    public abstract partial class WiseTankService : Service<IWiseTankChannel>
    {
        public static void CheckDefaultTimelines(Guid userId, Guid appId, Language language, int offsetDate)
        {
            UserDataBasicModel model = new UserDataBasicModel
                {
                    UserId = userId,
                    AppId = appId,
                    LanguageFrom = language,
                    OffsetDate = offsetDate
                };

            WiseTankService.Execute("CheckDefaultTimelines", model);
        }

        public static IEnumerable<TankTimeline> GetTimelines(Guid userId, Guid appId, Language language)
        {
            return GetAreaTimelines(userId, appId, language, null);
        }

        public static IEnumerable<TankTimeline> GetAreaTimelines(Guid userId, Guid appId, Language language, TankArea area)
        {
            return GetAreaTimelines(userId, appId, language, (TankArea?)area);
        }

        private static IEnumerable<TankTimeline> GetAreaTimelines(Guid userId, Guid appId, Language language, TankArea? area)
        {
            Dictionary<Guid, TankTimeline> timelines = new Dictionary<Guid, TankTimeline>();
            List<TankTimeline> parentTimelines = new List<TankTimeline>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetAreaTimelines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@area_id",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    area);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                    {
                        while (reader.Read())
                        {
                            Guid id = (Guid)reader["id"];

                            TankTimeline timeline = new TankTimeline
                            {
                                Id = id,
                                Name = (string)reader["name"],
                                Description = reader["description"] as string,
                                /* Image */
                                Area = (TankArea)reader["area"],
                                Category = reader["category"] as int?,
                                IsAppCategory = reader["is_app_category"] as bool?,
                                Language = ((int)reader["language"]).ParseWordLanguageDatabaseId(),
                                AccessType = (TankAccessType)reader["access_permission"],
                                CreatedById = reader["created_by"] as Guid?,
                                CreateDate = (DateTime)reader["create_date"],
                                UserEditable = (bool)reader["user_editable"],
                                AreaDefault = (bool)reader["area_default"],
                                ModeratedArticles = (bool)reader["moderated_articles"],
                                ModeratedComments = (bool)reader["moderated_comments"],
                                WriteOwnArticles = (bool)reader["write_own_articles"],
                                Role = (int)reader["role"],
                                PermissionType = (int)reader["permission_type"],
                                CustomPermission = (bool)reader["custom_permission"],
                                DefaultPermission = (bool)reader["default_permission"],
                                Position = (int)reader["position"],
                                RefreshRate = reader["refresh_rate"] as int?,
                                BoxesWidth = reader["boxes_width"] as int?,
                                UserThinktank = reader["user_thinktank"] as int?,
                                ThinktankLevel = reader["thinktank_level"] as int?,
                                Permissions = new TankPermissions
                                    {
                                        ListUsers = reader["list_users"] as bool?,
                                        InviteUsers = reader["invite_users"] as bool?,
                                        ReadArticles = reader["read_articles"] as bool?,
                                        WriteArticles = reader["write_articles"] as bool?,
                                        UnmoderatedArticles = reader["unmoderated_articles"] as bool?,
                                        VoteArticles = reader["vote_articles"] as bool?,
                                        AssignArticles = reader["assign_articles"] as bool?,
                                        ApproveArticles = reader["approve_articles"] as bool?,
                                        DeleteArticles = reader["delete_articles"] as bool?,
                                        ReadComments = reader["read_comments"] as bool?,
                                        WriteComments = reader["write_comments"] as bool?,
                                        WritePrivateComments = reader["write_private_comments"] as bool?,
                                        UnmoderatedComments = reader["unmoderated_comments"] as bool?,
                                        ApproveComments = reader["approve_comments"] as bool?,
                                        DeleteComments = reader["delete_comments"] as bool?,
                                        HideAuthor = reader["hide_author"] as bool?,
                                        CreateSubTimelines = reader["create_subtimelines"] as bool?,
                                        OverrideOwnArticles = reader["override_own_articles"] as bool?
                                    }
                            };

                            Guid? parent = reader["parent"] as Guid?;
                            if (parent.HasValue)
                            {
                                TankTimeline parentTimeline;
                                timelines.TryGetValue(parent.Value, out parentTimeline);

                                if (parentTimeline != null)
                                {
                                    if (parentTimeline.Children == null)
                                    {
                                        parentTimeline.Children = new List<TankTimeline>();
                                    }

                                    ((List<TankTimeline>)parentTimeline.Children).Add(timeline);
                                }
                            }
                            else
                            {
                                parentTimelines.Add(timeline);
                            }

                            timelines.Add(id, timeline);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            Guid timeline = (Guid)reader["timeline"];
                            int role = (int)reader["role"];
                            int permissionType = (int)reader["permission_type"];

                            TankTimeline tl = null;
                            if (timelines.TryGetValue(timeline, out tl))
                            {
                                Dictionary<int, int> permissionTypes;
                                if (tl.PermissionTypes == null)
                                {
                                    permissionTypes = new Dictionary<int, int>();
                                    tl.PermissionTypes = permissionTypes;
                                }
                                else
                                {
                                    permissionTypes = (Dictionary<int, int>)tl.PermissionTypes;
                                }

                                permissionTypes.Add(role, permissionType);
                            }
                        }
                    });
            }

            return parentTimelines;
        }

        public static WiseTankStreamBoxDataModel GetTimelineData(
            Guid userId,
            Guid appId,
            Language language,
            Guid timeline,
            int numArticles,
            long[] lastMessage,
            int direction)
        {
            List<TankStreamArticle> articles = new List<TankStreamArticle>();
            int totalArticles = 0;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetTimelineArticles]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@num_articles",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    numArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@last_article",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    lastMessage != null && lastMessage.Length > 0 ? lastMessage[0] : (long?)null);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@last_article_bottom",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    lastMessage != null && lastMessage.Length == 2 ? lastMessage[1] : (long?)null);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@direction",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    direction);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                TankStreamArticle article = GetArticle(reader);
                                articles.Add(article);
                            }

                            reader.NextResult();
                            reader.Read();
                            totalArticles = (int)reader["total_articles"];
                        });
            }

            WiseTankStreamBoxDataModel model = new WiseTankStreamBoxDataModel
                {
                    Articles = articles,
                    ArticleCount = totalArticles
                };

            return model;
        }

        public static WiseTankUserDataModel GetTimelineUserData(
            Guid userId,
            Guid appId,
            Language language,
            Guid timeline)
        {
            Dictionary<Guid, IEnumerable<TankTimelineUser>> users = new Dictionary<Guid, IEnumerable<TankTimelineUser>>();
            Dictionary<Guid, bool> hasPermissions = new Dictionary<Guid, bool>();
            Dictionary<int, int> permissions = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetTimelineUserData]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    timeline);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                Guid timelineId = (Guid)reader["timeline"];
                                bool hasPermission = (bool)reader["list_users_permission"];

                                List<TankTimelineUser> timelineUsers;
                                if (!users.TryGetTypedValue(timelineId, out timelineUsers))
                                {
                                    timelineUsers = new List<TankTimelineUser>();
                                    users.Add(timelineId, timelineUsers);
                                    hasPermissions.Add(timelineId, hasPermission);
                                }

                                TankTimelineUser userData = new TankTimelineUser
                                    {
                                        UserId = (Guid)reader["user"],
                                        Role = (int)reader["role"],
                                        PermissionType = reader["permission_type"] as int?,
                                        CustomPermission = reader["custom_permission"] as bool?,
                                        DefaultPermission = reader["default_permission"] as bool?,
                                        ThinktankLevel = reader["thinktank_level"] as int?,
                                        Permissions =
                                            hasPermission
                                                ? new TankPermissions
                                                    {
                                                        ListUsers = reader["list_users"] as bool?,
                                                        InviteUsers = reader["invite_users"] as bool?,
                                                        ReadArticles = reader["read_articles"] as bool?,
                                                        WriteArticles = reader["write_articles"] as bool?,
                                                        UnmoderatedArticles = reader["unmoderated_articles"] as bool?,
                                                        VoteArticles = reader["vote_articles"] as bool?,
                                                        AssignArticles = reader["assign_articles"] as bool?,
                                                        ApproveArticles = reader["approve_articles"] as bool?,
                                                        DeleteArticles = reader["delete_articles"] as bool?,
                                                        ReadComments = reader["read_comments"] as bool?,
                                                        WriteComments = reader["write_comments"] as bool?,
                                                        WritePrivateComments = reader["write_private_comments"] as bool?,
                                                        UnmoderatedComments = reader["unmoderated_comments"] as bool?,
                                                        ApproveComments = reader["approve_comments"] as bool?,
                                                        DeleteComments = reader["delete_comments"] as bool?,
                                                        HideAuthor = reader["hide_author"] as bool?,
                                                        CreateSubTimelines = reader["create_subtimelines"] as bool?,
                                                        OverrideOwnArticles = reader["override_own_articles"] as bool?
                                                    }
                                                : null
                                    };

                                timelineUsers.Add(userData);
                            }

                            reader.NextResult();
                            permissions = new Dictionary<int, int>();

                            while (reader.Read())
                            {
                                permissions.Add((int)reader["role"], (int)reader["permission_type"]);
                            }
                    });
            }

            WiseTankUserDataModel model = new WiseTankUserDataModel
                {
                    UserData = users,
                    HasPermissions = hasPermissions,
                    Permissions = permissions
                };

            return model;
        }

        public static WiseTankError AddTimelineUser(
            Guid userId,
            Guid appId,
            Language language,
            Guid timelineId,
            Guid newUserId,
            int thinktankLevel,
            int role,
            int permissions)
        {
            WiseTankAddTimelineUserModel model = new WiseTankAddTimelineUserModel
            {
                User = userId,
                App = appId,
                Language = language,
                Timeline = timelineId,
                NewUser = newUserId,
                ThinkTankLevel = thinktankLevel,
                Role = role,
                Permissions = permissions
            };

            WiseTankError error = WiseTankService.Execute<WiseTankError>("AddTimelineUser", model);
            return error;
        }

        public static WiseTankError EditTimelineUser(
            Guid userId,
            Guid appId,
            Language language,
            Guid timelineId,
            Guid editUserId,
            int thinktankLevel,
            int role,
            int permissions)
        {
            WiseTankAddTimelineUserModel model = new WiseTankAddTimelineUserModel
            {
                User = userId,
                App = appId,
                Language = language,
                Timeline = timelineId,
                NewUser = editUserId,
                ThinkTankLevel = thinktankLevel,
                Role = role,
                Permissions = permissions
            };

            WiseTankError error = WiseTankService.Execute<WiseTankError>("EditTimelineUser", model);
            return error;
        }

        public static Guid CreateTimeline(
            Guid userId,
            Guid appId,
            Language language,
            string name,
            string description,
            TankAccessType accessType,
            bool moderatedArticles,
            bool moderatedComments,
            bool writeOwnArticles,
            IDictionary<int, int> permissionTypes,
            TankArea area,
            int offsetDate)
        {
            WiseTankCreateTimelineModel model = new WiseTankCreateTimelineModel
                {
                    User = userId,
                    App = appId,
                    Language = language,
                    Name = name,
                    Description = description,
                    AccessType = accessType,
                    ModeratedArticles = moderatedArticles,
                    ModeratedComments = moderatedComments,
                    WriteOwnArticles = writeOwnArticles,
                    PermissionTypes = permissionTypes,
                    Area = area,
                    OffsetDate = offsetDate
                };

            Guid timelineId = WiseTankService.Execute<Guid>("CreateTimeline", model);
            return timelineId;
        }

        public static WiseTankError EditTimeline(
            Guid userId,
            Guid appId,
            Language language,
            Guid timelineId,
            string name,
            string description,
            TankAccessType accessType,
            bool moderatedArticles,
            bool moderatedComments,
            bool writeOwnArticles,
            IDictionary<int, int> permissionTypes)
        {
            WiseTankEditTimelineModel model = new WiseTankEditTimelineModel
                {
                    User = userId,
                    App = appId,
                    Language = language,
                    Timeline = timelineId,
                    Name = name,
                    Description = description,
                    AccessType = accessType,
                    ModeratedArticles = moderatedArticles,
                    ModeratedComments = moderatedComments,
                    WriteOwnArticles = writeOwnArticles,
                    PermissionTypes = permissionTypes
                };

            WiseTankError status = WiseTankService.Execute<WiseTankError>("EditTimeline", model);
            return status;
        }

        public static void SetTimelineArea(Guid userId, Guid appId, Language language, Guid timeline, TankArea area)
        {
            WiseTankSetTimelineAreaModel model = new WiseTankSetTimelineAreaModel
                {
                    User = userId,
                    App = appId,
                    Language = language,
                    Timeline = timeline,
                    Area = area 
                };

            WiseTankService.Execute("SetTimelineArea", model);
        }

        public static Guid AddTimelineColumn(
            Guid userId,
            Guid appId,
            Language language,
            Guid timeline,
            string name,
            int thinktankLevel,
            TankAccessType accessType,
            bool moderatedArticles,
            bool moderatedComments,
            bool writeOwnArticles,
            int offsetDate)
        {
            WiseTankAddTimelineColumnModel model = new WiseTankAddTimelineColumnModel
                {
                    User = userId,
                    App = appId,
                    Language = language,
                    Timeline = timeline,
                    Name = name,
                    ThinkTankLevel = thinktankLevel,
                    AccessType = accessType,
                    ModeratedArticles = moderatedArticles,
                    ModeratedComments = moderatedComments,
                    WriteOwnArticles = writeOwnArticles,
                    OffsetDate = offsetDate
                };

            Guid columnId = WiseTankService.Execute<Guid>("AddTimelineColumn", model);
            return columnId;
        }

        public static void EditGroupBoxWidth(Guid userId, Guid appId, Language language, Guid timelineId, int width)
        {
            WiseTankBoxWidthModel model = new WiseTankBoxWidthModel
            {
                User = userId,
                App = appId,
                Language = language,
                Data = timelineId,
                Width = width
            };

            WiseTankService.Execute("EditGroupBoxWidth", model);
        }
    }
}
