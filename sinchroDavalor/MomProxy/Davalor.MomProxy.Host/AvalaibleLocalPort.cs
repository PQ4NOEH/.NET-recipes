using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Davalor.MomProxy.Host
{
    public sealed class AvalaibleLocalPort
    {
        readonly int _port;
        public AvalaibleLocalPort(int port)
        {
            if (port < 1025 || port > 65535) throw new ArgumentOutOfRangeException("port number has to between 1025 and 65535");
            if (PortIsBusy(port)) throw new WebException(string.Format("The port {0} is already in use", port));
            _port = port;
        }

        bool PortIsBusy(int port)
        {
            return IPGlobalProperties
                    .GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Any(c => c.LocalEndPoint.Port == port);
        }
        public int Value
        {
            get
            {
                return _port;
            }
        }

        public static implicit operator Uri(AvalaibleLocalPort port)
        {
            return new Uri(string.Format("Http://{0}:{1}", Environment.MachineName, port.Value));
        }

        public static implicit operator AvalaibleLocalPort(int port)
        {
            return new AvalaibleLocalPort(port);
        }
    }
}
