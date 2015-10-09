using System;

namespace Report.Domain
{
    public interface IReportIdentity
    {
        Guid SessionId { get; set; }

        Guid CountryId { get; set; }

        Guid PatientId { get; set; }
    }
}
