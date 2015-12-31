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
    using Altea.Models.WiseTank;

    using Heracles.Models.WiseTank;

    public abstract partial class WiseTankService : Service<IWiseTankChannel>
    {
        public static IEnumerable<TankStream> GetStreamData(Guid userId, Language language)
        {
            List<TankStream> streams = new List<TankStream>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetUserStreams]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            TankStream lastStream = null;

                            while (reader.Read())
                            {
                                Guid id = (Guid)reader["id"];

                                if (lastStream == null || lastStream.Id != id)
                                {
                                    lastStream = new TankStream
                                        {
                                            Id = id,
                                            Name = (string)reader["name"],
                                            Position = (int)reader["position"],
                                            BoxesWidth = (int)reader["boxes_width"],
                                            Boxes = new List<TankBox>(),
                                            RefreshRate = (int)reader["refresh_rate"]
                                        };

                                    streams.Add(lastStream);
                                }

                                if (!(reader["box_position"] is DBNull))
                                {
                                    TankBox box = new TankBox
                                        {
                                            Id = (Guid)reader["box_id"],
                                            Name = reader["box_name"] as string,
                                            Type = (TankBoxType)reader["box_type"],
                                            Position = (int)reader["box_position"],
                                            Query = (string)reader["query_name"]
                                        };

                                    ((List<TankBox>)lastStream.Boxes).Add(box);
                                }
                            }
                        });
            }

            return streams;
        }

        public static WiseTankStreamBoxDataModel GetStreamBoxData(
            Guid userId,
            Guid appId,
            Language language,
            Guid streamId,
            Guid boxId, 
            int numArticles,
            long[] lastMessage,
            int direction)
        {
            List<TankStreamArticle> articles = new List<TankStreamArticle>();
            int totalArticles = 0;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetStreamBoxData]"))
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
                    "@stream_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    streamId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@box_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    boxId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@num_articles",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    numArticles);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@last_message",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    lastMessage != null && lastMessage.Length > 0 ? lastMessage[0] : (long?)null);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@last_message_bottom",
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

        public static IEnumerable<TankStreamArticle> GetTimelineArticles(
            Guid userId,
            Guid appId,
            Guid timelineId,
            int numArticles,
            Guid? lastArticle)
        {
            List<TankStreamArticle> articles = new List<TankStreamArticle>();

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
                    timelineId);

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
                    SqlDbType.UniqueIdentifier,
                    lastArticle);

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
                        });
            }

            return articles;
        }

        public static void EditBoxWidth(Guid userId, Language language, Guid streamId, int width)
        {
            WiseTankBoxWidthModel model = new WiseTankBoxWidthModel
                {
                    User = userId,
                    Language = language,
                    Data = streamId,
                    Width = width
                };

            WiseTankService.Execute("EditBoxWidth", model);
        }

        public static void EditBoxRefreshRate(Guid userId, Language language, Guid streamId, int refreshRate)
        {
            WiseTankBoxRefreshRateModel model = new WiseTankBoxRefreshRateModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId,
                    RefreshRate = refreshRate
                };

            WiseTankService.Execute("EditBoxRefreshRate", model);
        }

        public static Guid CreateStream(Guid userId, Language language, string name)
        {
            WiseTankCreateModel model = new WiseTankCreateModel
                {
                    User = userId,
                    Language = language,
                    Name = name
                };

            Guid stream = WiseTankService.Execute<Guid>("CreateStream", model);
            return stream;
        }

        public static WiseTankError EditStreamName(Guid userId, Language language, Guid streamId, string name)
        {
            WiseTankEditStreamModel model = new WiseTankEditStreamModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId,
                    Name = name
                };

            WiseTankError status = WiseTankService.Execute<WiseTankError>("EditStreamName", model);
            return status;
        }

        public static void RepositionStream(Guid userId, Language language, Guid streamId, int position)
        {
            WiseTankRepositionStreamModel model = new WiseTankRepositionStreamModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId,
                    Position = position
                };

            WiseTankService.Execute("RepositionStream", model);
        }

        public static WiseTankError DeleteStream(Guid userId, Language language, Guid streamId)
        {
            WiseTankStreamModel model = new WiseTankStreamModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId
                };

            WiseTankError status = WiseTankService.Execute<WiseTankError>("DeleteStream", model);
            return status;
        }

        public static Guid CreateBox(Guid userId, Language language, Guid streamId, TankBoxType type, string query)
        {
            WiseTankCreateBoxModel model = new WiseTankCreateBoxModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId,
                    BoxType = type,
                    Query = query
                };

            Guid box = WiseTankService.Execute<Guid>("CreateBox", model);
            return box;
        }

        public static void RepositionBox(
            Guid userId,
            Language language,
            Guid streamId,
            Guid boxId,
            int position)
        {
            WiseTankRepositionBoxModel model = new WiseTankRepositionBoxModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId,
                    Box = boxId,
                    Position = position
                };

            WiseTankService.Execute("RepositionBox", model);
        }

        public static WiseTankError DeleteBox(Guid userId, Language language, Guid streamId, Guid boxId)
        {
            WiseTankBoxModel model = new WiseTankBoxModel
                {
                    User = userId,
                    Language = language,
                    Stream = streamId,
                    Box = boxId
                };

            WiseTankError error = WiseTankService.Execute<WiseTankError>("DeleteBox", model);
            return error;
        }
    }
}
