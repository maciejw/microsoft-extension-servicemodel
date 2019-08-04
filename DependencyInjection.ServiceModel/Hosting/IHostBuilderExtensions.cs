using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IHostLifetime, ServiceBaseLifetime>();
            });
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }
    }
}
