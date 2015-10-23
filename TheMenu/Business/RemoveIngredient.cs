using System;
using TheMenu.Core;

namespace Business
{
    public class RemoveIngredient : ICommand
    {
        public readonly Guid IngredientId;
        public RemoveIngredient(Guid ingredientId)
        {
            IngredientId = ingredientId;
        }
    }
}
