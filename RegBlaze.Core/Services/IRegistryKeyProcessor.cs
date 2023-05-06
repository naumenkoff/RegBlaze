using System.Collections.Concurrent;
using Microsoft.Win32;
using RegBlaze.Core.Models;

namespace RegBlaze.Core.Services;

public interface IRegistryKeyProcessor
{
    void ProcessRegistryKeyForSearchMatches(RegistryKey key, ConcurrentQueue<SearchMatch> searchMatches,
        IKeywordChecker keywordChecker);
}