namespace Altea.Contracts
{
    using System;
    using System.ServiceModel;

    using Altea.Models;
    using Altea.Models.WiseReader;

    [ServiceContract]
    public interface IWiseReaderContract : IContract
    {
        [OperationContract]
        void CheckDefaultFolder(UserDataBasicModel model);

        [OperationContract]
        Guid CreateFolder(WiseReaderFolderNameModel model);

        [OperationContract]
        int EditFolder(WiseReaderFolderNameModel model);

        [OperationContract]
        int DeleteFolder(WiseReaderFolderModel model);

        [OperationContract]
        void SaveStorageFile(WiseReaderQueueProcessFileModel model);

        [OperationContract]
        void ReferenceUserAndFile(WiseReaderReferenceFileModel model);
    }

    [ContractData(RelayName = "wisereader")]
    public interface IWiseReaderChannel : IWiseReaderContract, IClientChannel
    {
    }
}
