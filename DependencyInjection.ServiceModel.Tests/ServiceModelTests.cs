using System;
using System.ServiceModel.Description;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DependencyInjection.ServiceModel.Tests
{
    public class ServiceFactoryTests
    {
        static Type serviceType = typeof(TestService);

        ServiceFactory sut = new ServiceFactory(serviceType);

        IServiceCollection services = new ServiceCollection();

        class TestService { }

        [Fact(DisplayName = "Should create an instance of registered type")]
        public void Fact2()
        {
            services.AddScoped(serviceType);

            using (ServiceProvider scopedServiceProvider = services.BuildServiceProvider())
            {
                var instance = sut.CreateInstance(scopedServiceProvider);

                Assert.Equal(serviceType, instance.GetType());
            }

        }
        [Fact(DisplayName = "Should throw if object is not registered")]
        public void Fact1()
        {
            using (ServiceProvider scopedServiceProvider = services.BuildServiceProvider())
            {
                var ex = Assert.Throws<InvalidOperationException>(() => sut.CreateInstance(scopedServiceProvider));

                Assert.Matches(@"No service for type '.+\+TestService' has been registered.", ex.Message);
            }
        }
    }
}
