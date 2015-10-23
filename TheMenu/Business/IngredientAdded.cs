
using System;
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class IngredientAdded: Ingredient, IEvent
    {
        public Guid AggregateId { get; private set; }
        public IngredientAdded(Guid aggregateId, Guid ingredientId, NotNullOrWhiteSpaceString quantity)
            : base(ingredientId, quantity)
        {
            AggregateId = aggregateId;
        }
    }
}
