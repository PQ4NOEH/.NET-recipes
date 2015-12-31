namespace Altea.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Desks;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Models.Desks;

    public class DesksService : IService, IDesksContract
    {
        public void AutoUnblock(DesksAutoUnblockModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_AutoUnblock]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);
                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@days",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Days);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        public void CheckLastBlock(DesksCheckLastBlockModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_CheckLastBlock]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@days",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Days);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        #region Index
        public void AssignIndex(DesksAssignIndexModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_Assign]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Member);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@member_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.MemberType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AssignmentType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@area",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Area);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@subject",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Subject);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@num",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Num);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Remote);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assign",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Assign);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@unblock",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Unblock);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_teacher",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AssignmentTeacher);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public string StartIndex(DesksIndexModel model)
        {
            string code;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_Start]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Remote);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code_valid_for",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.CodeValidFor);

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@code",
                    ParameterDirection.Output,
                    SqlDbType.NVarChar,
                    16);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                code = (string)command.Parameters["@code"].Value;
            }

            return code;
        }

        public DesksCheckResult CheckIndex(DesksCheckIndexModel model)
        {
            DesksCheckResult result = null;

            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_Check]"))
            {
                table.Columns.Add("question", typeof(int));
                table.Columns.Add("answer", typeof(string));
                table.Columns.Add("gap", typeof(int));

                IEnumerator<int> questions = model.Questions.GetEnumerator();
                IEnumerator<IEnumerable<string>> answers = model.Answers.GetEnumerator();

                for (int i = 0, l = model.Questions.Count(); i < l; i++)
                {
                    questions.MoveNext();
                    int question = questions.Current;

                    answers.MoveNext();
                    IEnumerable<string> answer = answers.Current;
                    IEnumerator<string> questionAnswers = answer.GetEnumerator();

                    for (int j = 0, k = answer.Count(); j < k; j++)
                    {
                        questionAnswers.MoveNext();

                        DataRow row = table.NewRow();
                        row["question"] = question;
                        row["answer"] = questionAnswers.Current;
                        row["gap"] = j + 1;

                        table.Rows.Add(row);
                    }
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Code);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    "[dbo].[DESKS_CheckData]",
                    table);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@execution_time",
                    ParameterDirection.Input,
                    SqlDbType.Time,
                    model.Time);

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@new_code",
                    ParameterDirection.Output,
                    SqlDbType.NVarChar,
                    16);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            reader.Read();

                            result = new DesksCheckResult
                                {
                                    CheckStatus = (DesksCheckStatus)reader["status"],
                                    Questions = new List<int>(),
                                    Status = new List<bool>(),
                                    AnswersStatus = new List<IEnumerable<bool>>(),
                                    Answers = new List<IEnumerable<string>>()
                                };

                            if (result.CheckStatus == DesksCheckStatus.CodeError)
                            {
                                return;
                            }

                            reader.NextResult();

                            int lastQuestion = 0;
                            List<bool> lastAnswersStatus = null;
                            List<string> lastAnswers = null;

                            while (reader.Read())
                            {
                                int question = (int)reader["question"];

                                if (question != lastQuestion)
                                {
                                    lastQuestion = question;
                                    lastAnswersStatus = new List<bool>();
                                    lastAnswers = new List<string>();

                                    ((List<int>)result.Questions).Add(question);
                                    ((List<bool>)result.Status).Add((bool)reader["status"]);
                                    ((List<IEnumerable<bool>>)result.AnswersStatus).Add(lastAnswersStatus);
                                    ((List<IEnumerable<string>>)result.Answers).Add(lastAnswers);
                                }

                                lastAnswersStatus.Add((bool)reader["answer_status"]);
                                lastAnswers.Add((string)reader["answer"]);
                            }
                        });

                if (result.CheckStatus == DesksCheckStatus.Success)
                {
                    result.Code = (string)command.Parameters["@new_code"].Value;
                }
            }

            return result;
        }

        public DesksFinishResult FinishIndex(DesksFinishIndexModel model)
        {
            DesksFinishResult result = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_Finish]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Code);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@execution_time",
                    ParameterDirection.Input,
                    SqlDbType.Time,
                    model.Time);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(command, "@has_analyse", ParameterDirection.Output, SqlDbType.Bit);
                SqlDatabaseManager.AddParameter(command, "@is_blocked", ParameterDirection.Output, SqlDbType.Bit);
                DesksCheckStatus status = SqlDatabaseManager.ExecuteScalar<DesksCheckStatus>(command, SqlConnectionString.DataWarehouse);

                result = new DesksFinishResult
                    {
                        CheckStatus = status
                    };

                if (status == DesksCheckStatus.Success)
                {
                    result.HasAnalyse = (bool)command.Parameters["@has_analyse"].Value;
                    result.IsBlocked = (bool)command.Parameters["@is_blocked"].Value;
                }
            }

            return result;
        }

        public void ReportIndex(DesksReportIndexModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_INDEX_Report]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);
                
                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@question",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Question);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@feedback_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.FeedbackType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@feedback",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Feedback);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@block",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Block);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }
        #endregion

        #region Exams
        public void AssignExam(DesksAssignExamModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_Assign]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Member);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@member_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.MemberType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AssignmentType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@part",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Part);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@vocabulary",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Vocabulary);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Remote);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assign",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Assign);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@unblock",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Unblock);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_teacher",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AssignmentTeacher);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public string StartExam(DesksExamModel model)
        {
            string code;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_Start]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code_valid_for",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.CodeValidFor);

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@code",
                    ParameterDirection.Output, 
                    SqlDbType.NVarChar,
                    16);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                code = (string)command.Parameters["@code"].Value;
            }

            return code;
        }

        public DesksCheckResult CheckExam(DesksCheckExamModel model)
        {
            DesksCheckResult result = null;

            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_Check]"))
            {
                table.Columns.Add("question", typeof(int));
                table.Columns.Add("answer", typeof(string));
                table.Columns.Add("gap", typeof(int));

                IEnumerator<int> questions = model.Questions.GetEnumerator();
                IEnumerator<IEnumerable<string>> answers = model.Answers.GetEnumerator();

                for (int i = 0, l = model.Questions.Count(); i < l; i++)
                {
                    questions.MoveNext();
                    int question = questions.Current;

                    answers.MoveNext();
                    IEnumerable<string> answer = answers.Current;
                    IEnumerator<string> questionAnswers = answer.GetEnumerator();

                    for (int j = 0, k = answer.Count(); j < k; j++)
                    {
                        questionAnswers.MoveNext();

                        DataRow row = table.NewRow();
                        row["question"] = question;
                        row["answer"] = questionAnswers.Current;
                        row["gap"] = j;

                        table.Rows.Add(row);
                    }
                }

                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Input, SqlDbType.BigInt, model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Code);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    "[dbo].[DESKS_CheckData]",
                    table);

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@new_code",
                    ParameterDirection.Output,
                    SqlDbType.NVarChar,
                    16);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            reader.Read();

                            result = new DesksCheckResult
                                {
                                    CheckStatus = (DesksCheckStatus)reader["status"],
                                    Questions = new List<int>(),
                                    Status = new List<bool>(),
                                    AnswersStatus = new List<IEnumerable<bool>>(),
                                    Answers = new List<IEnumerable<string>>()
                                };

                            reader.NextResult();

                            int lastQuestion = 0;
                            List<bool> lastAnswersStatus = null;
                            List<string> lastAnswers = null;

                            while (reader.Read())
                            {
                                int question = (int)reader["question"];

                                if (question != lastQuestion)
                                {
                                    lastQuestion = question;
                                    lastAnswersStatus = new List<bool>();
                                    lastAnswers = new List<string>();

                                    ((List<int>)result.Questions).Add(question);
                                    ((List<bool>)result.Status).Add((bool)reader["status"]);
                                    ((List<IEnumerable<bool>>)result.AnswersStatus).Add(lastAnswersStatus);
                                    ((List<IEnumerable<string>>)result.Answers).Add(lastAnswers);
                                }

                                lastAnswersStatus.Add((bool)reader["answer_status"]);
                                lastAnswers.Add((string)reader["answer"]);
                            }
                        });

                result.Code = (string)command.Parameters["@code"].Value;
            }

            return result;
        }

        public bool FinishExam(DesksFinishExamModel model)
        {
            bool hasAnalyse;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_Finish]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Code);

                SqlDatabaseManager.AddParameter(command, "@has_analyse", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                hasAnalyse = (int)command.Parameters["@has_analyse"].Value == 1;
            }

            return hasAnalyse;
        }

        public void ReportExam(DesksReportExamModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_Report]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@part",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Part);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@question",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Question);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@feedback_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.FeedbackType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@feedback",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Feedback);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@block",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Block);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }
        #endregion

        #region ExamTest
        public bool AssignExamTest(DesksAssignExamTestModel model)
        {
            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_AssignTest]"))
            {
                table.Columns.Add("n", typeof(int));

                if (!model.Test.HasValue)
                {
                    foreach (int part in model.Parts)
                    {
                        DataRow row = table.NewRow();
                        row["n"] = part;
                        table.Rows.Add(row);
                    }
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Member);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@member_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.MemberType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AssignmentType);

                SqlDatabaseManager.AddParameter(
                    command, 
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@group",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Group);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@paper",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Paper);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@test",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Test);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@round",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Round);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@parts",
                    ParameterDirection.Input,
                    "[dbo].[intlist]",
                    table);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Remote);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assign",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Assign);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@unblock",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Unblock);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_teacher",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AssignmentTeacher);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }

            return true;
        }

        public string StartExamTest(DesksIndexModel model)
        {
            string code;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_StartTest]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code_valid_for",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.CodeValidFor);

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@code",
                    ParameterDirection.Output,
                    SqlDbType.NVarChar,
                    16);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                code = (string)command.Parameters["@code"].Value;
            }

            return code;
        }

        public DesksCheckResult CheckExamTest(DesksCheckExamTestModel model)
        {
            DesksCheckResult result = null;

            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_CheckTest]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Code);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    "[dbo].[DESKS_CheckData]",
                    table);

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@new_code",
                    ParameterDirection.Output,
                    SqlDbType.NVarChar,
                    16);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            result = new DesksCheckResult
                                {
                                    CheckStatus = (DesksCheckStatus)reader["status"],
                                    Questions = new List<int>(),
                                    Status = new List<bool>(),
                                    AnswersStatus = new List<IEnumerable<bool>>(),
                                    Answers = new List<IEnumerable<string>>()
                                };

                            int lastQuestion = 0;
                            List<bool> lastAnswersStatus = null;
                            List<string> lastAnswers = null;

                            while (reader.Read())
                            {
                                int question = (int)reader["question"];

                                if (question != lastQuestion)
                                {
                                    lastQuestion = question;
                                    lastAnswersStatus = new List<bool>();
                                    lastAnswers = new List<string>();

                                    ((List<int>)result.Questions).Add(question);
                                    ((List<bool>)result.Status).Add((bool)reader["status"]);
                                    ((List<IEnumerable<bool>>)result.AnswersStatus).Add(lastAnswersStatus);
                                    ((List<IEnumerable<string>>)result.Answers).Add(lastAnswers);
                                }

                                lastAnswersStatus.Add((bool)reader["answer_Status"]);
                                lastAnswers.Add((string)reader["answer"]);
                            }
                        });

                result.Code = (string)command.Parameters["@new_code"].Value;
            }

            return result;
        }

        public bool FinishExamTest(DesksFinishExamTestModel model)
        {
            bool hasAnalyse;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXAMS_FinishTest]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@code",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Code);

                SqlDatabaseManager.AddParameter(command, "@has_analyse", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                hasAnalyse = (int)command.Parameters["@has_analyse"].Value == 1;
            }

            return hasAnalyse;
        }
        #endregion

        #region Extra
        public void AssignExtra(DesksAssignExtraModel model)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DESKS_EXTRA_Assign]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Member);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@member_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.MemberType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.AssignmentType);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@part",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Part);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@remote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Remote);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assign",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Assign);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@unblock",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.Unblock);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@assignment_teacher",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AssignmentTeacher);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }
        #endregion
    }
}
