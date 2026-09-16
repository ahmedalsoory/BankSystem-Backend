using Dapper;
using DTOs.Dashboard;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Repositories;
using Repositories.Queries;
using RepositoryContracts.DashboardRepo;
using Shared;
using Shared.Enums.Dashboard;
using Shared.Interfaces;

public class DashboardRepository : BaseRepository, IDashboardRepository
{
    public DashboardRepository(IDbConnectionProvider dbConnection, Context error, IOptions<DbSettings> options)
        : base(dbConnection, error,connectionOptions: options) { }

    public async Task<List<ChartDataPointDto>> GetHistoricalChartDataAsync(metrics metrics)
    {
        base.SetAction();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var data = await connection.QueryAsync<ChartDataPointDto>(
            Query.Dashboard.GetHistoricalData, new { Months = (int)metrics });

        return data.ToList();
    }

    public async Task<(int TotalClients, int TotalAccounts, decimal TotalVolume)> GetSystemCountersAsync()
    {
        base.SetAction();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        return await connection.QuerySingleAsync<(int, int, decimal)>(Query.Dashboard.GetSystemCounters);
    }

    public async Task AggregateYesterdayMetricsAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(Query.Dashboard.AggregateYesterday);
    }
}