using Davalor.PortalPaciente.Messages.Payable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    class PayableRepository : BaseRepository<PayableAggregate>
    {
        public PayableRepository(string DbConnectionString) : base(DbConnectionString) { }
        public PayableAggregate Get(Guid id)
        {
            var query = "SELECT [Id],[CurrencyId],[PaymentStatus],[Price],[TimeStamp] " +
                "FROM [VisionLocalIntegrationTests].[dbo].[Payable] where id = '" + id.ToString() + "'";
            var resultGraph = this.Query(query, r =>
                {
                    return new PayableAggregate
                        {
                            Id = r.GetGuid(0),
                            CurrencyId = r.GetGuid(1),
                            PaymentStatus = r.GetInt32(2),
                            Price = r.GetFloat(3),
                            TimeStamp = r.GetDateTimeOffset(4)
                        };
                });
            if (resultGraph == null) return null;
            query = "SELECT T.Id, T.CurrencyId, T.PayableId,T.PaymentId, T.PartnerId, T.Amount,T.TimeStamp," +
                " P.Id ,P.Amount,P.CurrencyId,P.FreeSessionReasonId,P.GatewayId,P.OperationType,P.PatientId,P.PaymentDate,P.PaymentType,P.ProcessingState,P.RequestDate,P.TimeStamp," +
                " PM.Id, PM.AuthNumber, PM.BankingNumber, PM.OperationNumber, PM.Sequence, PM.TimeStamp" +
                "  FROM [VisionLocalIntegrationTests].[dbo].[PaymentTransaction] T" +
                " INNER JOIN [VisionLocalIntegrationTests].[dbo].[Payment] P ON T.PAYMENTID = P.ID" +
                " INNER JOIN [VisionLocalIntegrationTests].[dbo].[PaymentMpos] PM ON PM.ID = P.ID" + 
                " where T.PayableId = '" + resultGraph.Id.ToString() + "'";
            resultGraph.PaymentTransaction = this.Query(query, r =>
                {
                    return new PayableAggregate
                        {
                            PaymentTransaction = new Collection<PaymentTransaction>
                            {
                                new PaymentTransaction
                                {
                                    Id = r.GetGuid(0),
                                    CurrencyId = r.GetGuid(1),
                                    PayableId = r.GetGuid(2),
                                    PaymentId = r.GetGuid(3),
                                    PartnerId = r.GetGuid(4),
                                    Amount = r.GetDecimal(5),
                                    TimeStamp = r.GetDateTimeOffset(6),
                                    Payment = new PaymentEntity
                                    {
                                        Id = r.GetGuid(7),
                                        Amount = r.GetDecimal(8),
                                        CurrencyId = r.GetGuid(9),
                                        FreeSessionReasonId = r.GetGuid(10),
                                        GatewayId = r.GetGuid(11),
                                        OperationType = r.GetInt32(12),
                                        PatientId = r.GetGuid(13),
                                        PaymentDate = r.GetDateTime(14),
                                        PaymentType = r.GetInt32(15),
                                        ProcessingState = r.GetInt32(16),
                                        RequestDate = r.GetDateTime(17),
                                        TimeStamp = r.GetDateTimeOffset(18),
                                        PaymentMpos = new PaymentMpos
                                        {
                                            Id = r.GetGuid(19),
                                            AuthNumber = r.GetString(20),
                                            BankingNumber = r.GetString(21),
                                            OperationNumber = r.GetString(22),
                                            Sequence = r.GetString(23),
                                            TimeStamp = r.GetDateTimeOffset(24),
                                        }
                                    }
                                }
                            }
                        };
                }).PaymentTransaction;
            if (resultGraph == null) return null;
           
            resultGraph.PaymentTransaction.ToList().ForEach(t =>
                {
                     query = "SELECT [Id],[PaymentTransactionId],[TaxId],[Amount],[NameKeyId],[TimeStamp] " +
                            "FROM [VisionLocalIntegrationTests].[dbo].[PaymentTaxTransaction] where PaymentTransactionId = '" + t.Id.ToString() + "'";
                     t.PaymentTaxTransaction = this.Query(query, r =>
                         {
                             return new PayableAggregate
                             {
                                 PaymentTransaction = new Collection<PaymentTransaction>
                                {
                                    new PaymentTransaction
                                    {
                                        PaymentTaxTransaction = new Collection<PaymentTaxTransaction>
                                        {
                                            new PaymentTaxTransaction
                                            {
                                                Id = r.GetGuid(0),
                                                PaymentTransactionId = r.GetGuid(1),
                                                TaxId = r.GetGuid(2),
                                                Amount = r.GetDecimal(3),
                                                NameKeyId = r.GetString(4),
                                                TimeStamp = r.GetDateTimeOffset(5)
                                            }
                                        }
                                    }
                                }
                             };
                         }).PaymentTransaction.First().PaymentTaxTransaction;
                }); 
            
            return resultGraph;
        }

        public void Insert(PayableAggregate aggregate)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Payable]([Id],[CurrencyId],[PaymentStatus],[Price],[TimeStamp])" +
                            " VALUES (@Id,@CurrencyId,@PaymentStatus,@Price,@TimeStamp)";
                        cmd.Parameters.Add(new SqlParameter("@Id", aggregate.Id));
                        cmd.Parameters.Add(new SqlParameter("@CurrencyId", aggregate.CurrencyId));
                        cmd.Parameters.Add(new SqlParameter("@PaymentStatus", aggregate.PaymentStatus));
                        cmd.Parameters.Add(new SqlParameter("@Price", aggregate.Price));
                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", aggregate.TimeStamp));
                        cmd.ExecuteNonQuery();
                    }

                    aggregate.PaymentTransaction.ToList().ForEach(t =>
                        {
                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[Payment]" +
                                    " (Id ,Amount,CurrencyId,FreeSessionReasonId,GatewayId,OperationType,PatientId,PaymentDate,PaymentType,ProcessingState,RequestDate,TimeStamp)" +
                                    " VALUES (@Id ,@Amount, @CurrencyId, @FreeSessionReasonId, @GatewayId, @OperationType, @PatientId, @PaymentDate, @PaymentType, @ProcessingState, @RequestDate, @TimeStamp)";

                                cmd.Parameters.Add(new SqlParameter("@Id", t.Payment.Id));
                                cmd.Parameters.Add(new SqlParameter("@Amount", t.Payment.Amount));
                                cmd.Parameters.Add(new SqlParameter("@CurrencyId", t.Payment.CurrencyId));
                                cmd.Parameters.Add(new SqlParameter("@FreeSessionReasonId", t.Payment.FreeSessionReasonId));
                                cmd.Parameters.Add(new SqlParameter("@GatewayId", t.Payment.GatewayId));
                                cmd.Parameters.Add(new SqlParameter("@OperationType", t.Payment.OperationType));
                                cmd.Parameters.Add(new SqlParameter("@PatientId", t.Payment.PatientId));
                                cmd.Parameters.Add(new SqlParameter("@PaymentDate", t.Payment.PaymentDate));
                                cmd.Parameters.Add(new SqlParameter("@PaymentType", t.Payment.PaymentType));
                                cmd.Parameters.Add(new SqlParameter("@ProcessingState", t.Payment.ProcessingState));
                                cmd.Parameters.Add(new SqlParameter("@RequestDate", t.Payment.RequestDate));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", t.Payment.TimeStamp));
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[PaymentMpos]"+
                                    "(Id, AuthNumber, BankingNumber, OperationNumber, Sequence, TimeStamp)" +
                                    " VALUES (@Id, @AuthNumber, @BankingNumber, @OperationNumber, @Sequence, @TimeStamp)";

                                cmd.Parameters.Add(new SqlParameter("@Id", t.Payment.PaymentMpos.Id));
                                cmd.Parameters.Add(new SqlParameter("@AuthNumber", t.Payment.PaymentMpos.AuthNumber));
                                cmd.Parameters.Add(new SqlParameter("@BankingNumber", t.Payment.PaymentMpos.BankingNumber));
                                cmd.Parameters.Add(new SqlParameter("@OperationNumber", t.Payment.PaymentMpos.OperationNumber));
                                cmd.Parameters.Add(new SqlParameter("@Sequence", t.Payment.PaymentMpos.Sequence));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", t.Payment.PaymentMpos.TimeStamp));

                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[PaymentTransaction]" +
                                    "(Id, CurrencyId, PayableId,PaymentId, PartnerId, Amount,TimeStamp)" +
                                    " VALUES (@Id, @CurrencyId, @PayableId,@PaymentId, @PartnerId, @Amount, @TimeStamp)";

                                cmd.Parameters.Add(new SqlParameter("@Id", t.Id));
                                cmd.Parameters.Add(new SqlParameter("@CurrencyId", t.CurrencyId));
                                cmd.Parameters.Add(new SqlParameter("@PayableId", t.PayableId));
                                cmd.Parameters.Add(new SqlParameter("@PaymentId", t.PaymentId));
                                cmd.Parameters.Add(new SqlParameter("@PartnerId", t.PartnerId));
                                cmd.Parameters.Add(new SqlParameter("@Amount", t.Amount));
                                cmd.Parameters.Add(new SqlParameter("@TimeStamp", t.TimeStamp));

                                cmd.ExecuteNonQuery();
                            }
                            t.PaymentTaxTransaction.ToList().ForEach(tt =>
                                {
                                    using (SqlCommand cmd = connection.CreateCommand())
                                    {
                                        cmd.Transaction = transaction;
                                        cmd.CommandText = "INSERT INTO [VisionLocalIntegrationTests].[dbo].[PaymentTaxTransaction]" +
                                            "([Id],[PaymentTransactionId],[TaxId],[Amount],[NameKeyId],[TimeStamp])" +
                                            " VALUES (@Id,@PaymentTransactionId,@TaxId,@Amount,@NameKeyId,@TimeStamp)";

                                        cmd.Parameters.Add(new SqlParameter("@Id", tt.Id));
                                        cmd.Parameters.Add(new SqlParameter("@PaymentTransactionId", tt.PaymentTransactionId));
                                        cmd.Parameters.Add(new SqlParameter("@TaxId", tt.TaxId));
                                        cmd.Parameters.Add(new SqlParameter("@Amount", tt.Amount));
                                        cmd.Parameters.Add(new SqlParameter("@NameKeyId", tt.NameKeyId));
                                        cmd.Parameters.Add(new SqlParameter("@TimeStamp", tt.TimeStamp));

                                        cmd.ExecuteNonQuery();
                                    }
                                });
                        });

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
