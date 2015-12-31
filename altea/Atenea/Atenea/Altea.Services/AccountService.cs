namespace Altea.Services
{
    using System;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    using Altea.Contracts;
    using Altea.Models.Account;

    public class AccountService : IService, IAccountContract
    {
        private const int SALT_SIZE = 16;

        /// <summary>
        /// Valid username pattern:
        /// 1. Username must be between 6 and 24 characters long.
        /// 1. Only contains alphanumeric characters, underscores and dots.
        /// 2. Underscore and dot can't be at the start or end of a username.
        /// </summary>
        private const string USERNAME_PATTERN = @"^(?=.{6,24}$)(?![_.])[a-zA-Z0-9_.](?<![_.])$";


        public void CreateUser(string username, string password, string email, RegisterModel model)
        {

            if (!ValidateUsername(ref username))
            {

            }
            else if (!ValidatePassword(ref password))
            {

            }
            else if (!ValidateEmail(ref email))
            {

            }
            else
            {
                string salt = GenerateSalt();
                string encodedPassword = EncodePassword(ref password, ref salt);


            }
            /*
            if (!SecUtility.ValidateParameter(ref password, true, true, false, 128))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return (MembershipUser)null;
            }
            else
            {

                if (!SecUtility.ValidateParameter(ref username, true, true, true, 256))
                {
                    status = MembershipCreateStatus.InvalidUserName;
                    return (MembershipUser)null;
                }
                else if (
                    !SecUtility.ValidateParameter(
                        ref email,
                        this.RequiresUniqueEmail,
                        this.RequiresUniqueEmail,
                        false,
                        256))
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return (MembershipUser)null;
                }
                else if (providerUserKey != null && !(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return (MembershipUser)null;
                }
                else if (password.Length < this.MinRequiredPasswordLength)
                {
                    status = MembershipCreateStatus.InvalidPassword;
                    return (MembershipUser)null;
                }
                else
                {
                    int num1 = 0;
                    for (int index = 0; index < password.Length; ++index)
                    {
                        if (!char.IsLetterOrDigit(password, index)) ++num1;
                    }
                    if (num1 < this.MinRequiredNonAlphanumericCharacters)
                    {
                        status = MembershipCreateStatus.InvalidPassword;
                        return (MembershipUser)null;
                    }
                    else if (this.PasswordStrengthRegularExpression.Length > 0
                             && !Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
                    {
                        status = MembershipCreateStatus.InvalidPassword;
                        return (MembershipUser)null;
                    }
                    else
                    {
                        ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, password, true);
                        this.OnValidatingPassword(e);
                        if (e.Cancel)
                        {
                            status = MembershipCreateStatus.InvalidPassword;
                            return (MembershipUser)null;
                        }
                        else
                        {
                            try
                            {
                                SqlConnectionHolder connectionHolder = (SqlConnectionHolder)null;
                                try
                                {
                                    connectionHolder =
                                        SqlConnectionHelper.GetConnection(this._sqlConnectionString, true);
                                    this.CheckSchemaVersion(connectionHolder.Connection);
                                    DateTime dateTime = this.RoundToSeconds(DateTime.UtcNow);
                                    SqlCommand sqlCommand = new SqlCommand(
                                        "dbo.aspnet_Membership_CreateUser",
                                        connectionHolder.Connection);
                                    sqlCommand.CommandTimeout = this.CommandTimeout;
                                    sqlCommand.CommandType = CommandType.StoredProcedure;
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@ApplicationName",
                                            SqlDbType.NVarChar,
                                            (object)this.ApplicationName));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam("@UserName", SqlDbType.NVarChar, (object)username));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam("@Password", SqlDbType.NVarChar, (object)str1));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam("@PasswordSalt", SqlDbType.NVarChar, (object)salt));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam("@Email", SqlDbType.NVarChar, (object)email));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@PasswordQuestion",
                                            SqlDbType.NVarChar,
                                            (object)passwordQuestion));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@PasswordAnswer",
                                            SqlDbType.NVarChar,
                                            (object)str2));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@IsApproved",
                                            SqlDbType.Bit,
                                            (object)(bool)(isApproved ? 1 : 0)));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@UniqueEmail",
                                            SqlDbType.Int,
                                            (object)(this.RequiresUniqueEmail ? 1 : 0)));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@PasswordFormat",
                                            SqlDbType.Int,
                                            (object)this.PasswordFormat));
                                    sqlCommand.Parameters.Add(
                                        this.CreateInputParam(
                                            "@CurrentTimeUtc",
                                            SqlDbType.DateTime,
                                            (object)dateTime));l
                                    SqlParameter inputParam = this.CreateInputParam(
                                        "@UserId",
                                        SqlDbType.UniqueIdentifier,
                                        providerUserKey);
                                    inputParam.Direction = ParameterDirection.InputOutput;
                                    sqlCommand.Parameters.Add(inputParam);
                                    SqlParameter sqlParameter = new SqlParameter("@ReturnValue", SqlDbType.Int);
                                    sqlParameter.Direction = ParameterDirection.ReturnValue;
                                    sqlCommand.Parameters.Add(sqlParameter);
                                    try
                                    {
                                        sqlCommand.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        if (ex.Number == 2627 || ex.Number == 2601 || ex.Number == 2512)
                                        {
                                            status = MembershipCreateStatus.DuplicateUserName;
                                            return (MembershipUser)null;
                                        }
                                        else throw;
                                    }
                                    int num2 = sqlParameter.Value != null ? (int)sqlParameter.Value : -1;
                                    if (num2 < 0 || num2 > 11) num2 = 11;
                                    status = (MembershipCreateStatus)num2;
                                    if (num2 != 0) return (MembershipUser)null;
                                    providerUserKey =
                                        (object)new Guid(sqlCommand.Parameters["@UserId"].Value.ToString());
                                    dateTime = dateTime.ToLocalTime();
                                    return new MembershipUser(
                                        this.Name,
                                        username,
                                        providerUserKey,
                                        email,
                                        passwordQuestion,
                                        (string)null,
                                        isApproved,
                                        false,
                                        dateTime,
                                        dateTime,
                                        dateTime,
                                        dateTime,
                                        new DateTime(1754, 1, 1));
                                }
                                finally
                                {
                                    if (connectionHolder != null) connectionHolder.Close();
                                }
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                }
            }
        
        */
        }

        private static bool ValidateUsername(ref string username)
        {
            return username != null && Regex.IsMatch(username, USERNAME_PATTERN);
        }

        private static bool ValidatePassword(ref string password)
        {
            return true;
        }

        private static bool ValidateEmail(ref string email)
        {
            return true;
        }

        private static string GenerateSalt()
        {
            byte[] numArray = new byte[SALT_SIZE];
            new RNGCryptoServiceProvider().GetBytes(numArray);
            return Convert.ToBase64String(numArray);
        }

        private static string EncodePassword(ref string password, ref string salt)
        {
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] buffer = new byte[saltBytes.Length + password.Length];
            Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, buffer, saltBytes.Length, passwordBytes.Length);

            // ReSharper disable once PossibleNullReferenceException
            byte[] hashedPassword = hashAlgorithm.ComputeHash(buffer);

            return Convert.ToBase64String(hashedPassword);
        }
    }
}
