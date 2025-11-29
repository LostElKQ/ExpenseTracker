using ExpenseTracker.Dto;

namespace ExpenseTracker.Services;

public interface IStatsService
{
    Task<TotalStatsDto> GetTotalStatsAsync(DateFilterOptions dateFilterOptions);
    Task<CategoryStatsDto[]> GetCategoryStatsAsync(DateFilterOptions dateFilterOptions);
    Task<MonthlyStatsDto[]> GetMonthlyStatsAsync(int year);
}