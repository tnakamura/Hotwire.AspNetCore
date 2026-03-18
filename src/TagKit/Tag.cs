namespace TagKit;

public sealed record Tag
{
    public Tag(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public string Name { get; }
}
