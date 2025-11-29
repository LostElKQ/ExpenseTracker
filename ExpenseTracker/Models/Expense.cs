namespace ExpenseTracker.Models;

public sealed class Expense
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Comment { get; set; }

    public Category Category { get; set; } = null!;
}
