using Davalor.SynchronizationManager.E2ETests.Database;
using System;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    public class SetupTeardown : IDisposable
    {
        readonly TestHostConfiguration _configuration;
        public SetupTeardown()
        {
            _configuration = new TestHostConfiguration();
            var dbHelper = new VisionLocalDBHelper(_configuration.TestServer);
            dbHelper.CreateTables().Wait();
        }
        public void Dispose()
        {
            new VisionLocalDBHelper(_configuration.TestServer)
                    .DropTables()
                    .Wait();
        }
    }
    [CollectionDefinition("E2ETestsCollection")]
    public class E2ETestsCollectionDefinition : ICollectionFixture<SetupTeardown>
    {
    }
}
