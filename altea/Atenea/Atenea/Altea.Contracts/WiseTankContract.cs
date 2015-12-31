namespace Altea.Contracts
{
    using System;
    using System.ServiceModel;

    using Altea.Classes.WiseTank;
    using Altea.Models;
    using Altea.Models.WiseTank;

    [ServiceContract]
    public interface IWiseTankContract : IContract
    {
        [OperationContract]
        void CheckDefaultTimelines(UserDataBasicModel model);

        [OperationContract]
        void EditBoxWidth(WiseTankBoxWidthModel model);

        [OperationContract]
        void EditBoxRefreshRate(WiseTankBoxRefreshRateModel model);

        [OperationContract]
        Guid CreateStream(WiseTankCreateModel model);

        [OperationContract]
        WiseTankError EditStreamName(WiseTankEditStreamModel model);

        [OperationContract]
        void RepositionStream(WiseTankRepositionStreamModel model);

        [OperationContract]
        WiseTankError DeleteStream(WiseTankStreamModel model);

        [OperationContract]
        Guid CreateBox(WiseTankCreateBoxModel model);

        [OperationContract]
        void RepositionBox(WiseTankRepositionBoxModel model);

        [OperationContract]
        WiseTankError DeleteBox(WiseTankBoxModel model);



        [OperationContract]
        void EditGroupBoxWidth(WiseTankBoxWidthModel model);

        [OperationContract]
        Guid CreateTimeline(WiseTankCreateTimelineModel model);

        [OperationContract]
        WiseTankError EditTimeline(WiseTankEditTimelineModel model);

        [OperationContract]
        void SetTimelineArea(WiseTankSetTimelineAreaModel model);

        [OperationContract]
        Guid AddTimelineColumn(WiseTankAddTimelineColumnModel model);

        [OperationContract]
        WiseTankError AddTimelineUser(WiseTankAddTimelineUserModel model);

        [OperationContract]
        WiseTankError EditTimelineUser(WiseTankAddTimelineUserModel model);




        [OperationContract]
        WiseTankError CreateArticle(WiseTankCreateArticleModel model);

        [OperationContract]
        int? Vote(WiseTankVoteModel model);

        [OperationContract]
        decimal? Karma(WiseTankKarmaModel model);
    }

    [ContractData(RelayName = "wisetank")]
    public interface IWiseTankChannel : IWiseTankContract, IClientChannel
    {
    }
}
