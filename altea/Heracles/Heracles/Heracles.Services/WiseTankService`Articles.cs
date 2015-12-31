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

    public abstract partial class WiseTankService : Service<IWiseTankChannel>
    {
        public static WiseTankError CreateArticle(
            Guid userId,
            Guid appId,
            Language language,
            Guid timeline,
            TankOrigin origin,
            string reference,
            string source,
            string favicon,
            string name,
            string lead,
            string description,
            string image,
            int offsetDate)
        {
            WiseTankCreateArticleModel model = new WiseTankCreateArticleModel
                {
                    UserId = userId,
                    AppId = appId,
                    Language = language,
                    Timeline = timeline,
                    Origin = origin,
                    Reference = reference,
                    Source = source,
                    Favicon = favicon,
                    Name = name,
                    Lead = lead,
                    Description = description,
                    Image = image,
                    OffsetDate = offsetDate
                };

            WiseTankError error = Execute<WiseTankError>("CreateArticle", model);
            return error;
        }

        public static int? Vote(Guid userId, Guid appId, Language language, long articleId, bool? upvote)
        {
            WiseTankVoteModel model = new WiseTankVoteModel
                                          {
                                              User = userId,
                                              App = appId,
                                              Language = language,
                                              Article = articleId,
                                              UpVote = upvote
                                          };

            int? votes = Execute<int?>("Vote", model);
            return votes;
        }

        public static decimal? Karma(Guid userId, Guid appId, Language language, long articleId, int? userKarma)
        {
            WiseTankKarmaModel model = new WiseTankKarmaModel
                {
                    User = userId,
                    App = appId,
                    Language = language,
                    Article = articleId,
                    Karma = userKarma
                };

            decimal? karma = Execute<decimal?>("Karma", model);
            return karma;
        }

        public static byte[] GetArticleImage(Guid userId, Guid appId, Language language, long articleId)
        {
            byte[] image;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetArticleImage]"))
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
                    "@article_id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    articleId);

                image = SqlDatabaseManager.ExecuteScalar(command, SqlConnectionString.DataWarehouse) as byte[];
            }

            return image;
        }

        private static TankStreamArticle GetArticle(SqlDataReader reader)
        {
            TankStreamArticle article = new TankStreamArticle
            {
                Id = (long)reader["id"],
                UserId = (Guid)reader["user"],
                AuthorId = (Guid)reader["author"],
                Name = reader["name"] as string,
                Description = reader["description"] as string,
                HasImage = (bool)reader["has_image"],
                Source = reader["source"] as string,
                FaviconArray = reader["favicon"] as byte[],
                Category = reader["category"] as int?,
                IsAppCategory = reader["is_app_category"] as bool?,
                Lead = reader["lead"] as string,
                Origin = (TankOrigin)reader["origin"],
                Reference = reader["reference"] as string,
                AddDate = DateTime.SpecifyKind((DateTime)reader["add_date"], DateTimeKind.Utc),
                UpVotes = (int)reader["upvotes"],
                DownVotes = (int)reader["downvotes"],
                Karma = (decimal)reader["karma"],
                Approved = reader["approved"] as bool?,
                Deleted = reader["deleted"] as bool?,
                ThinktankLevel = reader["thinktank_level"] as int?,
                UserUpvoted = reader["user_upvoted"] as bool?,
                UserKarmaed = reader["user_karmaed"] as decimal?,
                CanVote = (bool)reader["can_vote"],
                AssignedById = reader["assigned_by"] as Guid?,
                AssignMessage = reader["assign_message"] as string,
                ArticleNumber = (long)reader["article_number"]
            };

            return article;
        }

        public static IEnumerable<TankArticleComment> GetArticleComments(
            Guid userId,
            Guid appId,
            Guid timelineId,
            Guid articleId,
            int numComments,
            Guid? lastComment)
        {
            Dictionary<long, TankArticleComment> comments = new Dictionary<long, TankArticleComment>();
            List<TankArticleComment> parentComments = new List<TankArticleComment>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetArticleComments]"))
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
                    "@article_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    articleId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                    {
                        while (reader.Read())
                        {
                            long id = (long)reader["id"];

                            TankArticleComment comment = new TankArticleComment
                            {
                                Id = id,
                                User = (string)reader["user"],
                                Comment = (string)reader["comment"],
                                AddDate = (DateTime)reader["add_date"],
                                UpVotes = (int)reader["upvotes"],
                                DownVotes = (int)reader["downvotes"],
                                Approved = reader["approved"] as bool?,
                                Deleted = reader["deleted"] as bool?,
                                CommentNumber = (int)reader["comment_number"]
                            };

                            long? parent = reader["parent"] as long?;
                            if (parent.HasValue)
                            {
                                TankArticleComment parentComment;
                                comments.TryGetValue(parent.Value, out parentComment);

                                if (parentComment != null)
                                {
                                    if (parentComment.Children == null)
                                    {
                                        parentComment.Children = new List<TankArticleComment>();
                                    }

                                    ((List<TankArticleComment>)parentComment.Children).Add(comment);
                                }
                            }
                            else
                            {
                                parentComments.Add(comment);
                            }

                            comments.Add(id, comment);
                        }
                    });
            }

            return parentComments;
        }
    }
}
