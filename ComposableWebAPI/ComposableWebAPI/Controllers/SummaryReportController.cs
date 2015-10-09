using ComposableWebAPI.Models;
using Report.Domain;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace ComposableWebAPI.Controllers
{
    public class SummaryReportController : ApiController
    {
        readonly ISummaryReport _report;

        public SummaryReportController(ISummaryReport report)
        {
            _report = report;
        }
        public async Task<byte[]> Get(Guid sessionId, Guid countryId, Guid patientId)
        {
            var reportIdentity = new ReportIdentity
            {
                SessionId = sessionId,
                CountryId = countryId,
                PatientId = patientId
            };

            return await _report.Get(reportIdentity);
        }

        public async Task Post([FromBody]ReportIdentity req)
        {
            await _report.Create(req);
        }
    }
}