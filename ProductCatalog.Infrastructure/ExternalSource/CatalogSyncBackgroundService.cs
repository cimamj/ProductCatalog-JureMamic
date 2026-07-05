using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProductCatalog.Infrastructure.ExternalSource
{
    public class CatalogSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CatalogSyncBackgroundService> _logger;

        public CatalogSyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CatalogSyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextMidnight();
                _logger.LogInformation("Next scheduled catalog sync at {NextRunAt}.", DateTime.Now.Add(delay));

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                if (stoppingToken.IsCancellationRequested)
                    break;

                await RunSyncAsync();
            }
        }

        private async Task RunSyncAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<CatalogSyncService>();
                await syncService.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled catalog sync failed.");
            }
        }

        private static TimeSpan GetDelayUntilNextMidnight()
        {
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1);
            return nextMidnight - now;
        }
    }
}
