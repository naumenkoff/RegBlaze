namespace RegBlaze.Domain;

public interface IKeywordChecker
{
    bool CheckForKeyword(string registryKeyName, string valueName, string? value);

    bool Contains(string input);
}