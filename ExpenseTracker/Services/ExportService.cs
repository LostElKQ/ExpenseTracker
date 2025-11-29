using System.Text;
using ExpenseTracker.Context;
using ExpenseTracker.Dto;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public sealed class ExportService(ApplicationContext db) : IExportService
{
    public async Task<string> ExportCsvAsync(DateFilterOptions filterOptions)
    {
        var expenses = await db.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date >= filterOptions.DateFrom && e.Date <= filterOptions.DateTo)
            .Select(e => new { e.Date, e.Category.Name, e.Amount, e.Comment })
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Name)
            .ToListAsync();

        var csv = new StringBuilder("Date,Category,Amount,Comment\n");

        foreach (var expense in expenses)
        {
            csv.Append(
                $"{expense.Date.ToString("yyyy-MM-dd")},{expense.Name},{expense.Amount},\"{expense.Comment}\"\n");
        }

        return csv.ToString();
    }
}