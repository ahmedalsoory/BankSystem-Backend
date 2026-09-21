using API.Extensions;
using API.Filter.Extensions;
using Service;
using Shared;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Serilog using your new extension method
builder.AddCustomSerilog();

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