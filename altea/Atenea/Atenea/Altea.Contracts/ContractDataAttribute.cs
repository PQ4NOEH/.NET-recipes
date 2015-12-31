using System;

namespace Altea.Contracts
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class ContractDataAttribute : Attribute
    {
        public string RelayName { get; set; }
    }
}
