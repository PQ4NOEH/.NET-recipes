using Davalor.SynchronizationManager.E2ETests.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.SynchronizationManager.E2ETests
{
    public class oracleHelperTest
    {
        //[Fact]
        public void ItWorks()
        {
            var portal = new PortalPacienteDBHelper("user id=system;password=12345678*;data source=localhost:1521/xe");
            //portal.DropDataBase().Wait();
            try
            {
                portal.CreateDataBase().Wait();
                new PortalPacienteDBHelper("user id=portalPacienteTest;password=123456789;data source=localhost:1521/xe").CreateTables().Wait();
            }
            finally
            {
                portal.DropDataBase().Wait();
            }
        }
    }
}
