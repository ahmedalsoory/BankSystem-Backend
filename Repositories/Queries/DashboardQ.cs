using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class DashboardQ
    {
        public const string GetHistoricalData = @"
                SELECT MetricDate as [Date], TotalDeposits as Deposits, TotalWithdrawals as Withdrawals, TransactionCount
                FROM System.DailyDashboardMetrics
                WHERE MetricDate >= DATEADD(month, -@Months, GETDATE())
                  AND MetricDate < CAST(GETDATE() AS DATE)
                ORDER BY MetricDate ASC;";

        public const string GetSystemCounters = @"
                SELECT 
                    (SELECT SUM(rows) FROM sys.partitions WHERE object_id = OBJECT_ID('Core.Clients') AND index_id IN (0, 1)) as TotalClients,
                    (SELECT SUM(rows) FROM sys.partitions WHERE object_id = OBJECT_ID('Banking.Accounts') AND index_id IN (0, 1)) as TotalAccounts,
                    (SELECT ISNULL(SUM(TotalDeposits + TotalWithdrawals), 0) FROM System.DailyDashboardMetrics) as TotalVolume;";

        public const string AggregateYesterday = @"
                INSERT INTO System.DailyDashboardMetrics (MetricDate, TotalDeposits, TotalWithdrawals, TransactionCount)
                SELECT 
                    CAST(TransactionDate AS DATE) as MetricDate,
                    SUM(CASE WHEN Type = 1 THEN Amount ELSE 0 END) as TotalDeposits,
                    SUM(CASE WHEN Type = 2 THEN Amount WHEN Type = 3 THEN Amount ELSE 0 END) as TotalWithdrawals,
                    COUNT(*) as TransactionCount
                FROM Banking.Transactions
                WHERE TransactionDate >= CAST(DATEADD(day, -1, GETDATE()) AS DATE)
                  AND TransactionDate < CAST(GETDATE() AS DATE)
                GROUP BY CAST(TransactionDate AS DATE);";
    }
}
