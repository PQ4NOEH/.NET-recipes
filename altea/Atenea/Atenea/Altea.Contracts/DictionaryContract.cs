namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Models.Dictionary;

    [ServiceContract]
    public interface IDictionaryContract : IContract
    {
        [OperationContract]
        void InsertTextToSpeech(DictionaryInsertSpeechModel model);
    }

    [ContractData(RelayName = "dictionary")]
    public interface IDictionaryChannel : IDictionaryContract, IClientChannel
    {
    }
}
