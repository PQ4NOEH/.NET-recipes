namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.Desks;
    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Desks;

    public abstract partial class DesksService : Service<IDesksChannel>
    {
        public const float EXAM_CODE_TIME_MARGIN = 1.25f;

        public static DesksExamList GetExamsList(Language language, int level, DesksExamQuestionType type)
        {
            DesksExamList list = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_List]"))
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
                    reader => list = DesksService.GetExamsList(reader));
            }

            return list;
        }

        internal static DesksExamList GetExamsList(SqlDataReader reader)
        {
            List<DesksExamGroup> groups = new List<DesksExamGroup>();
            List<DesksExamTest> tests = new List<DesksExamTest>();

            DesksExamList list = new DesksExamList
            {
                Groups = groups,
                Tests = tests
            };

            // Groups
            int lastGroup = 0;
            List<DesksExamPaper> papers = null;
            while (reader.Read())
            {
                int id = (int)reader["id"];

                if (lastGroup != id)
                {
                    lastGroup = id;
                    papers = new List<DesksExamPaper>();

                    DesksExamGroup group = new DesksExamGroup
                    {
                        Id = id,
                        Title = reader["title"] as string,
                        Subtitle = reader["subtitle"] as string,
                        Position = (int)reader["position"],
                        Papers = papers
                    };

                    groups.Add(group);
                }

                DesksExamPaper paper = new DesksExamPaper
                {
                    Id = (int)reader["paper_id"],
                    Title = reader["paper_title"] as string,
                    Subtitle = reader["paper_subtitle"] as string,
                    Position = (int)reader["paper_position"],
                    HeaderId = reader["paper_header_id"] as int?,
                    HeaderTitle = reader["paper_header_title"] as string,
                    HeaderSubtitle = reader["paper_header_subtitle"] as string,
                    HasVocabulary = (bool)reader["has_vocabulary"],
                    ForceVocabulary = (bool)reader["force_vocabulary"],
                    ExamMode = (bool)reader["exam_mode"],
                    OnlyExamMode = (bool)reader["only_exam_mode"],
                    HasMixedExam = (bool)reader["has_mixed_exam"],
                    Info = reader["info"] as int?
                };

                papers.Add(paper);
            }

            reader.NextResult();

            // Tests
            int lastTest = 0;
            List<DesksExamTestPart> testParts = null;
            while (reader.Read())
            {
                int id = (int)reader["id"];

                if (lastTest != id)
                {
                    lastTest = id;
                    testParts = new List<DesksExamTestPart>();

                    DesksExamTest test = new DesksExamTest
                    {
                        Id = (int)reader["id"],
                        GroupId = (int)reader["group_id"],
                        Name = reader["name"] as string,
                        Position = (int)reader["position"],
                        Parts = testParts
                    };

                    tests.Add(test);
                }

                DesksExamTestPart part = new DesksExamTestPart
                {
                    Id = (int)reader["part"],
                    Paper = (int)reader["paper"],
                    Position = (int)reader["part_position"],
                    HasVocabulary = reader["has_vocabulary"] as bool?,
                    ForceVocabulary = reader["force_vocabulary"] as bool?,
                    AllowInExamMode = reader["allow_in_exam_mode"] as bool?,
                    AllowInMixedExam = reader["allow_in_mixed_exam"] as bool?
                };

                testParts.Add(part);
            }

            return list;
        }

        public static long GetExamsAssignmentId(
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

        public static bool IsExamAssigned(Guid userId, long id, bool allowLocal, bool allowRemote)
        {
            return DesksService.IsExamAssigned(userId, id, null, allowLocal, allowRemote);
        }

        public static bool IsExamAssigned(Guid userId, long id, string code, bool allowLocal, bool allowRemote)
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

        public static DesksExamData GetExamsData(long id)
        {
            return null;
        }

        public static IEnumerable<IDesksExamAssignment> GetExamAssignments(Guid userId, int level)
        {
            List<IDesksExamAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_GetAssignments]"))
            {
                assignments = GetExamAssignments(userId, level, command);
            }

            return assignments;
        }

        public static IEnumerable<IDesksExamAssignment> GetExamGroupAssignments(Guid groupId, int level)
        {
            List<IDesksExamAssignment> assignments;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_GetGroupAssignments]"))
            {
                assignments = GetExamAssignments(groupId, level, command);
            }

            return assignments;
        }

        private static List<IDesksExamAssignment> GetExamAssignments(Guid uid, int level, SqlCommand command)
        {
            List<IDesksExamAssignment> assignments = new List<IDesksExamAssignment>();

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
                        DesksExamPartAssignment assignment = new DesksExamPartAssignment
                        {
                            Part = (int)reader["part"],
                            Vocabulary = (bool)reader["vocabulary"],
                            Assigned = (bool)reader["assigned"],
                            RemoteAssignment = (bool)reader["remote_assignment"],
                            Blocked = (bool)reader["blocked"],
                            Finished = (bool)reader["finished"],
                            Certified = (bool)reader["certified"]
                        };

                        assignments.Add(assignment);
                    }

                    long lastId = 0;
                    List<int> parts = null;
                    for (int i = 2; i > 0; i--)
                    {
                        reader.NextResult();

                        while (reader.Read())
                        {
                            long id = (long)reader["id"];

                            if (id != lastId)
                            {
                                lastId = id;
                                DesksExamTestAssignment testAssignment = new DesksExamTestAssignment
                                    {
                                        Group = (int)reader["group"],
                                        Paper = (int)reader["paper"],
                                        Test = reader["test"] as int?,
                                        Round = reader["round"] as int?,
                                        Assigned = (bool)reader["assigned"],
                                        RemoteAssignment = (bool)reader["remote_assignment"],
                                        Blocked = (bool)reader["blocked"],
                                        Finished = (bool)reader["finished"],
                                        Certified = (bool)reader["certified"]
                                    };

                                if (i == 1)
                                {
                                    parts = new List<int>();
                                    testAssignment.Parts = parts;
                                    parts.Add((int)reader["part"]);
                                }

                                assignments.Add(testAssignment);
                            }
                            else if (i == 1)
                            {
                                parts.Add((int)reader["part"]);
                            }
                        }
                    }
                });

            return assignments;
        }

        #region Execute

        public static string StartExam(long id, int codeValidFor)
        {
            DesksExamModel model = new DesksExamModel
                {
                    Id = id,
                    CodeValidFor = codeValidFor
                };

            string code = DesksService.Execute<string>("StartExam", model);
            return code;
        }

        public static DesksCheckResult CheckExam(
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

            DesksCheckResult result = DesksService.Execute<DesksCheckResult>("CheckExam", model);
            return result;
        }

        public static bool FinishExam(long id, string code)
        {
            DesksFinishExamModel model = new DesksFinishExamModel
                {
                    Id = id,
                    Code = code
                };

            bool hasAnalyse = DesksService.Execute<bool>("FinishExam", model);
            return hasAnalyse;
        }

        #endregion

        #region Manage

        public static void ExamAssign(
            Guid member,
            AlteaMemberType memberType,
            DesksExamQuestionType assignmentType,
            int level,
            int part,
            bool vocabulary,
            bool remote,
            bool status,
            bool unblock,
            Guid teacher,
            int offsetDate)
        {
            DesksAssignExamModel model = new DesksAssignExamModel
            {
                Member = member,
                MemberType = memberType,
                AssignmentType = assignmentType,
                Level = level,
                Part = part,
                Vocabulary = vocabulary,
                Remote = remote,
                Assign = status,
                Unblock = unblock,
                AssignmentTeacher = teacher,
                OffsetDate = offsetDate
            };

            DesksService.Execute("AssignExam", model);
        }

        public static void ExamTestAssign(
            Guid member,
            AlteaMemberType memberType,
            DesksExamQuestionType assignmentType,
            int level,
            int group,
            int paper,
            int? test,
            int? round,
            IEnumerable<int> parts, 
            bool remote,
            bool status,
            bool unblock,
            Guid teacher,
            int offsetDate)
        {
            DesksAssignExamTestModel model = new DesksAssignExamTestModel
            {
                Member = member,
                MemberType = memberType,
                AssignmentType = assignmentType,
                Level = level,
                Group = group,
                Paper = paper,
                Test = test,
                Round = round,
                Parts = parts,
                Remote = remote,
                Assign = status,
                Unblock = unblock,
                AssignmentTeacher = teacher,
                OffsetDate = offsetDate
            };

            DesksService.Execute("AssignExamTest", model);
        }

        #endregion
    }
}
