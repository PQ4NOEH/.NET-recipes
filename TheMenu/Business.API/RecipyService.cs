using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMenu.Core;

namespace Business.API
{
    public class RecipyService
    {
        readonly IEventStore _eventStore;
        public RecipyService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task ChangeName(Guid recipyId, string newName)
        {
            IRecipy aggregate = await GetRecipy(recipyId.ToString());

            var nameChanged = aggregate.Execute(new ChangeName(newName));
            await _eventStore.AppendEvent(aggregate.Id.ToString(), nameChanged);
        }
        public async Task AddIngredient(Guid recipyId, Guid ingredientId, string quantity)
        {
            IRecipy aggregate = await GetRecipy(recipyId.ToString());
            var ingredientAdded = aggregate.Execute(new AddIngredient(ingredientId, quantity));
            await _eventStore.AppendEvent(recipyId.ToString(), ingredientAdded);
        }

        async Task<IRecipy> GetRecipy(string recipyId)
        {
            var events = await _eventStore.GetEventsFromBucket(recipyId);
            return new Recipy().LoadState(events);
        }
    }
}
