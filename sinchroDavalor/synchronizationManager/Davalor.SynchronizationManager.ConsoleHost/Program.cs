using Davalor.SynchronizationManager.Host;

namespace Davalor.SynchronizationManager.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = new BootStrapper();
            bootstrapper.StartServices();
            bootstrapper.StartKafkaListener();
            bootstrapper.StartLogger();
            while (true);
        }
    }
}
