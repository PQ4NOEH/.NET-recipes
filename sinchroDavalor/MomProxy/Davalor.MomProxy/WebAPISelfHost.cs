using Davalor.MomProxy.Host;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Davalor.MomProxy.ConsoleHost
{
    public class WebAPISelfHost : IDisposable
    {
        readonly HttpSelfHostConfiguration _configuration;
        HttpSelfHostServer _server;

        public WebAPISelfHost(AvalaibleLocalPort hostPort)
        {
            _configuration = new HttpSelfHostConfiguration(hostPort);
            _configuration.Routes.MapHttpRoute("API Default", "API/{controller}/{id}", new { id = RouteParameter.Optional });
        }
        public void StartListenning()
        {
           if(_server == null)
           {
               _server = new HttpSelfHostServer(_configuration);
               _server.OpenAsync().Wait();
               Console.WriteLine("Listening on port 4325");
           }
        }

        public void StopListeninning()
        {
            if (_server != null)
            {
                _server.CloseAsync().Wait();
                _server.Dispose();
                _server = null;
            }
        }

        public void Dispose()
        {
            StopListeninning();
        }
    }
}
