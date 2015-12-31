namespace Heracles.Web
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;

    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            // Set the maximum number of concurrent connections.
            // 12 * [Number of logical CPUs]
            ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;

            Trace.TraceInformation(
                "{0}: Heracles.Web role is starting ({1})",
                RoleEnvironment.CurrentRoleInstance.Id,
                DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("{0}: Heracles.Web role is stopping", RoleEnvironment.CurrentRoleInstance.Id);

            base.OnStop();

            Trace.TraceInformation("{0}: Heracles.Web role has stopped", RoleEnvironment.CurrentRoleInstance.Id);
        }
    }
}
