using ExpenseTracker.Context;
using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public sealed class ExpenseService(ApplicationContext db) : IExpenseService
{
    public async Task<List<ExpenseDto>> GetAllWithFilterAsync(FilterOptions filterOptions)
    {
        return await db.Expenses
            .Include(e => e.Category)
            .Where(e => filterOptions.CategoryId == null || e.CategoryId == filterOptions.CategoryId)
            .Where(e => e.Date >= filterOptions.DateFrom && e.Date <= filterOptions.DateTo)
            .Where(e => e.Amount >= filterOptions.MinAmount && e.Amount <= filterOptions.MaxAmount)
            .Select(e => new ExpenseDto(e.Id, e.CategoryId, e.Category.Name, e.Amount, e.Date, e.Comment))
            .AsAsyncEnumerable()
            .OrderBy(e => e.Date)
            .ThenByDescending(e => e.Amount)
            .ThenBy(e => e.CategoryName)
            .Skip(filterOptions.Page * filterOptions.Size)
            .Take(filterOptions.Size)
            .ToListAsync();
    }

    public async Task<Result<ExpenseDto, ProblemDetails>> GetByIdAsync(Guid id)
    {
        Expense? expense = await db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Expense not found",
                Detail = $"Expense with id {id} not found",
            };
        }

        return new ExpenseDto(expense.Id, expense.CategoryId, expense.Category.Name, expense.Amount, expense.Date, expense.Comment);
    }

    public async Task<Result<ExpenseDto, ProblemDetails>> AddAsync(AddExpenseRequestDto expense)
    {
        Category? category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == expense.CategoryId);

        if (category == null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Failed to add expense",
                Detail = $"Category with id {expense.CategoryId} not found",
            };
        }

        Expense newExpense = new()
        {
            Id = Guid.NewGuid(),
            CategoryId = expense.CategoryId,
            Amount = expense.Amount,
            Date = expense.Date,
            Comment = expense.Comment,
        };

        await db.Expenses.AddAsync(newExpense);
        await db.SaveChangesAsync();

        return new ExpenseDto(newExpense.Id,
            newExpense.CategoryId,
            newExpense.Category.Name,
            newExpense.Amount,
            newExpense.Date,
            newExpense.Comment);
    }

    public async Task<Result<ExpenseDto, ProblemDetails>> UpdateAsync(Guid id, UpdateExpenseDto expense)
    {
        Expense? expenseToUpdate = await db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expenseToUpdate == null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Failed to update expense",
                Detail = $"Expense with id {id} not found",
            };
        }

        if (expense.CategoryId is not null && expense.CategoryId != expenseToUpdate.CategoryId)
        {
            expenseToUpdate.CategoryId = expense.CategoryId.Value;
        }

        if (expense.Amount is not null && expense.Amount != expenseToUpdate.Amount)
        {
            expenseToUpdate.Amount = expense.Amount.Value;
        }

        if (expense.Date is not null && expense.Date != expenseToUpdate.Date)
        {
            expenseToUpdate.Date = expense.Date.Value;
        }

        if (expense.Comment is not null && expense.Comment != expenseToUpdate.Comment)
        {
            expenseToUpdate.Comment = expense.Comment;
        }

        await db.SaveChangesAsync();

        return new ExpenseDto(expenseToUpdate.Id,
            expenseToUpdate.CategoryId,
            expenseToUpdate.Category.Name,
            expenseToUpdate.Amount,
            expenseToUpdate.Date,
            expenseToUpdate.Comment);
    }

    public async Task<Result<int, ProblemDetails>> DeleteAsync(Guid id)
    {
        Expense? expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Failed to delete expense",
                Detail = $"Expense with id {id} not found",
            };
        }

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();

        return 0;
    }
}