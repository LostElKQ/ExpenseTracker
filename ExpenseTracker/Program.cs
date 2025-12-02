using ExpenseTracker;
using ExpenseTracker.Context;
using ExpenseTracker.Endpoints;
using ExpenseTracker.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: 
        "[{{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs", rollingInterval: RollingInterval.Day)
    .WriteTo.Debug(new CompactJsonFormatter())
    .CreateBootstrapLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddNpgsql<ApplicationContext>(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog((provider, configuration) =>
{
    configuration.ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(provider)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Debug(new CompactJsonFormatter())
        .WriteTo.File("logs/log", rollingInterval: RollingInterval.Day);
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IExportService, ExportService>();

WebApplication app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapExpenseEndpoints();
app.MapCategoryEndpoints();
app.MapStatsEndpoints();
app.MapExportEndpoints();

app.MapFallbackToFile("index.html");

app.Run();