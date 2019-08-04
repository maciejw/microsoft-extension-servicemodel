using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Description
{
    public class ServiceProviderServiceBehavior : IServiceBehavior
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Type serviceContract;

        public ServiceProviderServiceBehavior(IServiceProvider serviceProvider, Type serviceContract)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.serviceContract = serviceContract ?? throw new ArgumentNullException(nameof(serviceContract));
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serviceDescription" /> or
        /// <paramref name="serviceHostBase" /> is <see langword="null" />.
        /// </exception>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceDescription == null)
            {
                throw new ArgumentNullException(nameof(serviceDescription));
            }
            if (serviceHostBase == null)
            {
                throw new ArgumentNullException(nameof(serviceHostBase));
            }
            var instanceProvider = new ServiceProviderInstanceProvider(serviceProvider, new ServiceFactory(serviceContract).CreateInstance);

            var serviceEndpointContractNames = serviceDescription.GetContractNames(serviceContract).ToArray();

            foreach (var endpointDispatcher in serviceHostBase.GetEndpointDispatchers(serviceEndpointContractNames))
            {
                endpointDispatcher.DispatchRuntime.InstanceProvider = instanceProvider;
            }
        }

       
    }
}
