namespace TagKit;

public static class TagParser
{
    public static IReadOnlyList<Tag> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(tag => new Tag(tag.ToLowerInvariant()))
            .ToArray();
    }
}
