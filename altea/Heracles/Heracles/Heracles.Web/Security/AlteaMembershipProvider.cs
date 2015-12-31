namespace Heracles.Web.Security
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Security;

    using Altea.Database;

    using Heracles.Services;

    public class AlteaMembershipProvider : MembershipProvider
    {
        private string _appName;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private int _minRequiredPasswordLength;
        private int _maxInvalidPasswordAttempts;
        private int _passwordAttemptWindow;



        public override string ApplicationName
        {
            get
            {
                return this._appName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("ApplicationName");
                }

                if (value.Length > 256)
                {
                    throw new ArgumentException("Application name is too long", "ApplicationName");
                }

                this._appName = value;
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return this._enablePasswordReset;
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                return this._enablePasswordRetrieval;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                return this._minRequiredPasswordLength;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this._maxInvalidPasswordAttempts;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return this._passwordAttemptWindow;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "AlteaSqlMembershipProvider";
            }

            base.Initialize(name, config);

            this._appName = config["applicationName"];
            if (string.IsNullOrEmpty(this._appName))
            {
                throw new ArgumentNullException("ApplicationName");
            }

            if (this._appName.Length > 256)
            {
                throw new ArgumentException("Application name is too long", "ApplicationName");
            }
            
            this._enablePasswordReset = GetBooleanValue(config, "enablePasswordReset", true);
            this._enablePasswordRetrieval = GetBooleanValue(config, "enablePasswordRetrieval", false);
            this._minRequiredPasswordLength = GetIntValue(config, "minRequiredPasswordLength", 7, false, 128);
            this._maxInvalidPasswordAttempts = GetIntValue(config, "maxInvalidPasswordAttempts", 5, false, 0);
            this._passwordAttemptWindow = GetIntValue(config, "passwordAttemptWindow", 10, false, 0);

            config.Remove("applicationName");
            config.Remove("enablePasswordReset");
            config.Remove("enablePasswordRetrieval");
            config.Remove("minRequiredPasswordLength");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");

            if (config.Count <= 0)
            {
                return;
            }

            string key = config.GetKey(0);
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            throw new ArgumentException("Unrecognized attribute", key);
        }

        private static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
        {
            string str = config[valueName];
            if (str == null)
            {
                return defaultValue;
            }

            bool result;
            if (bool.TryParse(str, out result))
            {
                return result;
            }

            throw new ArgumentException("Value must be boolean", valueName);
        }

        private static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed)
        {
            string s = config[valueName];
            if (s == null)
            {
                return defaultValue;
            }

            int result;
            if (!int.TryParse(s, out result))
            {
                if (zeroAllowed)
                {
                    throw new ArgumentException("Value must be non negative integer", valueName);
                }
                else
                {
                    throw new ArgumentException("Value must be positive integer", valueName);
                }
            }
            else if (zeroAllowed && result < 0)
            {
                throw new ArgumentException("Value must be non negative integer", valueName);
            }
            else if (!zeroAllowed && result <= 0)
            {
                throw new ArgumentException("Value must be positive integer", valueName);
            }
            else
            {
                if (maxValueAllowed <= 0 || result <= maxValueAllowed)
                {
                    return result;
                }

                throw new ArgumentException("Value too big", valueName);
            }
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            this.OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (this.RequiresUniqueEmail && String.IsNullOrEmpty(this.GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = this.GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                //int recAdded = AccountService.CreateUser();
                int recAdded = 0;
                if (recAdded > 0)
                {
                    status = MembershipCreateStatus.Success;
                }
                else if (recAdded == 0)
                {
                    status = MembershipCreateStatus.UserRejected;
                }
                else
                {
                    status = MembershipCreateStatus.ProviderError;
                }

                return this.GetUser(username, false);
            }

            status = MembershipCreateStatus.DuplicateUserName;
            return null;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new ProviderException("Cannot retrieve Hashed passwords.");
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!this.ValidateUser(username, oldPassword))
            {
                return false;
            }

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, false);
            this.OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
                }
            }

            return AccountService.ChangePassword(username, oldPassword, newPassword);
        }

        public override string ResetPassword(string username, string answer)
        {
            //string newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            //ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, false);
            //OnValidatingPassword(args);

            //if (args.Cancel)
            //{
            //    if (args.FailureInformation != null)
            //    {
            //        throw args.FailureInformation;
            //    }
            //    else
            //    {
            //        throw new MembershipPasswordException("Reset password canceled due to password validation failure");
            //    }
            //}
            return "lalala;";
            //return AccountService.ResetPassword(username, newPassword);
        }

        public override void UpdateUser(MembershipUser user)
        {
            //AccountService.UpdateUser(user);
        }

        public Guid GetProviderUserKey(string username)
        {
            Guid providerUserKey = Guid.Empty;

            const string SqlQuery = "SELECT [UserId] FROM [dbo].[aspnet_Users] WHERE [UserName] = @username;";
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        if (!reader.Read())
                        {
                            return;
                        }

                        providerUserKey = (Guid)reader["UserId"];
                    });
            }

            return providerUserKey;
        }

        public bool UserExists(string username)
        {
            bool userExists = false;

            const string SqlQuery =
                "SELECT 1 FROM [dbo].[aspnet_Membership] AS [A] " +
                "INNER JOIN [dbo].[aspnet_Users] AS [B] ON [A].[UserId] = [B].[UserId] " +
                "INNER JOIN [dbo].[altea_UsersInApplications] AS [C] ON [A].[UserId] = [C].[user] " +
                "WHERE [B].[UserName] = @username AND [C].[application] = @application_id;";

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@application_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        if (!reader.Read())
                        {
                            userExists = false;
                            return;
                        }

                        userExists = true;
                    });
            }

            return userExists;
        }

        public override bool ValidateUser(string username, string password)
        {
            bool userExists = false;

            const string SqlQuery =
                "SELECT [Password], [PasswordSalt] FROM [dbo].[aspnet_Membership] AS [A] " +
                "INNER JOIN [dbo].[aspnet_Users] AS [B] ON [A].[UserId] = [B].[UserId] " +
                "INNER JOIN [dbo].[altea_UsersInApplications] AS [C] ON [A].[UserId] = [C].[user] " +
                "WHERE [B].[UserName] = @username AND [C].[application] = @application_id;";
            
            string encodedPassword = null, salt = null;

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);
                
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            if (!reader.Read())
                            {
                                return;
                            }

                            userExists = true;
                            encodedPassword = (string)reader["Password"];
                            salt = (string)reader["PasswordSalt"];
                        });
            }

            if (!userExists)
            {
                return false;
            }

            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] buffer = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, buffer, saltBytes.Length, passwordBytes.Length);

            // ReSharper disable once PossibleNullReferenceException
            byte[] hashedPassword = hashAlgorithm.ComputeHash(buffer);

            return true;//encodedPassword == Convert.ToBase64String(hashedPassword);
        }

        public bool UserIsApproved(string username)
        {
            bool isApproved;

            const string SqlQuery =
                "SELECT [IsApproved] FROM [dbo].[aspnet_Membership] AS [A] " +
                "INNER JOIN [dbo].[aspnet_Users] AS [B] ON [A].[UserId] = [B].[UserId] " +
                "WHERE [B].[UserName] = @UserName;";

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@UserName",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                isApproved = SqlDatabaseManager.ExecuteScalar<bool>(command, SqlConnectionString.Altea);
            }

            return isApproved;
        }

        public bool UserIsLocked(string username)
        {
            bool isLocked;

            const string SqlQuery =
                "SELECT [IsLockedOut] FROM [dbo].[aspnet_Membership] AS [A] " +
                "INNER JOIN [dbo].[aspnet_Users] AS [B] ON [A].[UserId] = [B].[UserId] " +
                "WHERE [B].[UserName] = @UserName;";

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.Text, SqlQuery))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@UserName",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                isLocked = SqlDatabaseManager.ExecuteScalar<bool>(command, SqlConnectionString.Altea);
            }

            return isLocked;
        }

        public override bool UnlockUser(string username)
        {
            return true;
            //return AccountService.UnlockUser(userName);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }

            if (!(providerUserKey is Guid))
            {
                throw new ArgumentException("Invalid provided user key", "providerUserKey");
            }

            MembershipUser user = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[aspnet_Membership_GetUserByUserId]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@UserId",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    providerUserKey);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@UpdateLastActivity",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    userIsOnline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@CurrentTimeUtc",
                    ParameterDirection.Input,
                    SqlDbType.DateTime,
                    DateTime.UtcNow);

                SqlDatabaseManager.AddParameter(command, "@ReturnValue", ParameterDirection.ReturnValue, SqlDbType.Int);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            if (reader.Read())
                            {
                                string nullableString1 = reader.IsDBNull(0) ? null : reader.GetString(0);
                                string nullableString2 = reader.IsDBNull(1) ? null : reader.GetString(1);
                                string nullableString3 = reader.IsDBNull(2) ? null : reader.GetString(2);
                                bool boolean1 = reader.GetBoolean(3);
                                DateTime creationDate = reader.GetDateTime(4).ToLocalTime();
                                DateTime lastLoginDate = reader.GetDateTime(5).ToLocalTime();
                                DateTime lastActivityDate = reader.GetDateTime(6).ToLocalTime();
                                DateTime lastPasswordChangedDate = reader.GetDateTime(7).ToLocalTime();
                                string nullableString4 = reader.IsDBNull(8) ? null : reader.GetString(8);
                                bool boolean2 = reader.GetBoolean(9);
                                DateTime lastLockoutDate = reader.GetDateTime(10).ToLocalTime();

                                user = new MembershipUser(
                                    this.Name,
                                    nullableString4,
                                    providerUserKey,
                                    nullableString1,
                                    nullableString2,
                                    nullableString3,
                                    boolean1,
                                    boolean2,
                                    creationDate,
                                    lastLoginDate,
                                    lastActivityDate,
                                    lastPasswordChangedDate,
                                    lastLockoutDate);
                            }
                        });
            }

            return user;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            MembershipUser user = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[aspnet_Membership_GetUserByName]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@ApplicationName",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    this.ApplicationName);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@UserName",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@UpdateLastActivity",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    userIsOnline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@CurrentTimeUtc",
                    ParameterDirection.Input,
                    SqlDbType.DateTime,
                    DateTime.UtcNow);

                SqlDatabaseManager.AddParameter(command, "@ReturnValue", ParameterDirection.ReturnValue, SqlDbType.Int);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        if (reader.Read())
                        {
                            string nullableString1 = reader.IsDBNull(0) ? null : reader.GetString(0);
                            string nullableString2 = reader.IsDBNull(1) ? null : reader.GetString(1);
                            string nullableString3 = reader.IsDBNull(2) ? null : reader.GetString(2);
                            bool boolean1 = reader.GetBoolean(3);
                            DateTime creationDate = reader.GetDateTime(4).ToLocalTime();
                            DateTime lastLoginDate = reader.GetDateTime(5).ToLocalTime();
                            DateTime lastActivityDate = reader.GetDateTime(6).ToLocalTime();
                            DateTime lastPasswordChangedDate = reader.GetDateTime(7).ToLocalTime();
                            Guid guid = reader.GetGuid(8);
                            bool boolean2 = reader.GetBoolean(9);
                            DateTime lastLockoutDate = reader.GetDateTime(10).ToLocalTime();

                            user = new MembershipUser(
                                this.Name,
                                username,
                                guid,
                                nullableString1,
                                nullableString2,
                                nullableString3,
                                boolean1,
                                boolean2,
                                creationDate,
                                lastLoginDate,
                                lastActivityDate,
                                lastPasswordChangedDate,
                                lastLockoutDate);
                        }
                    });
            }

            return user;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
    }
}
