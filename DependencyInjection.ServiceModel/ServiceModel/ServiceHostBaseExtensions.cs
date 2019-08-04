using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel
{

    public static class ServiceHostBaseExtensions
    {
        /// <summary>
        /// Gets endpoint dispatches filtered by contract name.
        /// </summary>
        /// <param name="serviceHostBase"></param>
        /// <param name="serviceEndpointContractNames"></param>
        /// <returns></returns>
        public static IEnumerable<EndpointDispatcher> GetEndpointDispatchers(this ServiceHostBase serviceHostBase, string[] serviceEndpointContractNames)
        {
            return from channelDispacher in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>()
                   from endpointDispatcher in channelDispacher.Endpoints
                   where serviceEndpointContractNames.Contains(endpointDispatcher.ContractName)
                   select endpointDispatcher;
        }
        /// <summary>
        /// Adds the custom service behavior required for dependency injection.
        /// </summary>
        /// <typeparam name="TServiceContract">The web service contract type.</typeparam>
        /// <param name="serviceHost">The service host.</param>
        /// <param name="serviceProvider">The container.</param>
        public static void AddDependencyInjectionBehavior<TServiceContract>(this ServiceHostBase serviceHost, IServiceProvider serviceProvider)
        {
            AddDependencyInjectionBehavior(serviceHost, serviceProvider, typeof(TServiceContract));
        }

        
        /// <summary>
        /// Adds the custom service behavior required for dependency injection.
        /// </summary>
        /// <param name="serviceHost">The service host.</param>
        /// <param name="serviceProvider">The container.</param>
        /// <param name="serviceContract">The web service contract type.</param>
        public static void AddDependencyInjectionBehavior(this ServiceHostBase serviceHost, IServiceProvider serviceProvider, Type serviceContract)
        {
            if (serviceHost is null)
            {
                throw new ArgumentNullException(nameof(serviceHost));
            }

            if (serviceContract is null)
            {
                throw new ArgumentNullException(nameof(serviceContract));
            }

            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (!serviceHost.Description.IsInstanceContextModeSingle())
            {
                serviceHost.Description.Behaviors.Add(new ServiceProviderServiceBehavior(serviceProvider, serviceContract));
            }
        }


    }
}
