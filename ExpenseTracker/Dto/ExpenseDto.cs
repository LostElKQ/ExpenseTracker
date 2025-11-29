namespace ExpenseTracker.Dto;

public record ExpenseDto(
    Guid Id,
    string CategoryName,
    decimal Amount,
    DateOnly Date,
    string? Comment);