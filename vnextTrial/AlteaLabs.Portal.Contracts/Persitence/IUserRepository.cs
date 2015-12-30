using AlteaLabs.Core.Guards;
using AlteaLabs.Portal.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.Portal.Contracts.Persitence
{
    public interface IUserRepository
    {
        User LoadUser(NotNulllEmptyOrWhiteSpaceString userMail);
        User LoadUser(Guid userId);

    }
}
