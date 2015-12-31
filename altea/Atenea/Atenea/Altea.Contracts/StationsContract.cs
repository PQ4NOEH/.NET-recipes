namespace Altea.Contracts
{
    using System.ServiceModel;

    using Altea.Models.Stations;

    [ServiceContract]
    public interface IStationsContract : IContract
    {
        [OperationContract]
        long CreateStation(StationModel model);

        [OperationContract]
        bool EditStation(long id, StationModel model);

        [OperationContract]
        void DeleteStation(long id);

        [OperationContract]
        long AddReservationDate(long id, StationReservationModel model);

        [OperationContract]
        bool EditReservationDate(long id, long reservationId, StationReservationModel model);

        [OperationContract]
        void DeleteReservationDate(long id, long reservationId);
    }

    [ContractData(RelayName = "stations")]
    public interface IStationsChannel : IStationsContract, IClientChannel
    {
    }
}
