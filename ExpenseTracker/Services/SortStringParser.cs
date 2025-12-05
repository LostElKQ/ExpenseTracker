using ExpenseTracker.Dto;

namespace ExpenseTracker.Services;

public static class SortStringParser
{
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    public static IReadOnlyList<SortingRule> Parse(string? sortString)
    {
        if (string.IsNullOrWhiteSpace(sortString))
        {
            return [];
        }

        string[] parts = sortString.Split(",", SplitOptions);

        List<SortingRule> rules = [];
        HashSet<SortingField> fields = [];

        foreach (string part in parts)
        {
            string[] tokens = part.Split(":", SplitOptions);

            if (tokens.Length != 2)
            {
                throw new ArgumentException($"Invalid sort token: '{part}'");
            }

            if (!Enum.TryParse(tokens[0], true, out SortingField field))
            {
                throw new ArgumentException($"Invalid sort field: '{tokens[0]}'");
            }

            if (!Enum.TryParse(tokens[1], true, out SortingDirection direction))
            {
                throw new ArgumentException($"Invalid sort direction: '{tokens[1]}'");
            }

            if (!fields.Add(field))
            {
                throw new ArgumentException($"Duplicated sort field: '{field}'");
            }

            rules.Add(new SortingRule(field, direction));
        }

        return rules;
    }

    public static bool TryParse(string? sortString, out IReadOnlyList<SortingRule> rules)
    {
        try
        {
            rules = Parse(sortString);

            return true;
        }
        catch (Exception)
        {
            rules = [];

            return false;
        }
    }
}