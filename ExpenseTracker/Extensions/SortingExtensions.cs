using ExpenseTracker.Dto;

namespace ExpenseTracker.Extensions.Sorting;

public static class SortingExtensions
{
    extension(IAsyncEnumerable<ExpenseDto> query)
    {
        public IAsyncEnumerable<ExpenseDto> ApplySorting(IReadOnlyList<SortingRule> sortingRules)
        {
            if (sortingRules.Count == 0)
            {
                return query;
            }

            var first = true;

            foreach (SortingRule sortingRule in sortingRules)
            {
                query = query.ApplyOnce(sortingRule, first);
                first = false;
            }

            return query;
        }

        private IAsyncEnumerable<ExpenseDto> ApplyOnce(SortingRule sortingRule, bool isFirst)
        {
            return (sortingRule.Field, sortingRule.Direction, isFirst) switch
            {
                (SortingField.Date, SortingDirection.Asc, true) => query.OrderBy(e =>
                    (e.Date.Year, e.Date.Month)),
                (SortingField.Date, SortingDirection.Desc, true) => query.OrderByDescending(e =>
                    (e.Date.Year, e.Date.Month)),
                (SortingField.Date, SortingDirection.Asc, false) => ((IOrderedAsyncEnumerable<ExpenseDto>)query)
                    .ThenBy(e => (e.Date.Year, e.Date.Month)),
                (SortingField.Date, SortingDirection.Desc, false) => ((IOrderedAsyncEnumerable<ExpenseDto>)query)
                    .ThenByDescending(e => (e.Date.Year, e.Date.Month)),

                (SortingField.Amount, SortingDirection.Asc, true) => query.OrderBy(e => e.Amount),
                (SortingField.Amount, SortingDirection.Desc, true) => query.OrderByDescending(e => e.Amount),
                (SortingField.Amount, SortingDirection.Asc, false) => ((IOrderedAsyncEnumerable<ExpenseDto>)query)
                    .ThenBy(e => e.Amount),
                (SortingField.Amount, SortingDirection.Desc, false) => ((IOrderedAsyncEnumerable<ExpenseDto>)query)
                    .ThenByDescending(e => e.Amount),

                _ => query,
            };
        }
    }
}