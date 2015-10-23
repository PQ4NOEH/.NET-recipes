using System;
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class AddIngredient : Ingredient, ICommand
    {
        public AddIngredient(Guid id, NotNullOrWhiteSpaceString quantity)
            : base(id, quantity)
        {
        }
    }
}
