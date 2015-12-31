namespace Heracles.Services
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.Threading;

    using Altea.Contracts;

    using Microsoft.Azure;
    using Microsoft.ServiceBus;

    public abstract class Service
    {
        protected static readonly int ConnectionRetries = -1;

        static Service()
        {
            if (Service.ConnectionRetries == -1)
            {
                Service.ConnectionRetries = Convert.ToInt32(
                    ConfigurationManager.AppSettings["ChannelConnectionRetries"]);
            }
        } 
    }

    public abstract class Service<TChannel> : Service, IDisposable where TChannel : IClientChannel
    {
        /// <summary>
        /// ThreadLocal Channels, so that each thread can have its own Channel.
        /// </summary>
        protected static readonly ThreadLocal<TChannel> Channel;

        /// <summary>
        /// Shared ChannelFactory for creating Channels.
        /// </summary>
        private static readonly ChannelFactory<TChannel> ChannelFactory;

        static Service()
        {
            Uri address = ServiceBusEnvironment.CreateServiceUri(
                "sb",
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ServiceNamespace"),
                GetContractData(typeof(TChannel)).RelayName);

            ChannelFactory = new ChannelFactory<TChannel>(
                new NetTcpRelayBinding(),
                address.ToString());

            ChannelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider( 
                        CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SharedAccessKeyName"),
                        CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SharedAccessKey"))
                });

            Channel = new ThreadLocal<TChannel>();
        }
        
        public void Dispose()
        {
            foreach (TChannel channel in Channel.Values)
            {
                if (channel.State != CommunicationState.Closed)
                {
                    channel.Abort();
                }

                channel.Dispose();
            }

            Channel.Dispose();
        }

        private static ContractDataAttribute GetContractData(Type contract)
        {
            return Attribute.GetCustomAttribute(
                contract,
                typeof(ContractDataAttribute)) as ContractDataAttribute;
        }

        /// <summary>
        /// Gets and caches a new channel.
        /// </summary>
        /// <returns>the created channel</returns>
        private static TChannel CreateChannel()
        {
            TChannel channel = ChannelFactory.CreateChannel();
            channel.Open();

            return channel;
        }

        /// <summary>
        /// Gets the channel instance for the current thread.
        /// </summary>
        /// <returns>the channel instance</returns>
        private static TChannel GetChannel()
        {
            // If this thread doesn't have a channel created, create one.
            if (!Channel.IsValueCreated || Channel.Value == null)
            {
                Channel.Value = CreateChannel();
            }
            else
            {
                switch (Channel.Value.State)
                {
                    // If channel is in faulted state, close and recreate it.
                    case CommunicationState.Faulted:
                        CloseChannel();
                        Channel.Value = CreateChannel();
                        break;

                    // If channel is closed, recreate it.
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                        Channel.Value = CreateChannel();
                        break;
                }
            }

            return Channel.Value;
        }

        /// <summary>
        /// Closes the channel in a proper way.
        /// </summary>
        private static void CloseChannel()
        {
            // Always check if we still have a valid channel.
            if (Channel.IsValueCreated)
            {
                // If the channel it's not fully closed, close it.
                if (Channel.Value.State != CommunicationState.Closed)
                {
                    Channel.Value.Abort();
                }

                Channel.Value = default(TChannel);
            }
        }

        protected static void Execute(string operation)
        {
            ExecuteInternal(operation, null);
        }

        protected static void Execute(string operation, params object[] parameters)
        {
            ExecuteInternal(operation, parameters);
        }

        protected static T Execute<T>(string operation)
        {
            return (T)ExecuteInternal(operation, null);
        }

        protected static T Execute<T>(string operation, params object[] parameters)
        {
            return (T)ExecuteInternal(operation, parameters);
        }

        private static object ExecuteInternal(string operation, params object[] parameters)
        {
            MethodInfo method = typeof(TChannel)
                .GetInterfaces()
                .Select(x => x.GetMethod(operation))
                .FirstOrDefault(x => x != null);

            if (method == null)
            {
                throw new ArgumentException(@"Contract does not have this operation.", "operation");
            }

            for (int attempt = 1;;)
            {
                try
                {
                    TChannel channel = GetChannel();
                    return method.Invoke(channel, parameters);
                }
                catch (CommunicationException)
                {
                    if (++attempt == Service.ConnectionRetries)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
