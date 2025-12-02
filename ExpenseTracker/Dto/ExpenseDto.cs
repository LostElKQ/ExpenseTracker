namespace ExpenseTracker.Dto;

public sealed record ExpenseDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    decimal Amount,
    DateOnly Date,
    string? Comment);