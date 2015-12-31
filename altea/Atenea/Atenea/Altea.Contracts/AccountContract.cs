namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Models.Account;

    [ServiceContract]
    public interface IAccountContract
    {
        [OperationContract]
        void CreateUser(string username, string password, string email, RegisterModel model);
    }

    [ContractData(RelayName = "account")]
    public interface IAccountChannel : IAccountContract, IClientChannel
    {
    }
}
