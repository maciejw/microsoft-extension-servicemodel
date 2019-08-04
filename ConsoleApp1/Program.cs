using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ServiceModel.Channels;

namespace ConsoleApp1
{
    static class Program
    {
        public static async Task Main(string[] args)
        {
            TestTrace();

            if (args.IsClient())
            {
                Console.Title = "Client";

                await CallServiceNetTcp();
                await CallServiceHttp();

            }
            else
            {
                Console.Title = "Server";

                IHostBuilder builder = CreateBuilder();

                if (args.IsService())
                {
                    await builder.RunAsServiceAsync();
                }
                else
                {
                    await builder.RunConsoleAsync();
                }
            }
        }

        private static bool IsService(this string[] args)
        {
            return !(Debugger.IsAttached || args.Contains("--console"));
        }

        private static bool IsClient(this string[] args)
        {
            return args.Contains("--client");
        }

        private static IHostBuilder CreateBuilder()
        {
            return new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddWcfService<IService1, Service1>();
                });
        }

        private static void TestTrace()
        {
            var s = new TraceSource("app");

            s.TraceInformation("trace source");
        }

        interface IService1Client : IClientChannel, IService1 { }
        static async Task CallServiceNetTcp()
        {
            await CallService("net.tcp://localhost:5001/service1", new NetTcpBinding());
        }
        static async Task CallServiceHttp()
        {
            await CallService("http://localhost:5000/service1", new BasicHttpBinding());
        }

        private static async Task CallService(string uri, Binding binding)
        {
            using (var channelFactory = new ChannelFactory<IService1Client>(binding, new EndpointAddress(uri)))
            {
                using (var channel = channelFactory.CreateChannel())
                {
                    Console.WriteLine($"calculation: {await channel.Calculate(1, 2)}");
                    Console.WriteLine($"calculation: {await channel.Calculate(1, 2)}");
                }

            }
        }
    }
}
