using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{

    [DesignerCategory(Disable)]
    public class ServiceBaseLifetime : ServiceBase, IHostLifetime
    {
        private const string Disable = "";

        private readonly TaskCompletionSource<object> delayStart = new TaskCompletionSource<object>();
        private readonly IApplicationLifetime applicationLifetime;

        public ServiceBaseLifetime(IApplicationLifetime applicationLifetime)
        {
            this.applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        Task IHostLifetime.WaitForStartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => delayStart.TrySetCanceled());
            applicationLifetime.ApplicationStopping.Register(Stop);

            new Thread(Run).Start(); // Otherwise this would block and prevent IHost.StartAsync from finishing.
            return delayStart.Task;
        }

        private void Run()
        {
            try
            {
                Run(this); // This blocks until the service is stopped.
                delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));
            }
            catch (Exception ex)
            {
                delayStart.TrySetException(ex);
            }
        }

        Task IHostLifetime.StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }

        // Called by base.Run when the service is ready to start.
        protected override void OnStart(string[] args)
        {
            delayStart.TrySetResult(null);
            base.OnStart(args);
        }

        // Called by base.Stop. This may be called multiple times by service Stop, ApplicationStopping, and StopAsync.
        // That's OK because StopApplication uses a CancellationTokenSource and prevents any recursion.
        protected override void OnStop()
        {
            applicationLifetime.StopApplication();
            base.OnStop();
        }
    }
}
