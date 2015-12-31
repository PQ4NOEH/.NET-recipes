using AlteaLabs.Core.Cqrs;

namespace AlteaLabs.Portal.Contracts.Commands
{
    public class LoginUser : Command
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
