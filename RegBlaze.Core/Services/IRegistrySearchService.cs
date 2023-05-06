using Microsoft.Win32;
using RegBlaze.Core.Models;

namespace RegBlaze.Core.Services;

public interface IRegistrySearchService
{
    Task<IEnumerable<SearchMatch>> RunRegistrySearch(string keyword, IEnumerable<RegistryHive> registryHives);
}