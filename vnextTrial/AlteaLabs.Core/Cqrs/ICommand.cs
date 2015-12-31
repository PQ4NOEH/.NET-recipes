using System;

namespace AlteaLabs.Core.Cqrs
{
    public interface ICommand
    {
        Guid Id { get; set; }
        DateTime CreatedDate { get; set; }
    }
}
