using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public class HostedWcfService<TContract, TService> : IHostedService
    {
        private readonly ServiceHost host;

        public HostedWcfService(IServiceProvider serviceProvider)
        {
            if (ServiceDescription.GetService(typeof(TService)).IsInstanceContextModeSingle())
            {
                host = new ServiceHost(serviceProvider.GetRequiredService<TContract>());
            }
            else
            {
                host = new ServiceHost(typeof(TService));
                host.AddDependencyInjectionBehavior<TContract>(serviceProvider);
            }
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {

            await host.OpenAsync();
        }

        async Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            await host.CloseAsync();

            using (host) { }
        }
    }
}
