using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/categories");

        group.MapGet("/", GetAllCategoriesAsync);

        group.MapPost("/", CreateCategoryAsync);

        group.MapPut("/{id:guid}", UpdateCategoryAsync);

        group.MapDelete("/{id:guid}", DeleteCategoryAsync);
    }

    private static async Task<List<CategoryDto>> GetAllCategoriesAsync(ICategoryService service) =>
        await service.GetAllAsync();

    private static async Task<Results<Ok<CategoryDto>, ProblemHttpResult>> CreateCategoryAsync(
        [FromBody] CreateUpdateCategoryDto createUpdateCategoryDto, ICategoryService service)
    {
        Result<CategoryDto, ProblemDetails> result = await service.AddAsync(createUpdateCategoryDto);

        return result.Match<Results<Ok<CategoryDto>, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            error => TypedResults.Problem(error));
    }

    private static async Task<Results<Ok<CategoryDto>, ProblemHttpResult>> UpdateCategoryAsync(
        Guid id, [FromBody] CreateUpdateCategoryDto createUpdateCategoryDto, ICategoryService service)
    {
        Result<CategoryDto, ProblemDetails> result = await service.UpdateAsync(id, createUpdateCategoryDto);

        return result.Match<Results<Ok<CategoryDto>, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            error => TypedResults.Problem(error));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>>
        DeleteCategoryAsync(Guid id, ICategoryService service)
    {
        Result<int, ProblemDetails> result = await service.DeleteAsync(id);

        return result.Match<Results<NoContent, ProblemHttpResult>>(
            _ => TypedResults.NoContent(),
            error => TypedResults.Problem(error));
    }
}