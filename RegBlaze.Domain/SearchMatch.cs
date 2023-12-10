namespace RegBlaze.Domain;

public readonly struct SearchMatch
{
    public required string RegistryKey { get; init; }
    public required string? Name { get; init; }
    public required string? Value { get; init; }
}