using System.Collections.Generic;

namespace AlteaLabs.Core.Cqrs
{
    public interface ICommandHandlerResult
    {
        IEnumerable<ICommandHandlerError> Errors { get; }
    }
}
