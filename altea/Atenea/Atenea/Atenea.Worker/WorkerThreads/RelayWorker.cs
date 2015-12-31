namespace Atenea.Worker.WorkerThreads
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using Altea.Contracts;
    using Altea.Services;

    using Atenea.Worker.ThreadedRole;

    using Microsoft.Azure;
    using Microsoft.ServiceBus;

    internal class RelayWorker : WorkerEntryPoint, IWorkerThread
    {
        private static readonly Type ContractInterface = typeof(IContract);

        private static readonly Type ChannelInterface = typeof(IClientChannel);

        private ServiceHost[] _hosts;

        public override bool OnStart()
        {
            WorkerEntryPoint.TraceInformation("relay worker is starting ({1})", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));

            // Get active services.
            Type[] services = GetActiveServices();

            // Create the credentials object for the endpoint.
            TransportClientEndpointBehavior sharedSecretServiceBusCredential =
                new TransportClientEndpointBehavior
                {
                    TokenProvider = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(
                        CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SharedAccessKeyName"),
                        CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SharedAccessKey"))
                        
                };

            // Create the ServiceRegistrySettings behavior for the endpoint.
            IEndpointBehavior serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Private);

            string serviceNamespace = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ServiceNamespace");

            // Start service hosts.
            int servicesLength = services.Length;
            this._hosts = new ServiceHost[servicesLength];

            WorkerEntryPoint.TraceInformation("relay worker has found {1} active services", servicesLength);

            for (int i = 0; i < servicesLength; i++)
            {
                Type service = services[i];
                Type contract = GetServiceContract(service);

                if (contract == null)
                {
                    WorkerEntryPoint.TraceWarning("relay worker error - service {1} has no contract", service.Name);
                    continue;
                }

                if (!CheckIfContractHasOperations(contract))
                {
                    WorkerEntryPoint.TraceWarning("relay worker error - contract for {1} has no operations", service.Name);
                    continue;
                }

                ContractDataAttribute serviceData = GetContractData(contract);

                if (serviceData == null)
                {
                    WorkerEntryPoint.TraceWarning("relay worker error - service {1} has no data attribute", service.Name);
                    continue;
                }

                // Create the service URI based on the service namespace.
                Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, serviceData.RelayName);

                // Create the service host reading the configuration.
                ServiceHost host = null;

                try
                {
                    host = new ServiceHost(service, address);

                    ServiceEndpoint endpoint = host.AddServiceEndpoint(
                        contract,
                        new NetTcpRelayBinding(),
                        address);

                    endpoint.Behaviors.Add(serviceRegistrySettings);
                    endpoint.Behaviors.Add(sharedSecretServiceBusCredential);

                    // Turn service debug behavior on
                    ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();

                    if (debug == null)
                    {
                        host.Description.Behaviors.Add(
                            new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
                    }
                    else
                    {
                        if (!debug.IncludeExceptionDetailInFaults)
                        {
                            debug.IncludeExceptionDetailInFaults = true;
                        }
                    }
                    
                    // Open the service
                    WorkerEntryPoint.TraceInformation("relay worker is starting host {1} for {2}", serviceData.RelayName, service.Name);

                    host.Open();
                    this._hosts[i] = host;
                }
                catch
                {
                    WorkerEntryPoint.TraceError("relay worker can't start host {1}", serviceData.RelayName);

                    if (host == null || host.State == CommunicationState.Faulted)
                    {
                        this._hosts[i] = null;
                        continue;
                    }

                    ((IDisposable)host).Dispose();
                    this._hosts[i] = null;
                }
            }

            bool result = base.OnStart();

            WorkerEntryPoint.TraceInformation("relay worker has been started");

            return result;
        }

        private static Type[] GetActiveServices()
        {
            Type serviceInterface = typeof(IService);
            Assembly servicesAssembly = serviceInterface.Assembly;
            return servicesAssembly
                .GetTypes()
                .Where(t => serviceInterface.IsAssignableFrom(t) && serviceInterface != t)
                .ToArray();
        }

        private static Type GetServiceContract(Type service)
        {
            return service
                .GetInterfaces()
                .SingleOrDefault(t => ContractInterface.IsAssignableFrom(t) && ContractInterface != t);
        }

        private static bool CheckIfContractHasOperations(Type contract)
        {
            bool hasOperations = false;
            Type operationAttribute = typeof(OperationContractAttribute);
            MethodInfo[] methods = contract.GetMethods();
            foreach (MethodInfo method in methods)
            {
                hasOperations = Attribute.GetCustomAttribute(method, operationAttribute) != null;
                if (hasOperations)
                {
                    break;
                }
            }

            return hasOperations;
        }

        private static ContractDataAttribute GetContractData(Type service)
        {
            Type channel =
                service.Assembly
                    .GetTypes()
                    .Single(
                        t => ChannelInterface.IsAssignableFrom(t) && service.IsAssignableFrom(t));

            return Attribute.GetCustomAttribute(
                channel,
                typeof(ContractDataAttribute)) as ContractDataAttribute;
        }

        public override void OnStop()
        {
            WorkerEntryPoint.TraceInformation("relay worker is stopping");

            // Close the service.
            foreach (ServiceHost host in _hosts.Where(host => host != null))
            {
                host.Close();
                ((IDisposable)host).Dispose();
            }

            base.OnStop();

            WorkerEntryPoint.TraceInformation("relay worker has stopped");
        }
    }
}
