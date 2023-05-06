using System.Collections.Concurrent;
using Microsoft.Win32;
using RegBlaze.Core.Models;

namespace RegBlaze.Core.Services;

public class RegistrySearchService : IRegistrySearchService
{
    private readonly IKeywordChecker _keywordChecker;
    private readonly IRegistryKeyProcessor _registryKeyProcessor;
    private readonly ITaskTracker _taskTracker;

    public RegistrySearchService(IKeywordChecker keywordChecker, IRegistryKeyProcessor registryKeyProcessor,
        ITaskTracker taskTracker)
    {
        _registryKeyProcessor = registryKeyProcessor;
        _keywordChecker = keywordChecker;
        _taskTracker = taskTracker;
    }

    public async Task<IEnumerable<SearchMatch>> RunRegistrySearch(string keyword,
        IEnumerable<RegistryHive> registryHives)
    {
        _keywordChecker.SetKeyword(keyword);

        var searchMatches = new ConcurrentQueue<SearchMatch>();

        await Parallel.ForEachAsync(registryHives, async (registryHive, _) =>
        {
            using var baseKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
            await RecursiveRegistrySearch(baseKey, searchMatches);
            _taskTracker.CompleteTask();
        });

        return searchMatches;
    }

    private async Task RecursiveRegistrySearch(RegistryKey key, ConcurrentQueue<SearchMatch> searchMatches)
    {
        await Parallel.ForEachAsync(key.GetSubKeyNames(), async (subKeyName, _) =>
        {
            try
            {
                using var nestedKey = key.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree);
                if (nestedKey is null) return;

                if (nestedKey.SubKeyCount is 0)
                {
                    _registryKeyProcessor.ProcessRegistryKeyForSearchMatches(nestedKey, searchMatches, _keywordChecker);
                    return;
                }

                await RecursiveRegistrySearch(nestedKey, searchMatches);
            }
            catch (Exception)
            {
                // ignore
            }
        });

        _registryKeyProcessor.ProcessRegistryKeyForSearchMatches(key, searchMatches, _keywordChecker);
    }
}