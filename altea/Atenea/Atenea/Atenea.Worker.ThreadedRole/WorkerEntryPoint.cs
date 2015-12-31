namespace Atenea.Worker.ThreadedRole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.WindowsAzure.ServiceRuntime;

    public abstract class WorkerEntryPoint
    {
        private static readonly IEnumerable<string> RoleInstance = new string[] { RoleEnvironment.CurrentRoleInstance.Id };

        public virtual bool OnStart()
        {
            return true;
        }

        internal void ProtectedRun()
        {
            try
            {
                Run();
            }
            catch (SystemException)
            {
                throw;
            }
            catch (Exception)
            {
            }
        }

        public virtual void Run()
        {
        }

        public virtual void OnStop()
        {
        }

        public static void TraceInformation(string message)
        {
            Trace.TraceInformation(
                "{0}: " + message,
                RoleEnvironment.CurrentRoleInstance.Id);
        }

        public static void TraceInformation(string format, params object[] args)
        {
            Trace.TraceInformation(
                "{0}: " + format,
                RoleInstance.Union(args).ToArray());
        }

        public static void TraceWarning(string message)
        {
            Trace.TraceWarning(
                "{0}: " + message,
                RoleEnvironment.CurrentRoleInstance.Id);
        }

        public static void TraceWarning(string format, params object[] args)
        {
            Trace.TraceWarning(
                "{0}: " + format,
                RoleInstance.Union(args).ToArray());
        }

        public static void TraceError(string message)
        {
            Trace.TraceError(
                "{0}: " + message,
                RoleEnvironment.CurrentRoleInstance.Id);
        }

        public static void TraceError(string format, params object[] args)
        {
            Trace.TraceError(
                "{0}: " + format,
                RoleInstance.Union(args).ToArray());
        }
    }
}
