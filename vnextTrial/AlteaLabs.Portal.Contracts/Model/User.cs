using AlteaLabs.Core.Types;
using System;

namespace AlteaLabs.Portal.Contracts.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public Media Avatar { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
    }
}
