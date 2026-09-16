using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Dashboard
{
    public readonly struct ChartDataPointDto
    {
        public DateTime Date { get; init; } 
        public decimal Deposits { get; init; }
        public decimal Withdrawals { get; init; }
        public int TransactionCount { get; init; }
    }
}
