using System;
using System.Linq;
using System.Collections.Generic;
using TheMenu.Core;

namespace Business
{
    public class Recipy : AggregateBase, IRecipy
    {
        public string _name;
        public readonly List<Ingredient> _ingredients = new List<Ingredient>();
        public readonly SortedList<uint, Step> _steps = new SortedList<uint,Step>();
        public byte[] _image;

        public Recipy()
        {
            Id = Guid.NewGuid();
        }

        public IEvent Execute(ChangeName command)
        {
            this._name = command.NewName;
            return new NameChanged(Id, this._name);
        }
        public IEvent Execute(AddIngredient command)
        {
            _ingredients.Add(new Ingredient(command.Id, command.Quantity));
            return new IngredientAdded(Id, command.Id, command.Quantity);
        }
        public IEvent Execute(RemoveIngredient command)
        {
            var removedIngredient =_ingredients.FirstOrDefault(i => i.Id == command.IngredientId);   
            if(removedIngredient == null) return null;

            _ingredients.Remove(removedIngredient);
            return new IngredientRemoved(Id, command.IngredientId, removedIngredient.Quantity);
        }
        public IEvent Execute(AddStep command)
        {
            _steps.Add(command.Order, new Step(command.Order, command.Explanation,command.Image));
            return new StepAdded(Id, command.Order, command.Explanation, command.Image);
        }
        public IEvent Execute(RemoveStep command)
        {
            if(!_steps.ContainsKey(command.StepOrder)) return null;
            _steps.Remove(command.StepOrder);
            return new StepRemoved(Id, command.StepOrder);
        }

        public IEvent Execute(ChangeImage command)
        {
            this._image = command.Image;
            return new ImageChanged(this.Id, command.Image);
        }

        public IRecipy LoadState(IEnumerable<IEvent> events)
        {
            if (events != null) events.ToList().ForEach(e => ApplyEvent((dynamic)e));
            return this;
        }

        void ApplyEvent(NameChanged @event)
        {
            Execute(new ChangeName(@event.NewName));
        }
        void ApplyEvent(IngredientAdded @event)
        {
            Execute(new AddIngredient(@event.Id, @event.Quantity));
        }
        void ApplyEvent(IngredientRemoved @event)
        {
            Execute(new RemoveIngredient(@event.Id));
        }
        void ApplyEvent(StepAdded @event)
        {
            Execute(new AddStep(@event.Order, @event.Explanation));
        }
        void ApplyEvent(StepRemoved @event)
        {
            Execute(new RemoveStep(@event.Order));
        }
        void ApplyEvent(ImageChanged @event)
        {
            Execute(new ChangeImage(@event.Image));
        }
    }
}
