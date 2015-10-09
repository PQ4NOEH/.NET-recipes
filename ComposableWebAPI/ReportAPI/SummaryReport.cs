using Report.Domain;
using System.Text;
using System.Threading.Tasks;

namespace ReportAPI
{
    public class SummaryReport : ISummaryReport
    {
        readonly IReportRepository<SummaryReport> _repository;
        public SummaryReport(IReportRepository<SummaryReport> repository)
        {
            _repository = repository;
        }
        public async Task Create(IReportIdentity reportIdentity)
        {
            await _repository.Save(reportIdentity, Encoding.UTF8.GetBytes(reportIdentity.ToString()));
        }

        public async Task<byte[]> Get(IReportIdentity reportIdentity)
        {
            return await _repository.Get(reportIdentity);
        }
    }
}
