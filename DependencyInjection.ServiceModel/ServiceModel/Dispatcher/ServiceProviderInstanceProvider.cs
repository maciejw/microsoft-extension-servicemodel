using System.ServiceModel.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace System.ServiceModel.Dispatcher
{
    public class ServiceProviderInstanceProvider : IInstanceProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Func<IServiceProvider, object> serviceFactory;

        public ServiceProviderInstanceProvider(IServiceProvider serviceProvider, Func<IServiceProvider, object> serviceFactory)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="InstanceContext"/> object.</param>
        /// <returns>A user-defined service object.</returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="InstanceContext"/> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>The service object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="instanceContext" /> is <see langword="null" />.
        /// </exception>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException(nameof(instanceContext));
            }

            var serviceScopeExtensions = new ServiceScopeExtension(serviceProvider);

            instanceContext.Extensions.Add(serviceScopeExtensions);

            try
            {
                return serviceFactory(serviceScopeExtensions.ScopedServiceProvider);
            }
            catch (Exception)
            {
                serviceScopeExtensions.Dispose();
                instanceContext.Extensions.Remove(serviceScopeExtensions);
                throw;
            }
        }

        /// <summary>
        /// Called when an <see cref="InstanceContext"/> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="instanceContext" /> is <see langword="null" />.
        /// </exception>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instanceContext is null)
            {
                throw new ArgumentNullException(nameof(instanceContext));
            }

            instanceContext.Extensions.Find<ServiceScopeExtension>()?.Dispose();
        }
    }
}
