using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Davalor.MomProxy.Host.UnitTests
{
    public class AvalaibleLocalPortSpec
    {
        [Fact]
        public void If_the_port_is_under_1025_throws_an_exception()
        {
            var portNumber = new Random().Next(1, 1024);
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new AvalaibleLocalPort(portNumber));
            Assert.Contains("port number has to between 1025 and 65535", ex.Message);
        }

        [Fact]
        public void If_the_port_is_over_65535_throws_an_exception()
        {
            var portNumber = new Random().Next(65535, 99999999);
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new AvalaibleLocalPort(portNumber));
            Assert.Contains("port number has to between 1025 and 65535", ex.Message);
        }

        [Fact]
        public void If_the_port_is_not_avalaible_throws_an_exception()
        {
            int portNumber = IPGlobalProperties
                                .GetIPGlobalProperties()
                                .GetActiveTcpConnections()
                                .Where(c => c.LocalEndPoint.Port > 1024 && c.LocalEndPoint.Port < 65535)
                                .Select(c => c.LocalEndPoint.Port)
                                .First();
           
            var ex = Assert.Throws<WebException>(() => new AvalaibleLocalPort(portNumber));
            Assert.Contains(portNumber.ToString(), ex.Message);
            Assert.Contains("in use", ex.Message);
        }
        [Fact]
        public void If_the_port_is_avalaible_and_is_between_1025_and_65534_creates_a_new_instance()
        {
            var busyPortNumbers = IPGlobalProperties
                                .GetIPGlobalProperties()
                                .GetActiveTcpConnections()
                                .Where(c => c.LocalEndPoint.Port > 1024 && c.LocalEndPoint.Port < 65535)
                                .Select(c => c.LocalEndPoint.Port)
                                .ToList<int>();
            int portNumber;
            do
            {
                portNumber = new Random().Next(1025, 65534);
            } while (busyPortNumbers.Contains(portNumber));

            Assert.NotNull(new AvalaibleLocalPort(portNumber));
        }
        [Fact]
        public void Value_returns_the_port_value()
        {
            var busyPortNumbers = IPGlobalProperties
                                .GetIPGlobalProperties()
                                .GetActiveTcpConnections()
                                .Where(c => c.LocalEndPoint.Port > 1024 && c.LocalEndPoint.Port < 65535)
                                .Select(c => c.LocalEndPoint.Port)
                                .ToList<int>();
            int portNumber;
            do
            {
                portNumber = new Random().Next(1025, 65534);
            } while (busyPortNumbers.Contains(portNumber));

            Assert.Equal(new AvalaibleLocalPort(portNumber).Value, portNumber);
        }
        [Fact]
        public void It_implicitly_cast_to_int()
        {
            var busyPortNumbers = IPGlobalProperties
                                .GetIPGlobalProperties()
                                .GetActiveTcpConnections()
                                .Where(c => c.LocalEndPoint.Port > 1024 && c.LocalEndPoint.Port < 65535)
                                .Select(c => c.LocalEndPoint.Port)
                                .ToList<int>();
            int portNumber;
            do
            {
                portNumber = new Random().Next(1025, 65534);
            } while (busyPortNumbers.Contains(portNumber));
            var expected = new AvalaibleLocalPort(portNumber);
            var testAssert = new Action<AvalaibleLocalPort>(
                casted =>{
                    Assert.Equal(casted.Value, expected.Value);
                });
            testAssert(portNumber);
        }
        [Fact]
        public void It_implicitly_cast_to_Uri()
        {
            var busyPortNumbers = IPGlobalProperties
                                .GetIPGlobalProperties()
                                .GetActiveTcpConnections()
                                .Where(c => c.LocalEndPoint.Port > 1024 && c.LocalEndPoint.Port < 65535)
                                .Select(c => c.LocalEndPoint.Port)
                                .ToList<int>();
            int portNumber;
            do
            {
                portNumber = new Random().Next(1025, 65534);
            } while (busyPortNumbers.Contains(portNumber));
            var testAssert = new Action<Uri>(
                casted =>
                {
                    var expectedUri = new Uri(string.Format("Http://{0}:{1}", Environment.MachineName, portNumber));
                    Assert.Equal(casted, expectedUri);
                });
            testAssert(new AvalaibleLocalPort(portNumber));
        }
    }
}
