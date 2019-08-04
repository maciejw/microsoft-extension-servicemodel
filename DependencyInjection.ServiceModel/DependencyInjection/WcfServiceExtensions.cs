using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WcfServiceExtensions
    {
        public static void AddWcfService<TContract, TService>(this IServiceCollection services) where TContract : class where TService : class, TContract
        {
            services.AddScoped<TContract, TService>();
            services.AddHostedService<HostedWcfService<TContract, TService>>();
        }
    }
}
