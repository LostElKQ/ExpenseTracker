namespace ExpenseTracker.Dto;

public sealed record SortingRule(
    SortingField Field,
    SortingDirection Direction);