
using System;
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class Ingredient : IValueObject
    {
        public readonly Guid Id;
        public readonly string Quantity;
        public Ingredient(Guid id, NotNullOrWhiteSpaceString quantity)
        {
            Id = id;
            Quantity = quantity;
        }
    }
}
