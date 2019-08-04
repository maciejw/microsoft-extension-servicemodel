using System.Threading.Tasks;

namespace System.ServiceModel
{
    public static class ServiceHostExtensions
    {
        public static Task OpenAsync(this ServiceHostBase @this)
        {
            return Task.Factory.FromAsync(@this.BeginOpen, @this.EndOpen, new object());
        }

        public static Task OpenAsync(this ServiceHostBase @this, TimeSpan timeout)
        {
            return Task.Factory.FromAsync(@this.BeginOpen, @this.EndOpen, timeout, new object());
        }

        public static Task CloseAsync(this ServiceHostBase @this)
        {
            return Task.Factory.FromAsync(@this.BeginClose, @this.EndClose, new object());
        }

        public static Task CloseAsync(this ServiceHostBase @this, TimeSpan timeout)
        {
            return Task.Factory.FromAsync(@this.BeginClose, @this.EndClose, timeout, new object());
        }
    }
}
