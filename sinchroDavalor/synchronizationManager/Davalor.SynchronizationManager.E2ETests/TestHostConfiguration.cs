using Davalor.SynchronizationManager.Host;
using System.Configuration;

namespace Davalor.SynchronizationManager.E2ETests
{
    public class TestHostConfiguration : HostConfiguration
    {
        public TestHostConfiguration():base()
        {
            TestServer = ConfigurationManager.ConnectionStrings["VisionLocalCreate"].ConnectionString;

        }

        public string TestServer
        {
            get;
            private set;
        }
    }
}
