namespace ExpenseTracker.Dto;

public record UpdateExpenseDto(
    Guid? CategoryId,
    decimal? Amount,
    DateOnly? Date,
    string? Comment);