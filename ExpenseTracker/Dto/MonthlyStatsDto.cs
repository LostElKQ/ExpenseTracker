namespace ExpenseTracker.Dto;

public sealed record MonthlyStatsDto(string Date, decimal Total, decimal Income, decimal Expenses);