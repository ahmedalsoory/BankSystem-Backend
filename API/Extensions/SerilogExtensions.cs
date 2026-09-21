using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace API.Extensions
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("LogDatabase");

            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);

            columnOptions.AdditionalColumns = new List<SqlColumn>
            {
                new SqlColumn { ColumnName = "SourceAction", DataType = SqlDbType.NVarChar, DataLength = 200 }
            };

            var perfColumnOptions = new ColumnOptions();
            perfColumnOptions.Store.Remove(StandardColumn.LogEvent);
            perfColumnOptions.Store.Remove(StandardColumn.MessageTemplate);
            perfColumnOptions.Store.Remove(StandardColumn.Level);
            perfColumnOptions.Store.Remove(StandardColumn.TimeStamp);
            perfColumnOptions.Store.Remove(StandardColumn.Exception);
            perfColumnOptions.Store.Remove(StandardColumn.Properties);

            perfColumnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "Endpoint", DataType = SqlDbType.NVarChar, DataLength = 255 },
                new SqlColumn { ColumnName = "DurationMs", DataType = SqlDbType.BigInt },
                new SqlColumn { ColumnName = "CreatedDate", DataType = SqlDbType.DateTime2 }
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                // --- SINK 1: ERRORS ONLY ---
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "Errors", AutoCreateSqlTable = true },
                        columnOptions: columnOptions
                    ))
                // --- SINK 3: INFORMATION / PERFORMANCE ONLY ---
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.MessageTemplate.ToString().Contains("Performance log"))
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "PerformanceLogs", AutoCreateSqlTable = true },
                        columnOptions: perfColumnOptions
                    ))
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg => {
                System.Diagnostics.Debug.WriteLine("SERILOG ERROR: " + msg);
                Console.Error.WriteLine("SERILOG ERROR: " + msg);
            });

            builder.Host.UseSerilog();

            return builder;
        }
    }
}