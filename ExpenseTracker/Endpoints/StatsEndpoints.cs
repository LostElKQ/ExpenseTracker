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

    private static async Task<Results<Ok<TotalStatsDto>, BadRequest<ProblemDetails>>> GetTotalStats(
        [AsParameters] DateFilterOptions dateFilterOptions, IStatsService statsService)
    {
        if (dateFilterOptions.DateFrom > dateFilterOptions.DateTo)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid date range",
                Detail = "DateFrom must be less than DateTo",
            });
        }

        dateFilterOptions.DateFrom ??= new DateOnly(1, 1, 1);
        dateFilterOptions.DateTo ??= new DateOnly(9999, 12, 31);

        TotalStatsDto stats = await statsService.GetTotalStatsAsync(dateFilterOptions);

        return TypedResults.Ok(stats);
    }

    private static async Task<Results<Ok<CategoryStatsDto[]>, BadRequest<ProblemDetails>>> GetCategoryStats(
        [AsParameters] DateFilterOptions dateFilterOptions, IStatsService statsService)
    {
        if (dateFilterOptions.DateFrom > dateFilterOptions.DateTo)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid date range",
                Detail = "DateFrom must be less than DateTo",
            });
        }

        dateFilterOptions.DateFrom ??= new DateOnly(1, 1, 1);
        dateFilterOptions.DateTo ??= new DateOnly(9999, 12, 31);

        CategoryStatsDto[] stats = await statsService.GetCategoryStatsAsync(dateFilterOptions);

        return TypedResults.Ok(stats);
    }

    private static async Task<Ok<MonthlyStatsDto[]>> GetMonthlyStats(int year, IStatsService statsService)
    {
        MonthlyStatsDto[] stats = await statsService.GetMonthlyStatsAsync(year);

        return TypedResults.Ok(stats);
    }
}