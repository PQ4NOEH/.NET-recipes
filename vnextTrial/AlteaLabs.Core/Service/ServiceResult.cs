using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.Core.Service
{
    public class ServiceResult<T> where T : new()
    {
        public readonly T Data;
        public readonly IEnumerable<IServiceError> Errors;

        public ServiceResult(IEnumerable<IServiceError> errors)
        {
            Errors = errors;
        }

        public ServiceResult(T data)
        {
            Data = data;
        }
    }
}
