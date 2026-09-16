using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RepositoryContracts.DashboardRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DashboardAggregationWorker : BackgroundService
    {
        private readonly ILogger<DashboardAggregationWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DashboardAggregationWorker(
            ILogger<DashboardAggregationWorker> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Dashboard Midnight Worker initialized.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // 1. Calculate the exact time remaining until midnight
                TimeSpan timeToMidnight = CalculateTimeToMidnight();
                _logger.LogInformation("Worker sleeping for {Hours:F2} hours until midnight sync...", timeToMidnight.TotalHours);

                // 2. Sleep efficiently until the clock rolls over
                await Task.Delay(timeToMidnight, stoppingToken);

                try
                {
                    _logger.LogInformation("Midnight reached! Compiling yesterday's dashboard metrics...");

                    // 3. Create a short-lived scope to call our repository safely
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dashboardRepo = scope.ServiceProvider.GetRequiredService<IDashboardRepository>();

                        // Fire the fast, non-blocking INSERT query
                        await dashboardRepo.AggregateYesterdayMetricsAsync();
                    }

                    _logger.LogInformation("Yesterday's daily metrics successfully saved!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the midnight dashboard insert routine.");
                }
            }
        }

        private TimeSpan CalculateTimeToMidnight()
        {
            DateTime now = DateTime.Now;

            // Target exactly 12:00:05 AM tomorrow to ensure the database server date has rolled over
            DateTime midnight = now.Date.AddDays(1).AddSeconds(5);
            return midnight - now;
        }
    }
}
