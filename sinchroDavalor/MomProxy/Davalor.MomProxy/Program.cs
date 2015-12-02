using Davalor.MomProxy.Domain.Quota;
using Davalor.MomProxy.Host.Configuration;
using Davalor.MomProxy.Repository;
using Davalor.MomProxy.Services;
using Topshelf;

namespace Davalor.MomProxy.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<HostService>(s =>
                {
                    s.ConstructUsing(name => new HostService());
                    s.WhenStarted(tc => tc.Start());             
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenPaused(tc => tc.Pause());
                    s.WhenContinued(tc => tc.Continue());
                });
               
                x.RunAsLocalSystem();
                x.SetDescription("This service forwards messages to the Message oriented middleware");
                x.SetDisplayName("MomProxy");
                x.SetServiceName("MomProxy");
            });

            while (true) { }
        }
    }
}
