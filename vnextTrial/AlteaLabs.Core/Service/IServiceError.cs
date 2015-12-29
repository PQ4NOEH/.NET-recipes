using System;

namespace AlteaLabs.Core.Service
{
    public interface IServiceError
    {
        Guid Id { get; }
        string Message { get; }
    }
}
