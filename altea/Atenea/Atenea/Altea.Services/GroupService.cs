namespace Altea.Services
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.Admin;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Models.Admin;

    public class GroupService : IService, IGroupContract
    {
        public Guid CreateGroup()
        {
            throw new NotImplementedException();
        }

        public bool EditGroup()
        {
            throw new NotImplementedException();
        }

        public bool DeleteGroup()
        {
            throw new NotImplementedException();
        }

        public bool ToggleEnableGroup()
        {
            throw new NotImplementedException();
        }

        public bool AddUserToGroup()
        {
            throw new NotImplementedException();
        }

        public bool DeleteUserFromGroup()
        {
            throw new NotImplementedException();
        }

        public bool SetGroupLevels(AdminSetMemberLevelsModel model)
        {
            bool status;

            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_SetGroupLevels]"))
            {
                table.Columns.Add("language_from", typeof(int));
                table.Columns.Add("language_to", typeof(int));
                table.Columns.Add("level", typeof(int));
                table.Columns.Add("sublevel", typeof(int));
                table.Columns.Add("primary", typeof(bool));
                table.Columns.Add("active", typeof(bool));

                foreach (AdminMemberLevel level in model.Levels)
                {
                    DataRow row = table.NewRow();
                    row["language_from"] = DBNull.Value;
                    row["language_to"] = DBNull.Value;
                    row["level"] = level.Level;
                    row["sublevel"] = DBNull.Value;
                    row["primary"] = level.Primary;
                    row["active"] = true;

                    table.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@member",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Member);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Application);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.LanguageFrom);


                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.LanguageTo);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@levels",
                    ParameterDirection.Input,
                    "[dbo].[altea_Level]",
                    table);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 0;
            }

            return status;
        }
    }
}
