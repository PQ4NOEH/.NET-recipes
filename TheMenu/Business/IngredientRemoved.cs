
using System;
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class IngredientRemoved:Ingredient, IEvent
    {
        public Guid AggregateId { get; private set; }
        public IngredientRemoved(Guid aggregateId, Guid ingredientId, NotNullOrWhiteSpaceString quantity)
            : base(ingredientId, quantity)
        {
            AggregateId = aggregateId;
        }
    }
}
