using AlteaLabs.Core.Guards;
using System;

namespace AlteaLabs.Core.Cqrs
{

    public abstract class CommandHandlerError : ICommandHandlerError
    {
        public Guid Id { get; private set; }

        public string Message { get; private set; }

        public CommandHandlerError(Guid id, NotNulllEmptyOrWhiteSpaceString message)
        {
            Id = id;
            Message = message;
        }
    }
}
