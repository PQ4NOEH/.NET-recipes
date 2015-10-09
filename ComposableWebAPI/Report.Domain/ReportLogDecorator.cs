using System.Diagnostics;
using System.Threading.Tasks;

namespace Report.Domain
{
    public class ReportLogDecorator : IReport
    {
        readonly IReport _decoratedReport;
        public ReportLogDecorator(IReport decoratedReport)
        {
            _decoratedReport = decoratedReport;
        }
        public async Task Create(IReportIdentity reportIdentity)
        {
            Debug.WriteLine("Creating the report... ");
            await _decoratedReport.Create(reportIdentity);
            Debug.WriteLine("Report created.");
        }

        public async Task<byte[]> Get(IReportIdentity reportIdentity)
        {
            Debug.WriteLine("Getting the report... ");
            var result = await _decoratedReport.Get(reportIdentity);
            Debug.WriteLine("Report Got.");
            return result;
        }
    }

    public class SummaryLogDecorator : ReportLogDecorator, ISummaryReport
    {
        public SummaryLogDecorator(ISummaryReport decoratedReport) : base(decoratedReport) { }
    }
}
