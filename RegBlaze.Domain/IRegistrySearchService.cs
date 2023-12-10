using Microsoft.Win32;

namespace RegBlaze.Domain;

public interface IRegistrySearchService
{
    Task<IEnumerable<SearchMatch>> ExecuteSearch(IEnumerable<RegistryHive> registryHives);
}