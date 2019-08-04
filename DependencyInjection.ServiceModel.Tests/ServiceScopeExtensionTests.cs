using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DependencyInjection.ServiceModel.Tests
{

    public class ServiceScopeExtensionTests
    {
        IServiceCollection services = new ServiceCollection();
        class TestService { }

        [Fact(DisplayName = "Should create new scope for services")]
        public void Fact1()
        {
            services.AddScoped<TestService>();

            using (var provider = services.BuildServiceProvider())
            {
                var rootInstance = provider.GetService<TestService>();

                using (var sut = new ServiceScopeExtension(provider))
                {
                    Assert.NotSame(provider, sut.ScopedServiceProvider);

                    var scopedService1 = sut.ScopedServiceProvider.GetService<TestService>();
                    var scopedService2 = sut.ScopedServiceProvider.GetService<TestService>();

                    Assert.NotSame(rootInstance, scopedService1);
                    Assert.Same(scopedService1, scopedService1);
                }
            }
        }
    }
}
