using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/expenses");

        group.MapGet("/", GetAllExpensesWithFilterAsync);
        group.MapGet("/{id:guid}", GetExpenseByIdAsync);
        group.MapPost("/", AddExpenseAsync);
        group.MapPut("/{id:guid}", UpdateExpenseAsync);
        group.MapDelete("/{id:guid}", DeleteExpenseAsync);
    }

    private static async Task<Results<Ok<List<ExpenseDto>>, BadRequest<ProblemDetails>>> GetAllExpensesWithFilterAsync(
        [AsParameters] FilterOptions filterOptions,
        IExpenseService service)
    {
        if (!FilterOptions.Validate(filterOptions, out FilterOptions? validated))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid filter options",
                Detail = "Invalid data range",
            });
        }

        return TypedResults.Ok(await service.GetAllWithFilterAsync(validated));
    }

    private static async Task<Results<Ok<ExpenseDto>, NotFound<ProblemDetails>>> GetExpenseByIdAsync(
        Guid id, IExpenseService service)
    {
        Result<ExpenseDto, ProblemDetails> result = await service.GetByIdAsync(id);

        return result.Match<Results<Ok<ExpenseDto>, NotFound<ProblemDetails>>>(
            success => TypedResults.Ok(success),
            error => TypedResults.NotFound(error));
    }

    private static async Task<Results<Ok<ExpenseDto>, NotFound<ProblemDetails>>> AddExpenseAsync(
        AddExpenseRequestDto expense, IExpenseService service)
    {
        Result<ExpenseDto, ProblemDetails> result = await service.AddAsync(expense);

        return result.Match<Results<Ok<ExpenseDto>, NotFound<ProblemDetails>>>(
            success => TypedResults.Ok(success),
            error => TypedResults.NotFound(error));
    }

    private static async Task<Results<Ok<ExpenseDto>, NotFound<ProblemDetails>>> UpdateExpenseAsync(
        Guid id, UpdateExpenseDto expense, IExpenseService service)
    {
        Result<ExpenseDto, ProblemDetails> result = await service.UpdateAsync(id, expense);

        return result.Match<Results<Ok<ExpenseDto>, NotFound<ProblemDetails>>>(
            success => TypedResults.Ok(success),
            error => TypedResults.NotFound(error));
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeleteExpenseAsync(
        Guid id, IExpenseService service)
    {
        Result<int, ProblemDetails> result = await service.DeleteAsync(id);

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => TypedResults.NotFound(error));
    }
}