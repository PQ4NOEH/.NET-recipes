using Report.Domain;
using System.IO;
using System.Threading.Tasks;

namespace Report.Repository
{
    public class ReportRepository<T> : IReportRepository<T> where T : IReport
    {
        string FilePath(IReportIdentity identity)
        {
            return string.Format("C:\\Temp\\ComposableUI\\{0}", identity);
        }

        public Task Save(IReportIdentity identity, byte[] report)
        {
            return Task.Run(() => File.WriteAllBytes(FilePath(identity), report));
        }

        public Task<byte[]> Get(IReportIdentity identity)
        {
            return Task.Run<byte[]>(() => File.ReadAllBytes(FilePath(identity)));
        }
    }
}
