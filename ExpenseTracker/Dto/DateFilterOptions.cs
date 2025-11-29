namespace ExpenseTracker.Dto;

public sealed class DateFilterOptions(
    DateOnly? dateFrom = null,
    DateOnly? dateTo = null)
{
    public DateOnly? DateFrom { get; set; } = dateFrom;
    public DateOnly? DateTo { get; set; } = dateTo;
}