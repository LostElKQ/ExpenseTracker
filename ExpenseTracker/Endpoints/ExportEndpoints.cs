using System.Text;
using ExpenseTracker.Context;
using ExpenseTracker.Dto;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseTracker.Endpoints;

public static class ExportEndpoints
{
    public static void MapExportEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/export");

        group.MapGet("/csv", ExportCsvAsync);
    }

    private static async Task<ContentHttpResult> ExportCsvAsync(
        [AsParameters] DateFilterOptions filterOptions,
        IExportService service)
    {
        filterOptions.DateFrom ??= new DateOnly(1, 1, 1);
        DateTime now = DateTime.Now;
        filterOptions.DateTo ??= new DateOnly(now.Year, now.Month, now.Day);

        string csv = await service.ExportCsvAsync(filterOptions);

        return TypedResults.Text(csv, "text/csv", new UTF8Encoding());
    }
}