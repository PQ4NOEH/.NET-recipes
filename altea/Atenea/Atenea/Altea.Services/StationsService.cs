namespace Altea.Services
{
    using System.Data;
    using System.Data.SqlClient;

    using Altea.Contracts;
    using Altea.Database;
    using Altea.Models.Stations;

    public class StationsService : IService, IStationsContract
    {
        public long CreateStation(StationModel model)
        {
            long id;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STATIONS_Create]"))
            {
                this.AddStationParameters(model, command);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Output,
                    SqlDbType.BigInt);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                id = (long)command.Parameters["@id"].Value;
            }

            return id;
        }

        public bool EditStation(long id, StationModel model)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STATIONS_Edit]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                this.AddStationParameters(model, command);
                
                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        private void AddStationParameters(StationModel model, SqlCommand command)
        {
            SqlDatabaseManager.AddParameter(
                command,
                "@name",
                ParameterDirection.Input,
                SqlDbType.NVarChar,
                model.Name);

            SqlDatabaseManager.AddParameter(
                command,
                "@type",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Type);

            SqlDatabaseManager.AddParameter(
                command,
                "@stations",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Stations);
        }

        public void DeleteStation(long id)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STATIONS_Delete]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.Altea);
            }
        }

        public long AddReservationDate(long id, StationReservationModel model)
        {
            long reservationId;

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(
                CommandType.StoredProcedure,
                "[dbo].[STATIONS_AddReservation]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                this.AddStationReservationParameters(model, command);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@reservation_id",
                    ParameterDirection.Output,
                    SqlDbType.BigInt);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                reservationId = (long)command.Parameters["@reservation_id"].Value;
            }

            return reservationId;
        }

        public bool EditReservationDate(long id, long reservationId, StationReservationModel model)
        {
            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STATIONS_EditReservation]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@reservation_id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    reservationId);

                this.AddStationReservationParameters(model, command);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);
                status = (int)command.Parameters["@status"].Value == 1;
            }

            return status;
        }

        private void AddStationReservationParameters(StationReservationModel model, SqlCommand command)
        {
            SqlDatabaseManager.AddParameter(
                command,
                "@type",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Type);

            SqlDatabaseManager.AddParameter(
                command,
                "@frequency",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Frequency);

            SqlDatabaseManager.AddParameter(
                command,
                "@stations",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Stations);

            SqlDatabaseManager.AddParameter(
                command,
                "@weekday",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.WeekDay);

            SqlDatabaseManager.AddParameter(
                command,
                "@hour",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Hour);

            SqlDatabaseManager.AddParameter(
                command,
                "@minute",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Minute);

            SqlDatabaseManager.AddParameter(
                command,
                "@duration",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.Duration);

            SqlDatabaseManager.AddParameter(
                command,
                "@offset_date",
                ParameterDirection.Input,
                SqlDbType.Int,
                model.OffsetDate);

            SqlDatabaseManager.AddParameter(
                command,
                "@start_date",
                ParameterDirection.Input,
                SqlDbType.Date,
                model.StartDate);

            SqlDatabaseManager.AddParameter(
                command,
                "@end_date",
                ParameterDirection.Input,
                SqlDbType.Date,
                model.EndDate);
        }

        public void DeleteReservationDate(long id, long reservationId)
        {
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(
                CommandType.StoredProcedure,
                "[dbo].[STATIONS_DeleteReservation"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    id);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@reservation_id",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    reservationId);

                SqlDatabaseManager.ExecuteNonQueryAsync(command, SqlConnectionString.Altea);
            }
        }
    }
}
