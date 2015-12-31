namespace Atenea.Worker.WorkerThreads
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using Altea.Protocol;

    using Atenea.Worker.ThreadedRole;

    using Microsoft.WindowsAzure.ServiceRuntime;

    internal class SocketWorker : WorkerEntryPoint, IWorkerThread
    {
        private TcpListener listener;
        private AutoResetEvent connectionWaitHandle;

        public override bool OnStart()
        {
            WorkerEntryPoint.TraceInformation(
                "socket worker is starting ({1})",
                DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            IPEndPoint endPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["AlteaTcp"].IPEndpoint;
            this.listener = new TcpListener(endPoint)
            {
                ExclusiveAddressUse = false
            };
            this.listener.Start();

            this.connectionWaitHandle = new AutoResetEvent(false);

            bool result = base.OnStart();

            WorkerEntryPoint.TraceInformation("socket worker has been started");

            return result;
        }

        public override void Run()
        {
            while (true)
            {
                this.listener.BeginAcceptTcpClient(this.HandleAsyncConnection, this.listener);
                this.connectionWaitHandle.WaitOne();
            }
        }

        private void HandleAsyncConnection(IAsyncResult result)
        {
            TcpListener connectionListener = (TcpListener)result.AsyncState;

            using (TcpClient client = connectionListener.EndAcceptTcpClient(result))
            {
                this.connectionWaitHandle.Set();

                using (NetworkStream netStream = client.GetStream())
                {
                    byte[] segment = new byte[ProtoAltea.TcpSegmentLength];
                    netStream.Read(segment, 0, ProtoAltea.TcpSegmentLength);
                    ProtoAlteaMessage message = ProtoAltea.Unpack(segment);
                    // TODO
                }
            }
        }

        public override void OnStop()
        {
            WorkerEntryPoint.TraceInformation("socket worker is stopping");

            this.listener.Stop();

            base.OnStop();

            WorkerEntryPoint.TraceInformation("socket worker has stopped");
        }
    }
}
