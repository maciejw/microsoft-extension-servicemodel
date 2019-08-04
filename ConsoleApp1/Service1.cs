using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1
{
    public class Service1 : IService1
    {
        private Service1() { }

        int counter = 1;
        private readonly ILogger<Service1> logger;

        public Service1(ILogger<Service1> logger)
        {
            this.logger = logger;
        }
        public async Task<int> Calculate(int a, int b)
        {
            await Task.Delay(0);

            logger.LogInformation("params a: {a} b: {b} counter: {counter}", a, b, counter);

            return (a + b) * counter++;
        }

        public void Dispose()
        {
            
        }
    }
}
