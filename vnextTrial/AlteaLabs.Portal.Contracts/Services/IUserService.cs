using AlteaLabs.Core.Guards;
using AlteaLabs.Core.Service;
using AlteaLabs.Portal.Contracts.Model;
using System;

namespace AlteaLabs.Portal.Contracts.Services
{
    public interface IUserService
    {
        ServiceResult<User> Login(NotNullable<UserCredentials> credentials);
        ServiceResult<User> Register(NotNullable<UserRegistration> registration);
        void LogOut(Guid sessionId);
    }
}
