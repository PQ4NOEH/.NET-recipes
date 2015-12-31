using Altea.Classes;
//using Altea.Classes.Altea;
using Altea.Contracts;
//using Altea.Models.Account;

namespace Heracles.Services
{
    public abstract class AccountService : Service<IAccountChannel>
    {
        public static void CreateUser(string username, string firstname, string lastname, string email, string password,
            string confirmPassword)
        {
            //RegisterModel model = new RegisterModel
            //{
            //    UserName = username,
            //    FirstName = firstname,
            //    LastName = lastname,
            //    Email = email,
            //    Password = password,
            //    ConfirmPassword = confirmPassword
            //};

            //Channel.CreateUser(model);
        }

        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            //ChangePasswordModel model = new ChangePasswordModel
            //{
            //    //User = AlteaUser.Get(username),
            //    OldPassword = oldPassword,
            //    NewPassword = newPassword
            //};
            return true;
            //return Channel.ChangePassword(model);
        }
    }
}
