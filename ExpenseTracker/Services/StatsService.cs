using ExpenseTracker.Context;
using ExpenseTracker.Dto;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public sealed class StatsService(ApplicationContext db) : IStatsService
{
    public async Task<TotalStatsDto> GetTotalStatsAsync(DateFilterOptions dateFilterOptions)
    {
        var total = await db.Expenses
            .Where(e => e.Date >= dateFilterOptions.DateFrom && e.Date <= dateFilterOptions.DateTo)
            .GroupBy(e => e.Amount > 0 ? "Income" : "Expenses")
            .Select(g => new { g.Key, Sum = g.Sum(e => e.Amount) })
            .ToListAsync();

        decimal income = total.FirstOrDefault(g => g.Key == "Income")?.Sum ?? 0;
        decimal expenses = total.FirstOrDefault(g => g.Key == "Expenses")?.Sum ?? 0;

        return new TotalStatsDto(income + expenses, income, expenses);
    }

    public async Task<CategoryStatsDto[]> GetCategoryStatsAsync(DateFilterOptions dateFilterOptions)
    {
        var categories = await db.Categories
            .Include(c => c.Expenses)
            .Select(c => new
            {
                Category = c.Name,
                Expenses = c.Expenses
                    .Where(e => e.Date >= dateFilterOptions.DateFrom && e.Date <= dateFilterOptions.DateTo)
                    .GroupBy(e => e.Amount > 0 ? "Income" : "Expenses")
                    .Select(g => new { g.Key, Sum = g.Sum(e => e.Amount) })
                    .ToList(),
            })
            .Where(c => c.Expenses.Count > 0)
            .ToListAsync();


        return categories.Select(c =>
            {
                decimal income = c.Expenses.FirstOrDefault(e => e.Key == "Income")?.Sum ?? 0;
                decimal expenses = c.Expenses.FirstOrDefault(e => e.Key == "Expenses")?.Sum ?? 0;

                return new CategoryStatsDto(c.Category, income + expenses, income, expenses);
            })
            .OrderBy(c => c.Total)
            .ToArray();
    }

    public async Task<MonthlyStatsDto[]> GetMonthlyStatsAsync(int year)
    {
        var monthly = await db.Expenses
            .Where(e => e.Date.Year == year)
            .GroupBy(e => e.Date.Month)
            .Select(g => new
            {
                g.Key,
                Expenses = g.GroupBy(e => e.Amount > 0 ? "Income" : "Expenses")
                    .Select(g1 => new { g1.Key, Sum = g1.Sum(e => e.Amount) })
                    .ToList(),
            })
            .ToListAsync();

        return monthly.Select(m =>
            {
                decimal income = m.Expenses.FirstOrDefault(e => e.Key == "Income")?.Sum ?? 0;
                decimal expenses = m.Expenses.FirstOrDefault(e => e.Key == "Expenses")?.Sum ?? 0;

                return new MonthlyStatsDto(
                    new DateOnly(year, m.Key, 1).ToString("yyyy-MM"),
                    income + expenses,
                    income,
                    expenses);
            })
            .ToArray();
    }
}