namespace Altea.Services
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Cryptography;
    using System.Text;

    using Altea.Classes.Admin;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Admin;
    using Altea.Models.User;

    public class UserService : IService, IUserContract
    {
        private const int SALT_SIZE = 16; 

        public string CreateUser(CreateUserModel model)
        {
            string username;
            string salt = GenerateSalt();
            string password = EncodePassword(model.Password, salt);

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_CreateUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@first_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.FirstName);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@last_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.LastName);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@mail",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Mail);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@password",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    password);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@salt",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    salt);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@role",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Role);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.LanguageFrom.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.LanguageTo.GetDatabaseId());

                SqlDatabaseManager.AddSizedParameter(
                    command,
                    "@username",
                    ParameterDirection.Output,
                    SqlDbType.NVarChar,
                    256);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);

                username = command.Parameters["@username"].Value as string;
            }

            return username;
        }

        private static string GenerateSalt()
        {
            byte[] buf = new byte[SALT_SIZE];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        private static string EncodePassword(string password, string salt)
        {
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] buffer = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, buffer, saltBytes.Length, passwordBytes.Length);

            // ReSharper disable once PossibleNullReferenceException
            byte[] hashedPassword = hashAlgorithm.ComputeHash(buffer);

            return Convert.ToBase64String(hashedPassword);
        }

        public bool SetUserLevels(AdminSetMemberLevelsModel model)
        {
            bool status;

            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_SetUserLevels]"))
            {
                UserService.SetMemberLevels(command, model, table);
                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 0;
            }

            return status;
        }

        public bool SetUserProLevels(AdminSetMemberLevelsModel model)
        {
            bool status;

            using (DataTable table = new DataTable())
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_SetUserProLevels]"))
            {
                UserService.SetMemberLevels(command, model, table);
                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        private static void SetMemberLevels(SqlCommand command, AdminSetMemberLevelsModel model, DataTable table)
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
                row["language_from"] = level.LanguageFrom;

                if (level.LanguageTo.HasValue && level.LanguageTo.Value != Language.NoLanguage)
                {
                    row["language_to"] = level.LanguageTo.Value;
                }
                else
                {
                    row["language_to"] = DBNull.Value;
                }

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
        }
    }
}
