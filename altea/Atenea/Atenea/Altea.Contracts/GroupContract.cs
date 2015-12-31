namespace Altea.Contracts
{
    using System;
    using System.ServiceModel;

    using Altea.Models.Admin;

    [ServiceContract]
    public interface IGroupContract : IContract
    {
        Guid CreateGroup();

        bool EditGroup();

        bool DeleteGroup();

        bool ToggleEnableGroup();

        bool AddUserToGroup();

        bool DeleteUserFromGroup();

        [OperationContract]
        bool SetGroupLevels(AdminSetMemberLevelsModel model);
    }

    [ContractData(RelayName = "group")]
    public interface IGroupChannel : IGroupContract, IClientChannel
    {
    }
}
