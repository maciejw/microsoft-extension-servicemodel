using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DependencyInjection.ServiceModel.Tests
{
    public class ServiceProviderInstanceProviderTests
    {
        NetTcpBinding binding = new NetTcpBinding();

        class Dependency
        {
            public int Initial { get; set; }
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        class TestServiceSingle : ITestService
        {
            private TestServiceSingle() { }
            int counter;
            public TestServiceSingle(Dependency dependency)
            {
                counter = dependency.Initial;
            }
            public int Operation()
            {
                return ++counter;
            }
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
        class TestServicePerSession: ITestService
        {
            private TestServicePerSession() { }
            int counter;
            public TestServicePerSession(Dependency dependency)
            {
                counter = dependency.Initial;
            }
            public int Operation()
            {
                return ++counter;
            }
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
        class TestServicePerCall : ITestService
        {
            private TestServicePerCall() { }
            int counter;
            public TestServicePerCall(Dependency dependency)
            {
                counter = dependency.Initial;
            }

            public int Operation()
            {
                return ++counter;
            }
        }

        [ServiceContract]
        private interface ITestService
        {
            [OperationContract]
            int Operation();
        }

        interface ITestServiceClient : ITestService, IClientChannel { }

        [Fact(DisplayName = "Should use instance provider to create per call service")]
        [Trait("type", "integration")]
        public async Task Fact1()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ITestService, TestServicePerCall>();
            services.AddSingleton(new Dependency { Initial = 1 });

            using (var provider = services.BuildServiceProvider())
            {
                using (ServiceHost host = new ServiceHost(typeof(TestServicePerCall), new Uri("net.tcp://localhost:5000")))
                {
                    host.AddServiceEndpoint(typeof(ITestService), binding, "/service1");
                    host.AddDependencyInjectionBehavior<ITestService>(provider);

                    await host.OpenAsync();

                    using (var client = ChannelFactory<ITestServiceClient>.CreateChannel(binding, new EndpointAddress("net.tcp://localhost:5000/service1")))
                    {
                        var result = client.Operation();
                        Assert.Equal(2, result);
                        result = client.Operation();
                        Assert.Equal(2, result);
                    }

                    await host.CloseAsync();
                }

            }
        }
        [Fact(DisplayName = "Should use instance provider to create per session service")]
        [Trait("type", "integration")]
        public async Task Fact5()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ITestService, TestServicePerSession>();
            services.AddSingleton(new Dependency { Initial = 1 });

            using (var provider = services.BuildServiceProvider())
            {
                using (ServiceHost host = new ServiceHost(typeof(TestServicePerSession), new Uri("net.tcp://localhost:5001")))
                {
                    host.AddServiceEndpoint(typeof(ITestService), binding, "/service1");
                    host.AddDependencyInjectionBehavior<ITestService>(provider);

                    await host.OpenAsync();

                    EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:5001/service1");

                    using (var client = ChannelFactory<ITestServiceClient>.CreateChannel(binding, endpointAddress))
                    {
                        var result = client.Operation();
                        Assert.Equal(2, result);
                        result = client.Operation();
                        Assert.Equal(3, result);
                    }
                    using (var client = ChannelFactory<ITestServiceClient>.CreateChannel(binding, endpointAddress))
                    {
                        var result = client.Operation();
                        Assert.Equal(2, result);
                        result = client.Operation();
                        Assert.Equal(3, result);
                    }

                    await host.CloseAsync();
                }

            }
        }
        [Fact(DisplayName = "Should not use instance provider to single create service")]
        [Trait("type", "integration")]
        public async Task Fact4()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ITestService, TestServiceSingle>();
            services.AddSingleton(new Dependency { Initial = 2 });

            using (var provider = services.BuildServiceProvider())
            {
                using (ServiceHost host = new ServiceHost(provider.GetRequiredService<ITestService>(), new Uri("net.tcp://localhost:5002")))
                {
                    host.AddServiceEndpoint(typeof(ITestService), binding, "/service1");
                    host.AddDependencyInjectionBehavior<ITestService>(provider);

                    await host.OpenAsync();

                    EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:5002/service1");

                    using (var client = ChannelFactory<ITestServiceClient>.CreateChannel(binding, endpointAddress))
                    {
                        var result = client.Operation();
                        Assert.Equal(3, result);
                        result = client.Operation();
                        Assert.Equal(4, result);
                    }
                    using (var client = ChannelFactory<ITestServiceClient>.CreateChannel(binding, endpointAddress))
                    {
                        var result = client.Operation();
                        Assert.Equal(5, result);
                        result = client.Operation();
                        Assert.Equal(6, result);
                    }

                    await host.CloseAsync();
                }
            }
        }


        [Fact(DisplayName = "Should not register instance provider in single instance service")]
        public void Fact3()
        {
            using (ServiceHost host = new ServiceHost(typeof(TestServiceSingle), new Uri("net.tcp://localhost:5002")))
            {
                using (var provider = new ServiceCollection().BuildServiceProvider())
                {
                    host.AddDependencyInjectionBehavior<ITestService>(provider);

                    Assert.False(host.Description.Behaviors.Contains(typeof(ServiceProviderServiceBehavior)));
                }
            }
        }
    }
}
