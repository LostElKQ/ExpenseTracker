using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<List<ExpenseDto>> GetAllWithFilterAsync(FilterOptions filterOptions, IReadOnlyList<SortingRule> rules);
    Task<Result<ExpenseDto, ProblemDetails>> GetByIdAsync(Guid id);
    Task<Result<ExpenseDto, ProblemDetails>> AddAsync(AddExpenseRequestDto expense);
    Task<Result<ExpenseDto, ProblemDetails>> UpdateAsync(Guid id, UpdateExpenseDto expense);
    Task<Result<int, ProblemDetails>> DeleteAsync(Guid id);
}