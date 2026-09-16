using DTOs.Dashboard;
using RepositoryContracts.DashboardRepo;
using ServiceContract.Dashboard;
using Shared.Enums.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public async Task<DashboardMetricsDto> GetDashboardMetricsSummaryAsync(metrics metrics)
        {
            // 1. Fire both optimized database calls
            var countersTask = _dashboardRepo.GetSystemCountersAsync();
            var chartDataTask = _dashboardRepo.GetHistoricalChartDataAsync(metrics);

            // Run concurrently to maximize server thread utilization
            await Task.WhenAll(countersTask, chartDataTask);

            var counters = await countersTask;
            var chartData = await chartDataTask;

            // 2. Wrap them into the single Dashboard Class DTO
            return new DashboardMetricsDto
            {
                TotalClients = counters.TotalClients,
                TotalAccounts = counters.TotalAccounts,
                TotalTransactionVolume = counters.TotalVolume,
                ChartData = chartData // Passes the reference pointer to the struct list
            };
        }
    }
}
