using AlteaLabs.Portal.Contracts.Model;
using AlteaLabs.Portal.Services.UserCredentialsErrors;
using FluentValidation;


namespace AlteaLabs.Portal.Services.ModelValidation
{
    public class UserCredentialsValidation : AbstractValidator<UserCredentials>
    {
        public UserCredentialsValidation()
        {
            RuleFor(c => c.UserMail).EmailAddress().WithState(x => new UserMailInvalid());

            RuleFor(c => c.Password).NotNull().WithState(x => new PasswordEmpty());
            RuleFor(c => c.Password).Length(5, 15).WithState(x => new PasswordInvalidLength()); 
        }
    }
}
