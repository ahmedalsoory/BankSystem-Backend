using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Dashboard
{
    public class DashboardMetricsDto
    {
        public int TotalClients { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalTransactionVolume { get; set; }

        // Chart Time-Series Data Array
        public List<ChartDataPointDto> ChartData { get; set; }
    }
}
