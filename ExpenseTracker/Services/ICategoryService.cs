using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<Result<CategoryDto, ProblemDetails>> AddAsync(CreateUpdateCategoryDto createUpdateCategoryDto);
    Task<Result<CategoryDto, ProblemDetails>> UpdateAsync(Guid id, CreateUpdateCategoryDto createUpdateCategoryDto);
    Task<Result<int, ProblemDetails>> DeleteAsync(Guid id);
}