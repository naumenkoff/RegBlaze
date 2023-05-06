namespace RegBlaze.Core.Models;

public interface IKeywordChecker
{
    void SetKeyword(string keyword);
    bool CheckForKeyword(string registryKeyName, string valueName, string? value);
}