using System.Diagnostics.CodeAnalysis;

namespace ExpenseTracker.Dto;

public sealed record DateFilterOptions(
    DateOnly? DateFrom,
    DateOnly? DateTo)
{
    private DateFilterOptions(DateFilterOptions original)
    {
        DateFrom = original.DateFrom ?? new DateOnly(1, 1, 1);
        DateTo = original.DateTo ?? new DateOnly(9999, 12, 31);
    }

    [MemberNotNullWhen(true,
        nameof(DateFrom),
        nameof(DateTo))]
    public static bool Validate(
        DateFilterOptions filterOptions,
        [NotNullWhen(true)] out DateFilterOptions? result)
    {
        if (filterOptions.DateFrom > filterOptions.DateTo)
        {
            result = null;

            return false;
        }

        result = new DateFilterOptions(filterOptions);

        return true;
    }
}