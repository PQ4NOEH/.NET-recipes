using System;
using System.Collections.Generic;
using TheMenu.Core;
namespace Business
{
    public interface IRecipy : IAggregate
    {
        IEvent Execute(AddIngredient command);
        IEvent Execute(AddStep command);
        IEvent Execute(ChangeName command);
        IEvent Execute(RemoveIngredient command);
        IEvent Execute(RemoveStep command);
        IRecipy LoadState(IEnumerable<IEvent> events);
    }
}
