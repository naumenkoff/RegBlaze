using System.Collections.Concurrent;
using Microsoft.Win32;
using RegBlaze.Core.Models;

namespace RegBlaze.Core.Services;

public class RegistryKeyProcessor : IRegistryKeyProcessor
{
    public void ProcessRegistryKeyForSearchMatches(RegistryKey key, ConcurrentQueue<SearchMatch> searchMatches,
        IKeywordChecker keywordChecker)
    {
        var valueNames = key.GetValueNames();
        if (valueNames.Length == 0)
        {
            var segments = key.Name.Split('\\');
            if (keywordChecker.Contains(segments[^1]))
                /*
                 This is necessary in order to avoid the following situations:
                 We are looking for SomeKeyExample, there are keys: (True - Enqueue and return, False - just return)
                 Software\SomeKeyExample - with or without checking - True
                 Software\SomeKeyExample\NestedKeyExample - with checking False, without checking - True
                 Software\SomeKeyExample\NestedKeyExample\Customers - with checking False, without checking - True
                 Without checking the last value, if checking the entire string, all keys will be added.
                 */
                searchMatches.Enqueue(new SearchMatch(key.Name, default, default));

            return;
        }

        foreach (var valueName in valueNames)
        {
            var value = key.GetValue(valueName) as string;
            if (keywordChecker.CheckForKeyword(key.Name, valueName, value) is false) continue;
            var searchMatch = new SearchMatch(key.Name, valueName, value);
            searchMatches.Enqueue(searchMatch);
        }
    }
}