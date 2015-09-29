using System;
using System.Diagnostics.Contracts;
namespace TypeValidation.CodeContract
{
    [ContractClass(typeof(CitizenContract))]
    public interface ICitizen
    {
        int Age { get; set; }
        DateTime BirtDate { get; set; }
        string Name { get; set; }
    }
}
