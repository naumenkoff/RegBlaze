using RegBlaze.Domain;

namespace RegBlaze.Infrastructe;

public class KeywordChecker : IKeywordChecker
{
    private readonly string _keyword;

    public KeywordChecker(string keyword)
    {
        _keyword = keyword;
    }

    public bool CheckForKeyword(string registryKeyName, string valueName, string? value)
    {
        return Contains(registryKeyName) || Contains(valueName) || Contains(value);
    }

    public bool Contains(string? input)
    {
        return input?.Contains(_keyword, StringComparison.OrdinalIgnoreCase) is true;
    }
}