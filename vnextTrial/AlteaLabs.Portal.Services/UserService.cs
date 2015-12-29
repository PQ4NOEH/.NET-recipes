using System;
using System.Linq;
using AlteaLabs.Portal.Contracts.Model;
using AlteaLabs.Portal.Contracts.Services;
using AlteaLabs.Core.Guards;
using AlteaLabs.Core.Service;
using FluentValidation;
using AlteaLabs.Core.Security;
using AlteaLabs.Portal.Contracts.Persitence;
using AlteaLabs.Portal.Services.UserCredentialsErrors;

namespace AlteaLabs.Portal.Services
{
    public class UserService : IUserService
    {
        readonly IValidator<UserCredentials> _credentialValidator;
        readonly IHashGenerator _hashGenerator;
        readonly IUserRepository _userRepository;
        public UserService(
            IValidator<UserCredentials> credentialValidator, 
            IHashGenerator hashGenerator,
            IUserRepository userRepository)
        {
            _credentialValidator = credentialValidator;
            _hashGenerator = hashGenerator;
            _userRepository = userRepository;
        }
        public ServiceResult<User> Login(NotNullable<UserCredentials> credentials)
        {
            ServiceResult<User>  result = null;
            var validation = _credentialValidator.Validate(credentials);
            if(validation.IsValid)
            {
                var user = _userRepository.LoadUser(credentials.Value.UserNameOrUserMail);
                if(user.Password.Equals(_hashGenerator.GenerateHash(credentials.Value.Password)))
                {
                    result = new ServiceResult<User>(user);
                }
                else result = new ServiceResult<User>(new IServiceError[] { new UserOrPasswordDoesNotMatch() });
            }
            else
            {
                result = new ServiceResult<User>(validation.Errors.Select(v => v.CustomState as IServiceError));
            }
            return result;
        }
        public ServiceResult<User> Register(NotNullable<UserRegistration> registration)
        {
            throw new NotImplementedException();
        }
        public void LogOut(Guid sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
