using Microsoft.Win32;
using RegBlaze.Domain;

namespace RegBlaze.Infrastructe;

public class RegistryKeyProcessor : IRegistryKeyProcessor
{
    private readonly IKeywordChecker _keywordChecker;

    public RegistryKeyProcessor(IKeywordChecker keywordChecker)
    {
        _keywordChecker = keywordChecker;
    }

    public void ProcessRegistryKey(RegistryKey key, List<SearchMatch> searchMatches)
    {
        var valueNames = key.GetValueNames().AsSpan();
        if (valueNames.Length == 0)
        {
            var segments = key.Name.Split('\\').AsSpan();
            if (!_keywordChecker.Contains(segments[^1])) return;

            // This is necessary in order to avoid the following situations:
            // We are looking for SomeKeyExample, there are keys: (True - Add and return, False - just return)
            // Software\SomeKeyExample - with or without checking - True
            // Software\SomeKeyExample\NestedKeyExample - with checking False, without checking - True
            // Software\SomeKeyExample\NestedKeyExample\Customers - with checking False, without checking - True
            // Without checking the last value, if checking the entire string, all keys will be added.
            var searchMatch = new SearchMatch(key.Name, default, default);
            searchMatches.Add(searchMatch);
        }
        else
        {
            foreach (var valueName in valueNames)
            {
                var value = key.GetValue(valueName) as string;
                if (!_keywordChecker.CheckForKeyword(key.Name, valueName, value)) continue;

                var searchMatch = new SearchMatch(key.Name, valueName, value);
                searchMatches.Add(searchMatch);
            }
        }
    }
}