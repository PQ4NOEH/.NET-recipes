using AlteaLabs.Core.Cqrs;
using System;

namespace AlteaLabs.Portal.Contracts.Commands
{
    public class LogOutUser : Command
    {
        public Guid UserId { get; set; }
    }
}
