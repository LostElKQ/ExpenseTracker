namespace ExpenseTracker.Dto;

public sealed record AddExpenseRequestDto(
    Guid CategoryId,
    decimal Amount,
    DateOnly Date,
    string? Comment);