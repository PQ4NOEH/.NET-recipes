using System.Threading.Tasks;

namespace Report.Domain
{
    public interface IReportRepository<T> where T : IReport
    {
        Task Save(IReportIdentity identity, byte[] report);
        Task<byte[]> Get(IReportIdentity identity);
    }
}
