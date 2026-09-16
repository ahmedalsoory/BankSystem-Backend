using DTOs.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.Dashboard;
using Shared.Enums.Dashboard;

namespace API.Controllers
{

    public class DashboardController : MyControllerBase
    {

        private readonly IDashboardService _dashboardService;

        public DashboardController(ILoggerFactory logger, IDashboardService dashboardService) : base(logger)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("{metrics}")]
        public async Task<ActionResult<DashboardMetricsDto>> GetDashboard(metrics metrics)
        {
            var dashboardData = await _dashboardService.GetDashboardMetricsSummaryAsync( metrics).ConfigureAwait(false); ;
            return Ok(dashboardData);
        }

    }
}
