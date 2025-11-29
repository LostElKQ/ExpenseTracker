namespace ExpenseTracker.Dto;

public sealed record CategoryStatsDto(string Category, decimal Total, decimal Income, decimal Expenses);