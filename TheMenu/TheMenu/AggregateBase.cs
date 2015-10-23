using System;

namespace TheMenu.Core
{
    public abstract class AggregateBase : IAggregate
    {
        public Guid Id { get; protected set; }

    }
}
