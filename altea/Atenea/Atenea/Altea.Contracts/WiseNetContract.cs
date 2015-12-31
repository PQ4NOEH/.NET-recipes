namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Models;
    using Altea.Models.WiseNet;

    [ServiceContract]
    public interface IWiseNetContract : IContract
    {
        [OperationContract]
        void CheckUserSearchEngines(UserDataBasicModel model);

        [OperationContract]
        int CreateArticle(WiseNetCreateModel model);
    }

    [ContractData(RelayName = "wisenet")]
    public interface IWiseNetChannel : IWiseNetContract, IClientChannel
    {
    }
}
