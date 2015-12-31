namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Models.Admin;
    using Altea.Models.User;

    [ServiceContract]
    public interface IUserContract : IContract
    {
        [OperationContract]
        string CreateUser(CreateUserModel model);

        [OperationContract]
        bool SetUserLevels(AdminSetMemberLevelsModel model);

        [OperationContract]
        bool SetUserProLevels(AdminSetMemberLevelsModel model);
    }

    [ContractData(RelayName = "users")]
    public interface IUserChannel : IUserContract, IClientChannel
    {
    }
}
