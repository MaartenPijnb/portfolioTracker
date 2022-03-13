//using PortfolioTracker.Server.Clients;
namespace PortfolioTracker.Server.HostedServices
{
    public class BackgroundProcessesHostedService : IHostedService, IDisposable
    {
        private Timer _timer = null!;

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                    TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            //var test = new YahooFinanceClient
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
