namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Desks;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Desks;

    public abstract partial class DesksService : Service<IDesksChannel>
    {
        public static DesksBookList GetBooksList(Language language, DesksBookQuestionType type)
        {
            DesksBookList list = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_BOOKS_List]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    type);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => list = DesksService.GetBooksList(reader));
            }

            return list;
        }

        internal static DesksBookList GetBooksList(SqlDataReader reader)
        {
            List<DesksBookType> types = new List<DesksBookType>();
            Dictionary<long, IDesksBookData> articles = new Dictionary<long, IDesksBookData>();
            Dictionary<long, IDesksBookData> books = new Dictionary<long, IDesksBookData>();
            Dictionary<long, IDesksBookData> collections = new Dictionary<long, IDesksBookData>();

            List<long> publications = new List<long>();

            // Types
            DesksBookType lastType = null;
            while (reader.Read())
            {
                int typeId = (int)reader["id"];
                if (lastType == null || lastType.Id != typeId)
                {
                    lastType = new DesksBookType
                        {
                            Id = typeId,
                            Name = (string)reader["name"],
                            SubTypes = new List<DesksBookType>()
                        };

                    types.Add(lastType);
                }

                int? subTypeId = reader["subtype_id"] as int?;
                if (subTypeId.HasValue)
                {
                    DesksBookType type = new DesksBookType
                        {
                            Id = subTypeId.Value,
                            Name = (string)reader["subname"]
                        };

                    ((List<DesksBookType>)lastType.SubTypes).Add(type);
                }
            }

            // Articles
            reader.NextResult();
            while (reader.Read())
            {
                DesksBookPublication article = new DesksBookPublication
                    {
                        Id = (long)reader["id"],
                        PublicationType = (DesksBookPublicationType)reader["type"],
                        ExerciseTypes = new List<int>(),
                        Levels = new List<int>(),
                        Authors = new List<string>(),
                        Publishers = new List<string>(),
                        Categories = new List<int>(),
                        Tags = new List<int>(),
                        Title = (string)reader["title"],
                        Subtitle = reader["subtitle"] as string,
                        IsTextArticle = reader["is_text_article"] as bool? ?? false,
                        IsCollectionArticle = reader["is_collection_article"] as bool? ?? false
                    };

                articles.Add(article.Id, article);
                publications.Add(article.Id);
            }

            reader.NextResult();
            DesksBookPublication lastArticle = null;
            while (reader.Read())
            {
                long id = (long)reader["publication"];
                if (lastArticle == null || lastArticle.Id != id)
                {
                    lastArticle = articles[id] as DesksBookPublication;
                }

                if (lastArticle != null)
                {
                    ((List<int>)lastArticle.ExerciseTypes).Add((int)reader["type"]);
                }
            }

            articles =
                articles.Where(x => (x.Value as DesksBookPublication).ExerciseTypes.Count() != 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            SetPublicationData(articles, reader);
            SetElementClassification(articles, reader);

            // Books
            reader.NextResult();
            while (reader.Read())
            {
                DesksBookPublication book = new DesksBookPublication
                {
                    Id = (long)reader["id"],
                    PublicationType = (DesksBookPublicationType)reader["type"],
                    Levels = new List<int>(),
                    Authors = new List<string>(),
                    Publishers = new List<string>(),
                    Categories = new List<int>(),
                    Tags = new List<int>(),
                    Title = (string)reader["title"],
                    Subtitle = reader["subtitle"] as string,
                    IsTextArticle = false,
                    IsCollectionArticle = false,
                };

                books.Add(book.Id, book);
                publications.Add(book.Id);
            }

            SetPublicationData(books, reader);
            SetElementClassification(books, reader);

            // Book Articles
            HashSet<long> bookArticles = new HashSet<long>();
            DesksBookPublication lastBook = null;
            reader.NextResult();
            while (reader.Read())
            {
                long bookId = (long)reader["book"];
                if (lastBook == null || lastBook.Id != bookId)
                {
                    lastBook = books[bookId] as DesksBookPublication;
                }

                long articleId = (long)reader["article"];
                DesksBookPublication article = articles[articleId] as DesksBookPublication;

                if (lastBook != null && article != null)
                {
                    if (lastBook.Publications == null)
                    {
                        lastBook.Publications = new Dictionary<int, DesksBookPublication>();
                    }

                    ((Dictionary<int, DesksBookPublication>)lastBook.Publications).Add((int)reader["position"], article);
                    bookArticles.Add(articleId);
                }
            }

            foreach (long article in bookArticles)
            {
                articles.Remove(article);
            }

            long[] emptyBooks = books.Values.Where(x => x.Publications == null).Select(x => x.Id).ToArray();
            foreach (long book in emptyBooks)
            {
                books.Remove(book);
                publications.Remove(book);
            }

            foreach (DesksBookPublication book in books.Values)
            {
                book.ExerciseTypes = book.Publications.SelectMany(x => x.Value.ExerciseTypes).Distinct().ToArray();
            }

            // Collections
            reader.NextResult();
            while (reader.Read())
            {
                DesksBookCollection collection = new DesksBookCollection
                        {
                            Id = (long)reader["id"],
                            Categories = new List<int>(),
                            Tags = new List<int>(),
                            Name = (string)reader["name"],
                            Publications = new Dictionary<int, DesksBookPublication>()
                        };

                    collections.Add(collection.Id, collection);
            }

            SetElementClassification(collections, reader);

            // Collection Publications
            HashSet<long> collectionArticles = new HashSet<long>();
            DesksBookCollection lastCollection = null;
            reader.NextResult();
            while (reader.Read())
            {
                long id = (long)reader["collection"];
                if (lastCollection == null || lastCollection.Id != id)
                {
                    lastCollection = collections[id] as DesksBookCollection;
                }

                long? publication = reader["publication"] as long?;
                if (publication.HasValue && articles.ContainsKey(publication.Value))
                {
                    IDesksBookData pub;
                    DesksBookPublication p;
                    if (articles.TryGetValue(publication.Value, out pub))
                    {
                        p = pub as DesksBookPublication;
                        p.PublicationFormat = DesksBookPublicationFormat.Article;
                        collectionArticles.Add(pub.Id);
                    }
                    else if (books.TryGetValue(publication.Value, out pub))
                    {
                        p = pub as DesksBookPublication;
                        p.PublicationFormat = DesksBookPublicationFormat.Book;
                    }
                    else
                    {
                        continue;
                    }

                    ((Dictionary<int, DesksBookPublication>)lastCollection.Publications).Add((int)reader["position"], p);
                }
            }

            foreach (long article in collectionArticles)
            {
                articles.Remove(article);
            }

            long[] emptyCollections = collections.Values.Where(x => !x.Publications.Any()).Select(x => x.Id).ToArray();
            foreach (long collection in emptyCollections)
            {
                collections.Remove(collection);
            }

            emptyCollections = collections.Values.Where(x => x.Publications.All(y => !publications.Contains(y.Value.Id))).Select(x => x.Id).ToArray();
            foreach (long collection in emptyCollections)
            {
                collections.Remove(collection);
            }

            foreach (DesksBookCollection collection in collections.Values.Select(x => x as DesksBookCollection))
            {
                collection.ExerciseTypes =
                    collection.Publications.Values.SelectMany(x => x.ExerciseTypes).Distinct().ToArray();
            }

            DesksBookPublication[] finalArticles =
                articles.Values
                    .Cast<DesksBookPublication>()
                    .OrderBy(x => x.PublicationType.GetAttribute<DesksBookPublicationTypeDataAttribute>().Position)
                    .ThenBy(x => x.Title)
                    .ThenBy(x => x.Subtitle)
                    .ToArray();

            DesksBookPublication[] finalBooks =
                books.Values
                    .Cast<DesksBookPublication>()
                    .OrderBy(x => x.PublicationType.GetAttribute<DesksBookPublicationTypeDataAttribute>().Position)
                    .ThenBy(x => x.Title)
                    .ThenBy(x => x.Subtitle)
                    .ToArray();

            DesksBookCollection[] finalCollections =
                collections.Values
                    .Cast<DesksBookCollection>()
                    .OrderBy(x => x.Name)
                    .ToArray();

            DesksBookList list = new DesksBookList
            {
                Types = types,
                Articles = finalArticles,
                Books = finalBooks,
                Collections = finalCollections
            };

            return list;
        }

        public static IDictionary<long, byte[]> GetBookCovers(Language language, bool collection)
        {
            Dictionary<long, byte[]> covers = new Dictionary<long, byte[]>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_BOOKS_Covers]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@collection",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    collection);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                long id = (long)reader["id"];
                                byte[] cover = reader["cover"] as byte[];
                                covers.Add(id, cover);
                            }
                        });
            }

            return covers;
        }

        private static void SetPublicationData(
            IDictionary<long, IDesksBookData> publications,
            IDataReader reader)
        {
            DesksBookPublication lastPublication = null;

            // Levels
            reader.NextResult();
            while (reader.Read())
            {
                long id = (long)reader["id"];

                if (lastPublication == null || lastPublication.Id != id)
                {
                    lastPublication = publications[id] as DesksBookPublication;
                }

                if (lastPublication != null)
                {
                    ((List<int>)lastPublication.Levels).Add((int)reader["level"]);
                }
            }

            // Authors
            reader.NextResult();
            while (reader.Read())
            {
                long id = (long)reader["id"];

                if (lastPublication == null || lastPublication.Id != id)
                {
                    lastPublication = publications[id] as DesksBookPublication;
                }

                if (lastPublication != null)
                {
                    ((List<string>)lastPublication.Authors).Add((string)reader["author"]);
                }
            }

            // Publishers
            reader.NextResult();
            while (reader.Read())
            {
                long id = (long)reader["id"];

                if (lastPublication == null || lastPublication.Id != id)
                {
                    lastPublication = publications[id] as DesksBookPublication;
                }

                if (lastPublication != null)
                {
                    ((List<string>)lastPublication.Publishers).Add((string)reader["publisher"]);
                }
            }
        }

        private static void SetElementClassification(
            IDictionary<long, IDesksBookData> datas,
            IDataReader reader)
        {
            IDesksBookData lastData = null;

            // Categories
            reader.NextResult();
            while (reader.Read())
            {
                long id = (long)reader["id"];

                if (lastData == null || lastData.Id != id)
                {
                    lastData = datas[id];
                }

                if (lastData != null)
                {
                    ((List<int>)lastData.Categories).Add((int)reader["category"]);
                }
            }

            // Tags
            reader.NextResult();
            while (reader.Read())
            {
                long id = (long)reader["id"];

                if (lastData == null || lastData.Id != id)
                {
                    lastData = datas[id];
                }

                if (lastData != null)
                {
                    ((List<int>)lastData.Tags).Add((int)reader["tag"]);
                }
            }
        }

        public static long GetBooksAssignmentId(
            Guid userId,
            int level,
            int part,
            bool vocabulary,
            bool allowLocal,
            bool allowRemote)
        {
            long id;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_GetAssignmentId]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@part",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    part);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@vocabulary",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    vocabulary);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowLocal);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowRemote);

                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Output, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                id = (long)command.Parameters["@id"].Value;
            }

            return id;
        }

        public static bool IsBookAssigned(Guid userId, long id, bool allowLocal, bool allowRemote)
        {
            return DesksService.IsBookAssigned(userId, id, null, allowLocal, allowRemote);
        }

        public static bool IsBookAssigned(Guid userId, long id, string code, bool allowLocal, bool allowRemote)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_IsAssigned]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    code);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowLocal);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@allow_remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    allowRemote);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        public static DesksExamData GetBooksData(long id)
        {
            return null;
        }

        public static IEnumerable<DesksBookAssignment> GetBookAssignments(Guid userId, Language language)
        {
            List<DesksBookAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_BOOKS_GetAssignments]"))
            {
                assignments = GetBookAssignments(userId, language, command);
            }

            return assignments;
        }

        public static IEnumerable<DesksBookAssignment> GetBookGroupAssignments(Guid groupId, Language language)
        {
            List<DesksBookAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_BOOKS_GetGroupAssignments]"))
            {
                assignments = GetBookAssignments(groupId, language, command);
            }

            return assignments;
        }

        private static List<DesksBookAssignment> GetBookAssignments(Guid uid, Language language, SqlCommand command)
        {
            List<DesksBookAssignment> assignments = new List<DesksBookAssignment>();

            SqlDatabaseManager.AddParameter(
                command,
                "@uid",
                ParameterDirection.Input,
                SqlDbType.UniqueIdentifier,
                uid);

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
                    while (reader.Read())
                    {
                        DesksBookAssignment assignment = new DesksBookAssignment
                        {
                            Id = (int)reader["id"],
                            Type = (DesksBookAssignmentType)reader["type"],
                            Publication = (int)reader["part"],
                            ExerciseType = (int)reader["exercise_type"],
                            ExerciseSubType = (int)reader["exercise_subtype"],
                            Assigned = (bool)reader["assignment"],
                            RemoteAssignment = (bool)reader["remote_assignment"],
                            Blocked = (bool)reader["blocked"],
                            Finished = (bool)reader["finished"],
                            Certified = (bool)reader["certified"]
                        };

                        assignments.Add(assignment);
                    }
                });

            return assignments;
        }

        public static string StartBook(long id, int codeValidFor)
        {
            DesksExamModel model = new DesksExamModel
                {
                    Id = id,
                    CodeValidFor = codeValidFor
                };

            string code = Execute<string>("StartExam", model);
            return code;
        }

        public static DesksCheckResult CheckBook(
            long id,
            string code,
            IEnumerable<int> questions,
            IEnumerable<IEnumerable<string>> answers)
        {
            DesksCheckExamModel model = new DesksCheckExamModel
                {
                    Id = id,
                    Code = code,
                    Questions = questions,
                    Answers = answers
                };

            DesksCheckResult result = Execute<DesksCheckResult>("CheckExam", model);
            return result;
        }

        public static bool FinishBook(long id, string code)
        {
            DesksFinishExamModel model = new DesksFinishExamModel
                {
                    Id = id,
                    Code = code
                };

            bool hasAnalyse = Execute<bool>("FinishExam", model);
            return hasAnalyse;
        }
    }
}
