using System;
using System.Net;

namespace TheMenu.Core
{
    public interface IHostConfiguration
    {
        IPAddress EventStoreIp { get; }
        int EventStorePort { get; }
    }
}
