namespace ExpenseTracker.Dto;

public sealed record UpdateExpenseDto(
    Guid? CategoryId,
    decimal? Amount,
    DateOnly? Date,
    string? Comment);