namespace ExpenseTracker.Dto;

public sealed class FilterOptions
{
    public Guid? CategoryId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}