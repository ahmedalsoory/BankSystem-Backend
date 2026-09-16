
using API.Extensions;
using API.Filter;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Service;
using Shared;
using System.Collections.ObjectModel;
using System.Data;
using Serilog.Filters;
using API.Filter.Extensions;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

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

// Map the structured properties to your existing database columns
perfColumnOptions.AdditionalColumns = new Collection<SqlColumn>
{
    new SqlColumn { ColumnName = "Endpoint", DataType = SqlDbType.NVarChar, DataLength = 255 },
    new SqlColumn { ColumnName = "DurationMs", DataType = SqlDbType.BigInt },
    new SqlColumn { ColumnName = "CreatedDate", DataType = SqlDbType.DateTime2 }
};


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // -- - SINK 1: ERRORS ONLY ---
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
        .WriteTo.MSSqlServer(
            connectionString: connectionString,
           sinkOptions: new MSSqlServerSinkOptions { TableName = "Errors", AutoCreateSqlTable = true },
           columnOptions: columnOptions
        ))
// --- SINK 2: WARNINGS ONLY ---
//.WriteTo.Logger(lc => lc
//    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
//    .WriteTo.MSSqlServer(
//        connectionString: connectionString,
//        sinkOptions: new MSSqlServerSinkOptions { TableName = "Warnings", AutoCreateSqlTable = true }
//    ))
// --- SINK 3: INFORMATION ONLY ---
  .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => 
        e.MessageTemplate.ToString().Contains("Performance log")) // Only takes logs with Endpoint property
        .WriteTo.MSSqlServer(
            connectionString: connectionString,
            sinkOptions: new MSSqlServerSinkOptions { TableName = "PerformanceLogs", AutoCreateSqlTable = true },
            columnOptions: perfColumnOptions // Use your custom perfColumnOptions here
        ))

    .CreateLogger();
Serilog.Debugging.SelfLog.Enable(msg => {
    System.Diagnostics.Debug.WriteLine("SERILOG ERROR: " + msg);
    Console.Error.WriteLine("SERILOG ERROR: " + msg);
});





builder.Host.UseSerilog();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddHostedService<DashboardAggregationWorker>();


builder.Services.AddApplicationServices();
builder.Services.AddSharedServices();
builder.Services.AddBankingFiltersServices(); 
builder.Services.AddControllers(options =>
{
    options.AddBankingFilters(); // Link your filters here
});

var app = builder.Build();
app.UseApplicationMiddleware(app.Environment);

app.MapControllers();

app.Run();





public partial class Program { }
