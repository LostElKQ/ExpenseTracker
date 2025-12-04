using System.Globalization;
using ExpenseTracker;
using ExpenseTracker.Context;
using ExpenseTracker.Endpoints;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var culture = new CultureInfo("en-GB")
{
    DateTimeFormat =
    {
        ShortDatePattern = "yyyy-MM-dd",
        YearMonthPattern = "yyyy-MM",
    },
};


CultureInfo[] supportedCultures = [culture];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs", rollingInterval: RollingInterval.Day)
    .WriteTo.Debug(new CompactJsonFormatter())
    .CreateBootstrapLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddNpgsql<ApplicationContext>(builder.Configuration.GetConnectionString("Default"));

builder.Services.AddSwaggerGen(options =>
{
    options.MapType<DateOnly>(() =>
        new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Format = "date",
            Example = DateTime.Now.ToString(culture.DateTimeFormat.ShortDatePattern),
        });
});

builder.Services.AddSerilog((provider, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
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

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
});

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