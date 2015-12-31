using AlteaLabs.Core.Cqrs;

namespace AlteaLabs.Portal.Contracts.Commands
{
    public class ChangeUserPassword : Command
    {
        public string Password { get; set; }
        public string PasswordRepited { get; set; }
    }
}
