using System;
using System.Threading;
using System.Threading.Tasks;
using ChangelingArionumPool.Services;
using Microsoft.Extensions.Hosting;

namespace ChangelingArionumPool.Jobs
{
    public class Payment : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Configuration.ShareValueProcessingInterval, stoppingToken);
                Console.WriteLine("test");
            }

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000/*_settings.CheckUpdateTime*/, stoppingToken);
            // Run your graceful clean-up actions
        }
    }
}
