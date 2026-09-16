using DTOs.Dashboard;
using Shared.Enums.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardMetricsDto> GetDashboardMetricsSummaryAsync(metrics metrics);
    }
}
