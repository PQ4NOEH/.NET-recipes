using Report.Domain;
using System;

namespace ComposableWebAPI.Models
{
    public class ReportIdentity : IReportIdentity
    {
        public Guid SessionId { get; set; }

        public Guid CountryId { get; set; }

        public Guid PatientId { get; set; }

        public override string ToString()
        {
            return string.Format("{0}_{1}_{2}", SessionId, CountryId, PatientId);
        }
    }
}