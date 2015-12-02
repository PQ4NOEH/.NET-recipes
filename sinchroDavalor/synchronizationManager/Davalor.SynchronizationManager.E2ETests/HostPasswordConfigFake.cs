using Davalor.Base.Security.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.E2ETests
{
    public class HostPasswordConfigFake
    {
        static readonly HostPasswordConfiguration _config;
        static HostPasswordConfigFake()
        {
            _config = new HostPasswordConfiguration();
            _config.LoadFromFile("HostPasswords.json");
        }
        public static string GetHostPassword()
        {
            return _config["Tester"].ToString();
        }
    }
}
