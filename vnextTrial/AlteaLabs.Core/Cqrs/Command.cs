using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.Core.Cqrs
{
    public abstract class Command : ICommand
    {
        public DateTime CreatedDate { get; set; }

        public Guid Id { get; set; }
    }
}
