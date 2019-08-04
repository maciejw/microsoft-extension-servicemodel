using Microsoft.Extensions.DependencyInjection;

namespace System.ServiceModel.Description
{
    public class ServiceFactory
    {
        private Type serviceContract;

        public ServiceFactory(Type serviceContract)
        {
            this.serviceContract = serviceContract ?? throw new ArgumentNullException(nameof(serviceContract));
        }
        public object CreateInstance(IServiceProvider scopedServiceProvider)
        {
            if (scopedServiceProvider is null)
            {
                throw new ArgumentNullException(nameof(scopedServiceProvider));
            }

            return scopedServiceProvider.GetRequiredService(serviceContract);
        }
    }
}
