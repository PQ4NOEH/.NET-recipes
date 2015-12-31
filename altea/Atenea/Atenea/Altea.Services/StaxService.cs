namespace Altea.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Stax;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Stax;

    public class StaxService : IService, IStaxContract
    {
        internal static readonly TimeSpan MinTimeSpan = TimeSpan.Zero;
        internal static readonly TimeSpan MaxTimeSpan = TimeSpan.FromDays(1d);

        public int InsertInboxData(StackNewInboxDataModel model)
        {
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[STAX_InsertInbox]"))
            {
                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, model.UserId);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, model.From.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, model.To.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack_type", ParameterDirection.Input, SqlDbType.Int, (int)model.Type);
                SqlDatabaseManager.AddParameter(command, "@data", ParameterDirection.Input, SqlDbType.NVarChar, model.Data);
                SqlDatabaseManager.AddParameter(command, "@origin", ParameterDirection.Input, SqlDbType.Int, model.Origin);
                SqlDatabaseManager.AddParameter(command, "@reference", ParameterDirection.Input, SqlDbType.Int, model.Reference);
                SqlDatabaseManager.AddParameter(command, "@searched", ParameterDirection.Input, SqlDbType.Bit, model.Searched);
                SqlDatabaseManager.AddParameter(command, "@sentence", ParameterDirection.Input, SqlDbType.NVarChar, model.Sentence);
                SqlDatabaseManager.AddParameter(command, "@inbox_overflow", ParameterDirection.Input, SqlDbType.Int, model.InboxOverflow);
                SqlDatabaseManager.AddParameter(command, "@offset_date", ParameterDirection.Input, SqlDbType.Int, model.OffsetDate);

                return SqlDatabaseManager.ExecuteScalar<int>(command, SqlConnectionString.DataWarehouse);
            }
        }

        public void AcceptInboxData(StackAcceptInboxDataModel model)
        {
            using (DataTable table = new DataTable())
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[STAX_AcceptInbox]"))
            {
                table.Columns.Add("n", typeof(string));
                table.Columns.Add("m", typeof(int));

                foreach (StaxContentData data in model.DataOptions)
                {
                    DataRow row = table.NewRow();
                    row["n"] = data.Data.Trim();
                    row["m"] = data.Type;
                    table.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, model.UserId);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, model.From.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, model.To.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack_type", ParameterDirection.Input, SqlDbType.Int, (int)model.Type);
                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Input, SqlDbType.BigInt, model.Id);
                SqlDatabaseManager.AddParameter(command, "@data", ParameterDirection.Input, SqlDbType.NVarChar, model.Data);
                SqlDatabaseManager.AddParameter(command, "@max_data", ParameterDirection.Input, SqlDbType.Int, model.MaxData);
                SqlDatabaseManager.AddParameter(command, "@data_options", ParameterDirection.Input, "[dbo].[stringintdictionary]", table);
                SqlDatabaseManager.AddParameter(command, "@offset_date", ParameterDirection.Input, SqlDbType.Int, model.OffsetDate);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public void DeleteInboxData(StackDeleteInboxDataModel model)
        {
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[STAX_DeleteInbox]"))
            {
                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, model.UserId);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, model.From.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, model.To.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack_type", ParameterDirection.Input, SqlDbType.Int, (int)model.Type);
                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.Input, SqlDbType.BigInt, model.Id);
                SqlDatabaseManager.AddParameter(command, "@data", ParameterDirection.Input, SqlDbType.NVarChar, model.Data);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }

        public void CheckStaxExist(StaxCheckModel model)
        {
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[STAX_CheckStaxExist]"))
            {
                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, model.UserId);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, model.LanguageFrom.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, model.LanguageTo.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack_type", ParameterDirection.Input, SqlDbType.Int, (int)model.Type);
                SqlDatabaseManager.AddParameter(command, "@max_stack", ParameterDirection.Input, SqlDbType.Int, model.MaxStack);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }


        public IEnumerable<string> SaveExercise(StackSaveExerciseDataModel model)
        {
            List<string> toInbox = new List<string>();

            using (DataTable exercisesTable = new DataTable())
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[STAX_SaveExercise]"))
            {
                exercisesTable.Columns.Add("n", typeof(long));
                exercisesTable.Columns.Add("m", typeof(string));

                foreach (StackExerciseAnswer data in model.Exercises)
                {
                    IEnumerator<string> answer = data.Answers.GetEnumerator();

                    for (int i = 0, l = data.Answers.Count(); i < l; i++)
                    {
                        answer.MoveNext();

                        DataRow row = exercisesTable.NewRow();
                        row["n"] = data.Id;
                        row["m"] = answer.Current;

                        exercisesTable.Rows.Add(row);
                    }
                }

                SqlDatabaseManager.AddParameter(command, "@uid", ParameterDirection.Input, SqlDbType.UniqueIdentifier, model.UserId);
                SqlDatabaseManager.AddParameter(command, "@app_id", ParameterDirection.Input, SqlDbType.UniqueIdentifier, model.AppId);
                SqlDatabaseManager.AddParameter(command, "@remote", ParameterDirection.Input, SqlDbType.Bit, model.Remote);
                SqlDatabaseManager.AddParameter(command, "@from", ParameterDirection.Input, SqlDbType.Int, model.From.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@to", ParameterDirection.Input, SqlDbType.Int, model.To.GetDatabaseId());
                SqlDatabaseManager.AddParameter(command, "@stack_type", ParameterDirection.Input, SqlDbType.Int, (int)model.Type);
                SqlDatabaseManager.AddParameter(command, "@stack_num", ParameterDirection.Input, SqlDbType.Int, model.StackNum);
                SqlDatabaseManager.AddParameter(command, "@exercises", ParameterDirection.Input, "[dbo].[bigintstringdictionary]", exercisesTable);
                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.Input, SqlDbType.Bit, model.Status);
                SqlDatabaseManager.AddParameter(command, "@retry_cooldown", ParameterDirection.Input, SqlDbType.Int, model.RetryCooldown);
                SqlDatabaseManager.AddParameter(command, "@inbox_errors", ParameterDirection.Input, SqlDbType.Int, model.InboxErrors);
                SqlDatabaseManager.AddParameter(command, "@offset_date", ParameterDirection.Input, SqlDbType.Int, model.OffsetDate);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@time",
                    ParameterDirection.Input,
                    SqlDbType.Time,
                    model.Time < MinTimeSpan ? MinTimeSpan : model.Time > MaxTimeSpan ? MaxTimeSpan : model.Time);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                    {
                        while (reader.Read())
                        {
                            toInbox.Add((string)reader["data"]);
                        }
                    });
            }

            return toInbox;
        }

        public static void CalculateAverages(StackType type, DateTime date)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STAX_CalculateAverages]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    (int)type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@date",
                    ParameterDirection.Input,
                    SqlDbType.DateTime,
                    date);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.DataWarehouse);
            }
        }
    }
}
