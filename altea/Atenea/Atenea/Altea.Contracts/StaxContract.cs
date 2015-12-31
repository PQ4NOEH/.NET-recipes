namespace Altea.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using Altea.Models.Stax;

    [ServiceContract]
    public interface IStaxContract : IContract
    {
        #region Inbox

        /// <returns>inbox data identifier</returns>
        [OperationContract]
        int InsertInboxData(StackNewInboxDataModel model);

        [OperationContract]
        void AcceptInboxData(StackAcceptInboxDataModel model);

        [OperationContract]
        void DeleteInboxData(StackDeleteInboxDataModel model);

        #endregion

        #region Stacks

        [OperationContract]
        void CheckStaxExist(StaxCheckModel model);

        /// <returns>list of data returned to the inbox</returns>
        [OperationContract]
        IEnumerable<string> SaveExercise(StackSaveExerciseDataModel model);

        #endregion
    }

    [ContractData(RelayName = "stax")]
    public interface IStaxChannel : IStaxContract, IClientChannel
    {
    }
}
