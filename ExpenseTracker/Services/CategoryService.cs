using System.Runtime.InteropServices.JavaScript;
using ExpenseTracker.Context;
using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public sealed class CategoryService(ApplicationContext db) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await db.Categories
            .AsAsyncEnumerable()
            .Select(c => new CategoryDto(c.Id, c.Name))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Result<CategoryDto, ProblemDetails>> AddAsync(CreateUpdateCategoryDto createUpdateCategoryDto)
    {
        int count = await db.Categories.CountAsync(c =>
            string.Equals(c.Name, createUpdateCategoryDto.Name));

        if (count > 0)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Failed to create category",
                Detail = "Category already exists",
            };
        }

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = createUpdateCategoryDto.Name,
        };

        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();

        return new CategoryDto(category.Id, category.Name);
    }

    public async Task<Result<CategoryDto, ProblemDetails>> UpdateAsync(
        Guid id, CreateUpdateCategoryDto createUpdateCategoryDto)
    {
        Category? category = await db.Categories.FindAsync(id);

        if (category is null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Failed to update category",
                Detail = "Category not found",
            };
        }

        category.Name = createUpdateCategoryDto.Name;

        await db.SaveChangesAsync();

        return new CategoryDto(category.Id, category.Name);
    }

    public async Task<Result<int, ProblemDetails>> DeleteAsync(Guid id)
    {
        Category? category = await db.Categories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category?.Expenses.Count > 0)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Failed to delete category",
                Detail = "Category has expenses",
            };
        }

        if (category == null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Failed to delete category",
                Detail = "Category not found",
            };
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return 0;
    }
}