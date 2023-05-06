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
        if (valueNames.Length == 0) return;

        foreach (var valueName in valueNames)
        {
            var value = key.GetValue(valueName) as string;
            if (keywordChecker.CheckForKeyword(key.Name, valueName, value) is false) continue;

            var searchMatch = new SearchMatch(key.Name, valueName, value);
            searchMatches.Enqueue(searchMatch);
        }
    }
}