using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [ServiceContract]
    public interface IService1: IDisposable
    {
        [OperationContract]
        Task<int> Calculate(int a, int b);
    }
}
