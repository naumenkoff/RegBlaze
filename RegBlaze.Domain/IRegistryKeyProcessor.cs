using Microsoft.Win32;

namespace RegBlaze.Domain;

public interface IRegistryKeyProcessor
{
    void ProcessRegistryKey(RegistryKey key, List<SearchMatch> searchMatches);
}