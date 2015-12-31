namespace Atenea.Worker
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Reflection;

    using Altea.Database;

    using Atenea.Worker.ThreadedRole;
    using Atenea.Worker.WorkerThreads;

    using Microsoft.WindowsAzure.ServiceRuntime;

#if !DEBUG
    using Mindscape.Raygun4Net;
#endif

    public class WorkerRole : ThreadedRoleEntryPoint
    {
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections.
            // 12 * [Number of logical CPUs]
            ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            Trace.TraceInformation(
                "{0}: role is starting ({1})",
                RoleEnvironment.CurrentRoleInstance.Id,
                DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            DatabaseSettings.ConnectionStringsWarehouse = ConnectionStringsOrigin.ConfigurationManager;

            Type workersType = typeof(IWorkerThread);
            Assembly workersAssembly = workersType.Assembly;

            bool result = base.OnStart(
                workersAssembly
                    .GetTypes()
                    .Where(t => workersType.IsAssignableFrom(t) && workersType != t)
                    .Select(t => Activator.CreateInstance(t) as WorkerEntryPoint)
                    .ToArray());

            Trace.TraceInformation("{0}: role has been started", RoleEnvironment.CurrentRoleInstance.Id);

            return result;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if DEBUG
            Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
            Debug.WriteLine(((Exception)e.ExceptionObject).Message);
#else
            RaygunClient client = new RaygunClient("X8g432kXiu6yV46IZHAsIw==");
            client.Send(e.ExceptionObject as Exception);
#endif
        }

        public override void OnStop()
        {
            Trace.TraceInformation("{0}: role is stopping", RoleEnvironment.CurrentRoleInstance.Id);

            base.OnStop();

            Trace.TraceInformation("{0}: role has stopped", RoleEnvironment.CurrentRoleInstance.Id);
        }

        internal static void RaygunTraceException(Exception ex)
        {
#if DEBUG
            Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
            Debug.WriteLine(ex.Message);
#else
            RaygunClient client = new RaygunClient("X8g432kXiu6yV46IZHAsIw==");
            client.Send(ex);
#endif
        }
    }
}
