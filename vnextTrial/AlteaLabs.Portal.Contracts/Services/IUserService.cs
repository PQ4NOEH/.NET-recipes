using AlteaLabs.Core.Cqrs;
using AlteaLabs.Core.Guards;
using AlteaLabs.Core.Service;
using AlteaLabs.Portal.Contracts.Commands;
using AlteaLabs.Portal.Contracts.Model;
using System;

namespace AlteaLabs.Portal.Contracts.Services
{
    public interface IUserService : 
        ICommandHandler<RegisterUser>, 
        ICommandHandler<LoginUser>
    {
        
    }
}
