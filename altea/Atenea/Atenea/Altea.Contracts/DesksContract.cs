namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Classes.Desks;
    using Altea.Models.Desks;

    [ServiceContract]
    public interface IDesksContract : IContract
    {
        [OperationContract]
        void AutoUnblock(DesksAutoUnblockModel model);

        [OperationContract]
        void CheckLastBlock(DesksCheckLastBlockModel model);

        [OperationContract]
        void AssignIndex(DesksAssignIndexModel model);

        [OperationContract]
        string StartIndex(DesksIndexModel model);

        [OperationContract]
        DesksCheckResult CheckIndex(DesksCheckIndexModel model);

        [OperationContract]
        DesksFinishResult FinishIndex(DesksFinishIndexModel model);

        [OperationContract]
        void ReportIndex(DesksReportIndexModel model);

        [OperationContract]
        void AssignExam(DesksAssignExamModel model);

        [OperationContract]
        string StartExam(DesksExamModel model);

        [OperationContract]
        DesksCheckResult CheckExam(DesksCheckExamModel model);

        [OperationContract]
        bool FinishExam(DesksFinishExamModel model);

        [OperationContract]
        void ReportExam(DesksReportExamModel model);

        [OperationContract]
        bool AssignExamTest(DesksAssignExamTestModel model);

        [OperationContract]
        void AssignExtra(DesksAssignExtraModel model);
    }

    [ContractData(RelayName = "desks")]
    public interface IDesksChannel : IDesksContract, IClientChannel
    {
    }
}
