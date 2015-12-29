using AlteaLabs.Portal.Contracts.Model;
using AlteaLabs.Portal.Services.UserCredentialsErrors;
using FluentValidation;


namespace AlteaLabs.Portal.Services.ModelValidation
{
    public class UserCredentialsValidation : AbstractValidator<UserCredentials>
    {
        public UserCredentialsValidation()
        {
            RuleFor(c => c.UserNameOrUserMail).NotNull().WithState(x => new UserNameOrUserMailEmpty());
            RuleFor(c => c.UserNameOrUserMail).Length(3, 50).WithState(x => new UserNameOrUserMailEmpty());
            RuleFor(c => c.UserNameOrUserMail).Matches(@"^[a - zA - Z0 - 9_@.]$").WithState(x => new UserNameOrUserMailInvalidCharacters());

            RuleFor(c => c.Password).NotNull().WithState(x => new PasswordEmpty());
            RuleFor(c => c.Password).Length(5, 15).WithState(x => new PasswordInvalidLength()); 
        }
    }
}
