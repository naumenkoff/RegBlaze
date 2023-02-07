using System.Diagnostics;
using Microsoft.Win32;
using RegistrySearcher.Models;

namespace RegistrySearcher.Services;

public class SearchService
{
    private readonly string _keyword;
    private readonly List<SearchMatch> _searchMatches;
    private int _numberOfScannedKeys;
    private int _numberOfUnsuccessfullyScannedKeys;
    private TimeSpan _registrySearchTime;

    /// <param name="keyword">
    ///     The keyword, i.e. what you will be looking for and on the basis of which the search result will
    ///     be generated.
    /// </param>
    public SearchService(string keyword)
    {
        _keyword = keyword;
        _searchMatches = new List<SearchMatch>();
    }

    /// <summary>
    ///     A method that gathers all the internal information collected during the scanning process into a brief effective
    ///     message about the work done.
    /// </summary>
    public string GetServiceResultMessage()
    {
        return
            $"{_searchMatches.Count} matches were found for the query '{_keyword}'. The search time was {_registrySearchTime.TotalSeconds:F} sec., a total of {_numberOfScannedKeys} registry keys were scanned, of which {_numberOfUnsuccessfullyScannedKeys} weren't scanned due to any problems.";
    }

    /// <summary>
    ///     The method that is needed to start all scanning processes. Scanning takes place recursively, using parallelization.
    /// </summary>
    /// <returns>a list with search matches.</returns>
    public List<SearchMatch> RunRegistrySearch()
    {
        var startingTimestamp = Stopwatch.GetTimestamp();
        Parallel.Invoke(
            () => { ExecuteParallelSearch(RegistryHive.CurrentUser); },
            () => { ExecuteParallelSearch(RegistryHive.LocalMachine); },
            () => { ExecuteParallelSearch(RegistryHive.Users); },
            () => { ExecuteParallelSearch(RegistryHive.ClassesRoot); },
            () => { ExecuteParallelSearch(RegistryHive.CurrentConfig); },
            () => { ExecuteParallelSearch(RegistryHive.PerformanceData); });
        _registrySearchTime = Stopwatch.GetElapsedTime(startingTimestamp);
        return _searchMatches;
    }

    /// <summary>
    ///     Runs a parallel recursive scan based on the specified registry hive.
    /// </summary>
    /// <param name="registryHive">registry hive in which to run a recursive scan</param>
    private void ExecuteParallelSearch(RegistryHive registryHive)
    {
        using var registryKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
        RecursiveRegistrySearch(registryKey);
    }

    /// <summary>
    ///     The method is engaged in recursive parallel scanning of a given registry key for the presence of matches for a
    ///     given keyword.
    /// </summary>
    private void RecursiveRegistrySearch(RegistryKey registryKey)
    {
        Parallel.ForEach(registryKey.GetSubKeyNames(), subKeyName =>
        {
            _numberOfScannedKeys++;
            try
            {
                using var nestedKey = registryKey.OpenSubKey(subKeyName);
                if (nestedKey is null) return;

                if (nestedKey.SubKeyCount is 0)
                {
                    ProcessRegistryKeyForSearchMatches(nestedKey);
                    return;
                }

                RecursiveRegistrySearch(nestedKey);
            }
            catch (Exception)
            {
                _numberOfUnsuccessfullyScannedKeys++;
            }
        });
    }

    /// <summary>
    ///     The method processes all pairs (ValueName : ValueData) in the specified registry key and checks them to see if they
    ///     contain a keyword.
    /// </summary>
    private void ProcessRegistryKeyForSearchMatches(RegistryKey regKey)
    {
        var valueNames = regKey.GetValueNames();
        foreach (var valueName in valueNames)
        {
            var searchMatch = new SearchMatch();

            if (ContainsKeyword(regKey.Name)) searchMatch.RegistryKey = regKey.Name;

            if (ContainsKeyword(valueName)) searchMatch.ValueName = valueName;

            if (regKey.GetValue(valueName) is string valueData && ContainsKeyword(valueData))
                searchMatch.ValueData = valueData;

            if (searchMatch.NumberOfAssignments is 0) continue;

            searchMatch.RegistryKey = regKey.Name;
            _searchMatches.Add(searchMatch);
        }
    }


    /// <summary>
    ///     Checks whether the specified string contains keyword.
    /// </summary>
    /// <returns><see langword="true" /> if the keyword is contained in the text; otherwise, <see langword="false" />.</returns>
    private bool ContainsKeyword(string text)
    {
        return text.ToLower().Contains(_keyword);
    }
}