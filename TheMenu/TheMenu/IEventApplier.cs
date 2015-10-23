using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMenu.Core
{
    public interface IEventApplier<T> where T : IAggregate
    {
        void ApplyEvents(T aggregate, IEnumerable<IEvent> @event);
    }
}
