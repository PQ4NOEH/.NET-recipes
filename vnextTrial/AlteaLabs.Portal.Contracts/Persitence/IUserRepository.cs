using AlteaLabs.Core.Guards;
using AlteaLabs.Portal.Contracts.Model;

namespace AlteaLabs.Portal.Contracts.Persitence
{
    public interface IUserRepository
    {
        User LoadUser(NotNulllEmptyOrWhiteSpaceString userMail);

    }
}
