using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace ExpenseTracker.Dto;

public sealed record FilterOptions(
    Guid[]? CategoryIds,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    decimal? MinAmount,
    decimal? MaxAmount,
    int Page,
    int Size)
{
    private FilterOptions(FilterOptions original)
    {
        CategoryIds ??= original.CategoryIds ?? [];
        DateFrom ??= original.DateFrom ?? new DateOnly(1, 1, 1);
        DateTo ??= original.DateTo ?? new DateOnly(9999, 12, 31);
        MinAmount ??= original.MinAmount ?? decimal.MinValue;
        MaxAmount ??= original.MaxAmount ?? decimal.MaxValue;
        Page = original.Page;
        Size = original.Size;
    }


    [MemberNotNullWhen(true,
        nameof(CategoryIds),
        nameof(DateFrom),
        nameof(DateTo),
        nameof(MinAmount),
        nameof(MaxAmount))]
    public static bool Validate(
        FilterOptions filterOptions,
        [NotNullWhen(true)] out FilterOptions? result)
    {
        if (filterOptions.DateFrom > filterOptions.DateTo || filterOptions.MinAmount > filterOptions.MaxAmount)
        {
            result = null;

            return false;
        }

        result = new FilterOptions(filterOptions);

        return true;
    }
}