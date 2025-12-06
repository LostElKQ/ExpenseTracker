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

    private static async Task<Results<Ok<CategoryDto>, BadRequest<ProblemDetails>>> CreateCategoryAsync(
        [FromBody] CreateUpdateCategoryDto createUpdateCategoryDto, ICategoryService service)
    {
        Result<CategoryDto, ProblemDetails> result = await service.AddAsync(createUpdateCategoryDto);

        return result.Match<Results<Ok<CategoryDto>, BadRequest<ProblemDetails>>>(
            success => TypedResults.Ok(success),
            error => TypedResults.BadRequest(error));
    }

    private static async Task<Results<Ok<CategoryDto>, NotFound<ProblemDetails>>> UpdateCategoryAsync(
        Guid id, [FromBody] CreateUpdateCategoryDto createUpdateCategoryDto, ICategoryService service)
    {
        Result<CategoryDto, ProblemDetails> result = await service.UpdateAsync(id, createUpdateCategoryDto);

        return result.Match<Results<Ok<CategoryDto>, NotFound<ProblemDetails>>>(
            success => TypedResults.Ok(success),
            error => TypedResults.NotFound(error));
    }

    private static async Task<Results<NoContent, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>>
        DeleteCategoryAsync(Guid id, ICategoryService service)
    {
        Result<int, ProblemDetails> result = await service.DeleteAsync(id);

        return result.Match<Results<NoContent, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error =>
            {
                return error.Status switch
                {
                    400 => TypedResults.BadRequest(error),
                    404 => TypedResults.NotFound(error),
                    _ => TypedResults.BadRequest(new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Failed to delete category",
                        Detail = "Something went wrong",
                    }),
                };
            });
    }
}