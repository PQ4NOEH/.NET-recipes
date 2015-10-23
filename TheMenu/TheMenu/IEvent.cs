
using System;
namespace TheMenu.Core
{
    public interface IEvent
    {
        Guid AggregateId { get; }
    }
}
