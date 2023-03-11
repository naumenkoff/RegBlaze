using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security;
using Microsoft.Win32;
using RegistrySearcher.Models;

namespace RegistrySearcher.Services;

/// <summary>
///     Represents a service for searching a Windows registry for a given keyword.
/// </summary>
public class SearchService
{
    private readonly string _keyword;
    private int _numberOfScannedKeys;
    private int _numberOfUnsuccessfullyScannedKeys;
    private TimeSpan _registrySearchTime;

    /// <summary>
    ///     Creates a new instance of the SearchService class with the specified keyword.
    /// </summary>
    /// <param name="keyword">The keyword to search for in the Windows registry.</param>
    public SearchService(string keyword)
    {
        _keyword = keyword;
    }

    /// <summary>
    ///     Gets a message that summarizes the results of the registry search.
    /// </summary>
    /// <param name="matches">The number of matches found in the registry search.</param>
    /// <returns>A message string that summarizes the results of the registry search.</returns>
    public string GetServiceResultMessage(int matches)
    {
        return
            $"{matches} matches were found for the query '{_keyword}'. The search time was {_registrySearchTime.TotalSeconds:F} sec., a total of {_numberOfScannedKeys} registry keys were scanned, of which {_numberOfUnsuccessfullyScannedKeys} weren't scanned due to any problems.";
    }

    /// <summary>
    ///     Searches the Windows registry for the specified keyword and returns the results.
    /// </summary>
    /// <returns>A collection containing the search results.</returns>
    public ConcurrentBag<SearchMatch> RunRegistrySearch()
    {
        var startingTimestamp = Stopwatch.GetTimestamp();
        var searchMatches = new ConcurrentBag<SearchMatch>();
        var registryHives = new[]
        {
            RegistryHive.ClassesRoot,
            RegistryHive.CurrentUser,
            RegistryHive.LocalMachine,
            RegistryHive.Users,
            RegistryHive.CurrentConfig,
            RegistryHive.PerformanceData
        };

        Parallel.ForEach(registryHives, registryHive =>
        {
            using var baseKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
            RecursiveRegistrySearch(baseKey, searchMatches);
        });

        _registrySearchTime = Stopwatch.GetElapsedTime(startingTimestamp);
        return searchMatches;
    }

    /// <summary>
    ///     Recursively searches the registry for a given keyword.
    /// </summary>
    /// <param name="key">The registry key to search.</param>
    /// <param name="searchMatches">A collection to store the search results.</param>
    private void RecursiveRegistrySearch(RegistryKey key, ConcurrentBag<SearchMatch> searchMatches)
    {
        Parallel.ForEach(key.GetSubKeyNames(), subKeyName =>
        {
            _numberOfScannedKeys++;
            try
            {
                using var nestedKey = key.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree);
                if (nestedKey is null) return;

                if (nestedKey.SubKeyCount is 0)
                {
                    ProcessRegistryKeyForSearchMatches(nestedKey, searchMatches);
                    return;
                }

                RecursiveRegistrySearch(nestedKey, searchMatches);
            }
            catch (SecurityException)
            {
                _numberOfUnsuccessfullyScannedKeys++;
            }
            catch (UnauthorizedAccessException)
            {
                _numberOfUnsuccessfullyScannedKeys++;
            }
        });

        ProcessRegistryKeyForSearchMatches(key, searchMatches);
    }

    /// <summary>
    ///     Processes a registry key to check for search matches.
    /// </summary>
    /// <param name="key">The registry key to process.</param>
    /// <param name="searchMatches">A collection to store the search results.</param>
    private void ProcessRegistryKeyForSearchMatches(RegistryKey key, ConcurrentBag<SearchMatch> searchMatches)
    {
        var valueNames = key.GetValueNames();
        if (valueNames.Length == 0) return;

        foreach (var valueName in valueNames)
        {
            var searchMatch = new SearchMatch();

            if (ContainsKeyword(key.Name)) searchMatch.Key = key.Name;

            if (ContainsKeyword(valueName)) searchMatch.ValueName = valueName;

            if (key.GetValue(valueName) is string valueData && ContainsKeyword(valueData))
                searchMatch.ValueData = valueData;

            if (searchMatch.NumberOfAssignments is 0) continue;

            searchMatch.Key = key.Name;
            searchMatches.Add(searchMatch);
        }
    }

    /// <summary>
    ///     Checks whether the specified text contains the keyword being searched for.
    /// </summary>
    /// <param name="text">The text to check for the keyword.</param>
    /// <returns>true if the text contains the keyword; otherwise, false.</returns>
    private bool ContainsKeyword(string text)
    {
        return text.Contains(_keyword, StringComparison.InvariantCultureIgnoreCase);
    }
}