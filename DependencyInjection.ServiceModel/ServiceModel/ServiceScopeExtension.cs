using Microsoft.Extensions.DependencyInjection;

namespace System.ServiceModel
{
    /// <summary>
    /// Holds reference to <see cref="IServiceScope" /> associated with <see cref="InstanceContextMode.PerCall"/> or <see cref="InstanceContextMode.PerSession"/> 
    /// </summary>
    public class ServiceScopeExtension : IExtension<InstanceContext>, IDisposable
    {
        private bool _disposed;
        private readonly IServiceScope serviceScope;

        /// <summary>
        /// Gets the current <see cref="ServiceScopeExtension"/>
        /// for the operation.
        /// </summary>
        /// <value>
        /// The <see cref="ServiceScopeExtension"/> associated
        /// with the current <see cref="OperationContext"/> if
        /// one exists; or <see langword="null" /> if there isn't one.
        /// </value>
        /// <remarks>
        /// <para>
        /// In a singleton service, there won't be a current <see cref="ServiceScopeExtension"/>
        /// because singleton services are resolved at the time the service host begins
        /// rather than on each operation.
        /// </para>
        /// </remarks>
        public static ServiceScopeExtension Current
        {
            get
            {
                return OperationContext.Current?.InstanceContext?.Extensions.Find<ServiceScopeExtension>();
            }
        }

        /// <summary>
        /// Gets the request/operation lifetime.
        /// </summary>
        /// <value>
        /// An <see cref="IServiceProvider"/> that this instance
        /// context will use to resolve service instances.
        /// </value>
        public IServiceProvider ScopedServiceProvider { get { return serviceScope.ServiceProvider; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceScopeExtension"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The outer container/lifetime scope from which the instance scope
        /// will be created.
        /// </param>
        public ServiceScopeExtension(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            serviceScope = serviceProvider.CreateScope();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ServiceScopeExtension"/> class.
        /// </summary>
        ~ServiceScopeExtension()
        {
            Dispose(false);
        }

        /// <summary>
        /// Enables an extension object to find out when it has been aggregated.
        /// Called when the extension is added to the
        /// <see cref="IExtensibleObject{T}.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Attach(InstanceContext owner)
        {
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated.
        /// Called when an extension is removed from the
        /// <see cref="IExtensibleObject{T}.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(InstanceContext owner)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Handles disposal of managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to dispose of managed resources (during a manual execution
        /// of <see cref="Dispose()"/>); or
        /// <see langword="false" /> if this is getting run as part of finalization where
        /// managed resources may have already been cleaned up.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    serviceScope.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
