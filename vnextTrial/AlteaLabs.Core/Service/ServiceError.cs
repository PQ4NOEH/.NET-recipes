using AlteaLabs.Core.Guards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.Core.Service
{
    public abstract class ServiceError : IServiceError
    {
        public Guid Id { get; private set; }

        public string Message { get; private set; }

        public ServiceError(Guid id, NotNulllEmptyOrWhiteSpaceString message)
        {
            Id = id;
            Message = message;
        }
    }
}
