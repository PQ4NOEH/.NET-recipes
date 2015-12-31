namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Models.Dictionary;

    [ServiceContract]
    public interface IDeanContract : IContract
    {
    }

    [ContractData(RelayName = "dictionary")]
    public interface IDeanChannel : IDeanContract, IClientChannel
    {
    }
}
