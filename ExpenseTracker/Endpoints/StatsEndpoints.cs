using ExpenseTracker.Dto;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints;

public static class StatsEndpoints
{
    public static void MapStatsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/stats");

        group.MapGet("/total", GetTotalStats);
        group.MapGet("/by-category", GetCategoryStats);
        group.MapGet("/monthly/{year:int}", GetMonthlyStats);
    }

    private static async Task<Results<Ok<TotalStatsDto>, ProblemHttpResult>> GetTotalStats(
        [AsParameters] DateFilterOptions dateFilterOptions, IStatsService statsService)
    {
        if (!DateFilterOptions.Validate(dateFilterOptions, out DateFilterOptions? validated))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid date filter options",
                Detail = "Invalid data range.",
            });
        }

        TotalStatsDto stats = await statsService.GetTotalStatsAsync(validated);

        return TypedResults.Ok(stats);
    }

    private static async Task<Results<Ok<CategoryStatsDto[]>, ProblemHttpResult>> GetCategoryStats(
        [AsParameters] DateFilterOptions dateFilterOptions, IStatsService statsService)
    {
        if (!DateFilterOptions.Validate(dateFilterOptions, out DateFilterOptions? validated))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid date filter options",
                Detail = "Invalid data range.",
            });
        }

        CategoryStatsDto[] stats = await statsService.GetCategoryStatsAsync(validated);

        return TypedResults.Ok(stats);
    }

    private static async Task<Ok<MonthlyStatsDto[]>> GetMonthlyStats(int year, IStatsService statsService)
    {
        MonthlyStatsDto[] stats = await statsService.GetMonthlyStatsAsync(year);

        return TypedResults.Ok(stats);
    }
}