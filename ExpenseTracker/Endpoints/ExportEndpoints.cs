using System.Text;
using ExpenseTracker.Context;
using ExpenseTracker.Dto;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints;

public static class ExportEndpoints
{
    public static void MapExportEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/export");

        group.MapGet("/csv", ExportCsvAsync)
            .Produces<string>(contentType: "text/csv");
    }

    private static async Task<Results<ContentHttpResult, BadRequest<ProblemDetails>>> ExportCsvAsync(
        [AsParameters] DateFilterOptions dateFilterOptions,
        IExportService service)
    {
        if (!DateFilterOptions.Validate(dateFilterOptions, out DateFilterOptions? validated))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid date filter options",
                Detail = "Invalid data range.",
            });
        }

        string csv = await service.ExportCsvAsync(validated);

        return TypedResults.Text(csv,
            "text/csv",
            new UTF8Encoding());
    }
}