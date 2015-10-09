using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain
{
    public interface IReport
    {
        Task Create(IReportIdentity reportIdentity);
        Task<byte[]> Get(IReportIdentity reportIdentity);
    }

    public interface ISummaryReport : IReport { }
    public interface IFullReport : IReport { }
    public interface IMonoChromeSummaryReport : IReport { }
}
