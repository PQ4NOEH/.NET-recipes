using System;

namespace AlteaLabs.Core.Cqrs
{
    public interface ICommandHandlerError
    {
        Guid Id { get; }
        string Message { get; }
    }
}
