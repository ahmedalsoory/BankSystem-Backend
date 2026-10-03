using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract
{
    public interface IPerformanceAlertService
    {
        Task SendSlowEndpointAlertAsync(string endpoint, long durationMs);
        Task SendErrorAlertAsync(string title, string errorDetails);
    }
}
