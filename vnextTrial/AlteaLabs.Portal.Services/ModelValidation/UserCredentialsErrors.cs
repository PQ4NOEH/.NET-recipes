using AlteaLabs.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.Portal.Services.UserCredentialsErrors
{
    public class UserMailEmpty : ServiceError
    {
        public UserMailEmpty() 
            :  base(Guid.Parse("{6329EC7C-847D-41F7-9149-C163B6A6158D}"), "UserNameOrUserMail must be provided")
        { }
    }

    public class UserMailInvalid : ServiceError
    {
        public UserMailInvalid() 
            : base(Guid.Parse("{02F738E8-8A51-4A18-BBB3-C2D8B35085E1}"), "The email is not a valid Email.")
        { }
    }

    

    public class PasswordEmpty : ServiceError
    {
        public PasswordEmpty()
            : base(Guid.Parse("{5989C4C2-0DC4-4E00-A2CF-3E7B557D7A68}"), "Password must be provided.")
        { }
    }

    public class PasswordInvalidLength : ServiceError
    {
        public PasswordInvalidLength()
            : base(Guid.Parse("{EC9EFD27-6E22-4B7C-824A-8AC79BF30773}"), "Password must have a minimum of five characters and a maximum of fifteen")
        { }
    }

    public class UserOrPasswordDoesNotMatch : ServiceError
    {
        public UserOrPasswordDoesNotMatch()
            : base(Guid.Parse("{AB05EA7E-3690-4D04-8C9B-A0B70C7E8BE7}"), "The email and password does not match")
        { }
    }
}
