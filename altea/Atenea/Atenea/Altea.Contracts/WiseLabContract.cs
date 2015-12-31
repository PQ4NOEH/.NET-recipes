using System.ServiceModel;

namespace Altea.Contracts
{
    using Altea.Classes.WiseLab;
    using Altea.Models.WiseLab;

    [ServiceContract]
    public interface IWiseLabContract : IContract
    {
        [OperationContract]
        WiseLabError SearchWord(WiseLabHuntDataModel model);

        [OperationContract]
        WiseLabError AddHuntData(WiseLabHuntDataModel model);

        [OperationContract]
        WiseLabError RemoveHuntData(WiseLabHuntDataModel model);

        [OperationContract]
        WiseLabError SaveLead(WiseLabWisdomHunterModel model);

        [OperationContract]
        WiseLabStatus FinishStatus(WiseLabArticleDataModel model);
    }

    [ContractData(RelayName = "wiselab")]
    public interface IWiseLabChannel : IWiseLabContract, IClientChannel
    {
    }
}
