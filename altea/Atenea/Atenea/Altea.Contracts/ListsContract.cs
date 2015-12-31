namespace Altea.Contracts
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IListsContract : IContract
    {
    }

    [ContractData(RelayName = "stacks")]
    public interface IListsChannel : IListsContract, IClientChannel
    {
    }
}
