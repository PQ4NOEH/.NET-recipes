namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Desks;

    public abstract partial class DesksService : Service<IDesksChannel>
    {
        public const float INDEX_CODE_TIME_MARGIN = 1.1f;

        public static IEnumerable<DesksIndexArea> GetIndexAreas(Language language)
        {
            List<DesksIndexArea> areas = new List<DesksIndexArea>();

            const string SqlQuery =
                "SELECT * FROM [dbo].[DESKS_INDEX_Areas] "
                + "WHERE [language] = @language "
                + "ORDER BY [column], [row];";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
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
                                DesksIndexArea area = new DesksIndexArea
                                    {
                                        Id = (int)reader["id"],
                                        Name = (string)reader["name"],
                                        Column = (int)reader["column"],
                                        Row = (int)reader["row"],
                                        RowSize = (int)reader["row_size"]
                                    };

                                areas.Add(area);
                            }
                        });
            }

            return areas;
        }

        public static string GetIndexSubjectName(Language language, int subject)
        {
            string subjectName;

            const string SqlQuery = "SELECT [name] FROM [dbo].[DESKS_INDEX_Subjects] WHERE [id] = @id AND [language] = @language;";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    subject);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                subjectName = (string)SqlDatabaseManager.ExecuteScalar(command, SqlConnectionString.DataWarehouse);
            }

            return subjectName;
        }

        public static DesksIndexList GetIndexList(Language language, int level, DesksIndexQuestionType type)
        {
            DesksIndexList list = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_List]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    type);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => list = DesksService.GetIndexList(reader));
            }

            return list;
        }

        internal static DesksIndexList GetIndexList(SqlDataReader reader)
        {
            List<DesksIndexType> types = new List<DesksIndexType>();
            List<DesksIndexSubject> subjects = new List<DesksIndexSubject>();
            Dictionary<int, DesksIndexSubject> flatSubjects = new Dictionary<int, DesksIndexSubject>();
            Dictionary<int, List<DesksIndexAreaSubject>> allSubjects = new Dictionary<int, List<DesksIndexAreaSubject>>();

            DesksIndexList list = new DesksIndexList
            {
                Types = types,
                Subjects = subjects
            };

            // Types
            while (reader.Read())
            {
                types.Add(
                    new DesksIndexType
                    {
                        Type = (DesksIndexExerciseType)reader["type"],
                        Position = (int)reader["position"]
                    });
            }

            reader.NextResult();

            // Subjects
            DesksIndexSubject lastSubject = null;
            while (reader.Read())
            {
                int id = (int)reader["id"];

                if (lastSubject == null || lastSubject.Id != id)
                {
                    int? parent = reader["parent"] as int?;
                    List<DesksIndexAreaSubject> areaSubjects = new List<DesksIndexAreaSubject>();

                    lastSubject = new DesksIndexSubject
                    {
                        Id = id,
                        Name = (string)reader["name"],
                        Areas = areaSubjects,
                    };

                    if (parent.HasValue)
                    {
                        DesksIndexSubject parentSubject;
                        if (!flatSubjects.TryGetValue(parent.Value, out parentSubject))
                        {
                            continue;
                        }

                        if (parentSubject.Children == null)
                        {
                            parentSubject.Children = new List<DesksIndexSubject>();
                        }

                        ((List<DesksIndexSubject>)parentSubject.Children).Add(lastSubject);
                    }
                    else
                    {
                        subjects.Add(lastSubject);
                    }

                    flatSubjects.Add(id, lastSubject);
                    allSubjects.Add(id, areaSubjects);
                }

                DesksIndexAreaSubject areaSubject = new DesksIndexAreaSubject
                {
                    Area = (int)reader["area"],
                    Position = (int)reader["position"],
                    Theory = reader["theory"] as int?,
                    Boards = new Dictionary<DesksIndexExerciseType, int>()
                };

                ((List<DesksIndexAreaSubject>)lastSubject.Areas).Add(areaSubject);
            }

            reader.NextResult();

            // Boards
            int lastSubjectId = 0;
            List<DesksIndexAreaSubject> lastAreaSubjects = null;
            while (reader.Read())
            {
                int subject = (int)reader["subject"];
                if (lastSubjectId != subject)
                {
                    lastSubjectId = subject;
                    if (!allSubjects.TryGetValue(subject, out lastAreaSubjects))
                    {
                        lastSubjectId = 0;
                        continue;
                    }
                }

                DesksIndexAreaSubject areaSubjects = lastAreaSubjects.Single(x => x.Area == (int)reader["area"]);
                Dictionary<DesksIndexExerciseType, int> boards = (Dictionary<DesksIndexExerciseType, int>)areaSubjects.Boards;
                boards.Add((DesksIndexExerciseType)reader["type"], (int)reader["boards"]);
            }

            return list;
        }

        public static long GetIndexAssignmentId(
            Guid userId,
            int level,
            DesksIndexExerciseType type,
            int area,
            int subject,
            bool allowLocal,
            bool allowRemote)
        {
            long id;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_GetAssignmentId]"))
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
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    (int)type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@area",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    area);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@subject",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    subject);

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

                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Output, SqlDbType.BigInt);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                id = (long)command.Parameters["@id"].Value;
            }

            return id;
        }

        public static bool IsIndexAssigned(Guid userId, long id, bool allowLocal, bool allowRemote)
        {
            return DesksService.IsIndexAssigned(userId, id, null, allowLocal, allowRemote);
        }

        public static bool IsIndexAssigned(Guid userId, long id, string code, bool allowLocal, bool allowRemote)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_IsAssigned]"))
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

        public static DesksIndexData GetIndexData(long id, DesksIndexQuestionType type)
        {
            DesksIndexData data = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_Get]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    type);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            List<DesksIndexExercise> exercises = new List<DesksIndexExercise>();

                            data = new DesksIndexData
                            {
                                Exercises = exercises
                            };

                            Dictionary<int, DesksIndexQuestion> questions = new Dictionary<int, DesksIndexQuestion>();

                            reader.Read();
                            DesksIndexExerciseType exerciseType = (DesksIndexExerciseType)reader["exercise_type"];
                            data.ExerciseType = exerciseType;
                            data.Round = (int)reader["round"];

                            // Exercises
                            reader.NextResult();
                            while (reader.Read())
                            {
                                DesksIndexExercise exercise = new DesksIndexExercise
                                    {
                                        Id = (int)reader["id"],
                                        Statement = reader["statement"] as string,
                                        Other = ((string)reader["other"]).FromJson<dynamic>()
                                    };
                                exercises.Add(exercise);
                            }

                            // Questions
                            reader.NextResult();
                            while (reader.Read())
                            {
                                DesksIndexQuestion question = new DesksIndexQuestion
                                    {
                                        Id = (int)reader["id"],
                                        Exercise = (int)reader["exercise"],
                                        Question = (string)reader["question"],
                                        ExtraData = reader["extra_data"] as string,
                                        NumGaps = (int)reader["num_gaps"],
                                        Position = (int)reader["position"],
                                        AuxiliarWords = new List<string>(),
                                        Answers = new List<DesksIndexAnswer>(),
                                        Reported = (DesksReportStatus)reader["reported"],
                                        Children = new List<DesksIndexQuestion>()
                                    };

                                questions.Add(question.Id, question);
                            }


                            // Auxiliar Words
                            reader.NextResult();
                            DesksIndexQuestion lastQuestion = null;
                            while (reader.Read())
                            {
                                int questionId = (int)reader["id"];

                                if (lastQuestion == null || lastQuestion.Id == questionId)
                                {
                                    lastQuestion = questions[questionId];
                                }

                                ((List<string>)lastQuestion.AuxiliarWords).Add((string)reader["aux_word"]);
                            }

                            // Answers
                            reader.NextResult();
                            while (reader.Read())
                            {
                                DesksIndexAnswer answer = new DesksIndexAnswer
                                    {
                                        Id = (int)reader["Id"],
                                        Answer = (string)reader["answer"],
                                        Gap = (int)reader["gap"],
                                        Position = (int)reader["position"],
                                        Valid = (bool)reader["valid"]
                                    };

                                int questionId = (int)reader["question"];

                                if (lastQuestion == null || lastQuestion.Id != questionId)
                                {
                                    lastQuestion = questions[questionId];
                                }

                                ((List<DesksIndexAnswer>)lastQuestion.Answers).Add(answer);
                            }

                            data.Questions = questions.Values.ToArray();
                        });
            }

            return data;
        }

        public static IEnumerable<DesksIndexAssignment> GetIndexAssignments(Guid userId, int level)
        {
            List<DesksIndexAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_GetAssignments]"))
            {
                assignments = GetIndexAssignments(userId, level, command);
            }

            return assignments;
        }

        public static IEnumerable<DesksIndexAssignment> GetIndexGroupAssignments(Guid groupId, int level)
        {
            List<DesksIndexAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_GetGroupAssignments]"))
            {
                assignments = GetIndexAssignments(groupId, level, command);
            }

            return assignments;
        }

        private static List<DesksIndexAssignment> GetIndexAssignments(Guid uid, int level, SqlCommand command)
        {
            List<DesksIndexAssignment> assignments = new List<DesksIndexAssignment>();

            SqlDatabaseManager.AddParameter(
                command,
                "@uid",
                ParameterDirection.Input,
                SqlDbType.UniqueIdentifier,
                uid);

            SqlDatabaseManager.AddParameter(
                command,
                "@level",
                ParameterDirection.Input,
                SqlDbType.Int,
                level);

            SqlDatabaseManager.ExecuteReader(
                command,
                SqlConnectionString.DataWarehouse,
                reader =>
                {
                    while (reader.Read())
                    {
                        int assigned = (int)reader["assigned"];
                        int finished = (int)reader["finished"];
                        int certified = (int)reader["certified"];

                        if (assigned + finished + certified == 0)
                        {
                            continue;
                        }

                        DesksIndexAssignment assignment = new DesksIndexAssignment
                        {
                            Area = (int)reader["area"],
                            Subject = (int)reader["subject"],
                            Type = (DesksIndexExerciseType)reader["type"],
                            Assigned = assigned,
                            RemoteAssignment = (int)reader["remote_assignment"],
                            Blocked = (int)reader["blocked"],
                            Finished = finished,
                            Certified = certified
                        };

                        assignments.Add(assignment);
                    }
                });

            return assignments;
        }

        public static int GetIndexAssignmentRound(long id)
        {
            int round;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_GetAssignmentRound]"))
            {
                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Input, SqlDbType.BigInt, id);
                SqlDatabaseManager.AddParameter(command, "@round", ParameterDirection.Output, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                round = (int)command.Parameters["@round"].Value;
            }

            return round;
        }

        public static void GetIndexAssignmentData(long id, out DesksIndexExerciseType exerciseType, out int round, out int numQuestions)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_GetAssignmentData]"))
            {
                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Input, SqlDbType.BigInt, id);
                SqlDatabaseManager.AddParameter(command, "@exercise_type", ParameterDirection.Output, SqlDbType.Int);
                SqlDatabaseManager.AddParameter(command, "@round", ParameterDirection.Output, SqlDbType.Int);
                SqlDatabaseManager.AddParameter(command, "@num_questions", ParameterDirection.Output, SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                exerciseType = (DesksIndexExerciseType)command.Parameters["@exercise_type"].Value;
                round = (int)command.Parameters["@round"].Value;
                numQuestions = (int)command.Parameters["@num_questions"].Value;
            }
        }

        #region Execute

        public static string StartIndex(long id, bool remote, int offset, int codeValidFor)
        {
            DesksIndexModel model = new DesksIndexModel
                {
                    Id = id,
                    Remote = remote,
                    OffsetDate = offset,
                    CodeValidFor = codeValidFor
                };

            string code = DesksService.Execute<string>("StartIndex", model);
            return code;
        }

        public static DesksCheckResult CheckIndex(
            long id,
            string code,
            IEnumerable<int> questions,
            IEnumerable<IEnumerable<string>> answers,
            TimeSpan executionTime)
        {
            DesksCheckIndexModel model = new DesksCheckIndexModel
                {
                    Id = id,
                    Code = code,
                    Questions = questions,
                    Answers = answers,
                    Time = executionTime
                };

            DesksCheckResult result = DesksService.Execute<DesksCheckResult>("CheckIndex", model);
            return result;
        }

        public static DesksFinishResult FinishIndex(long id, string code, TimeSpan executionTime, int offsetDate)
        {
            DesksFinishIndexModel model = new DesksFinishIndexModel
                {
                    Id = id,
                    Code = code,
                    Time = executionTime,
                    OffsetDate = offsetDate
                };

            DesksFinishResult result = DesksService.Execute<DesksFinishResult>("FinishIndex", model);
            return result;
        }

        #endregion

        #region Manage

        public static void IndexAssign(
            Guid member,
            AlteaMemberType memberType,
            DesksIndexQuestionType assignmentType,
            int level,
            int area,
            int subject,
            int type,
            int num,
            bool remote,
            bool status,
            bool unblock,
            Guid teacher,
            int offsetDate)
        {
            DesksAssignIndexModel model = new DesksAssignIndexModel
                {
                    Member = member,
                    MemberType = memberType,
                    AssignmentType = assignmentType,
                    Level = level,
                    Area = area,
                    Subject = subject,
                    Type = type,
                    Num = num,
                    Remote = remote,
                    Assign = status,
                    Unblock = unblock,
                    AssignmentTeacher = teacher,
                    OffsetDate = offsetDate
                };

            DesksService.Execute("AssignIndex", model);
        }

        #endregion
    }
}
