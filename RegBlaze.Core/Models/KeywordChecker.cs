namespace RegBlaze.Core.Models;

public class KeywordChecker : IKeywordChecker
{
    private string? _keyword;

    public void SetKeyword(string keyword)
    {
        _keyword = keyword;
    }

    public bool CheckForKeyword(string registryKeyName, string valueName, string? value)
    {
        var containsInKeyNameOrValueName = Contains(registryKeyName) || Contains(valueName);
        var containsInValue = value is not null && Contains(value);
        return containsInKeyNameOrValueName || containsInValue;
    }

    public bool Contains(string input)
    {
        return input.Contains(_keyword!, StringComparison.InvariantCultureIgnoreCase);
    }
}