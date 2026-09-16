using DTOs.Dashboard;
using Shared.Enums.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.DashboardRepo
{
    public interface IDashboardRepository
    {
        Task<List<ChartDataPointDto>> GetHistoricalChartDataAsync(metrics metrics);
        Task<(int TotalClients, int TotalAccounts, decimal TotalVolume)> GetSystemCountersAsync();
        Task AggregateYesterdayMetricsAsync();
    }
}
