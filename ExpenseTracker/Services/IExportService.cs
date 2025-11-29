using ExpenseTracker.Dto;

namespace ExpenseTracker.Services;

public interface IExportService
{
    Task<string> ExportCsvAsync(DateFilterOptions filterOptions);
}