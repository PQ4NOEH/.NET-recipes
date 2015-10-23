using Business;
using System;
using System.Collections.Generic;
using TheMenu.Core;
using Xunit;

namespace BusinessTests
{
    public class RecipySpec
    {
        [Fact]
        public void If_the_name_get_changed_it_register_a_new_event_with_the_new_name()
        {
            var sut = new Recipy();
            var command = new ChangeName("Aguado");
            var @event = sut.Execute(command) as NameChanged;
            Assert.NotNull(@event);
            Assert.Equal(@event.NewName, "Aguado");
            Assert.Equal(@event.AggregateId, sut.Id);
        }

       [Fact]
        public void Given_a_bunch_of_events_when_given_to_an_instance_then_load_the_state()
        {
            var sut = new Recipy();
            var events = new List<IEvent>
            {
                new NameChanged(sut.Id, "Anystring"),
                new StepAdded(sut.Id, 0,"Anystringyouwant"),
                new StepRemoved(sut.Id, 0),
                new StepAdded(sut.Id, 0,"A better explanation"),
                new IngredientAdded(sut.Id, Guid.NewGuid(),"two spons")
            };
            sut.LoadState(events);
            Assert.Equal(sut._name, "Anystring");

            Assert.Equal(sut._steps.Count, 1);
            Assert.Equal(sut._steps[0].Order, (uint)0);
            Assert.Equal(sut._steps[0].Explanation, "A better explanation");

            Assert.Equal(sut._ingredients.Count, 1);
            Assert.Equal(sut._ingredients[0].Quantity, "two spons");
        }
    }
}
