namespace Altea.Contracts
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IProDesksContract : IContract
    {
    }

    [ContractData(RelayName = "prodesks")]
    public interface IProDesksChannel : IProDesksContract, IClientChannel
    {
    }
}
