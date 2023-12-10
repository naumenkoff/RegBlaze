using Microsoft.Win32;
using RegBlaze.Domain;

namespace RegBlaze.Infrastructe;

public class RegistrySearchService : IRegistrySearchService
{
    private readonly IRegistryKeyProcessor _registryKeyProcessor;
    private readonly ITaskTracker _taskTracker;

    public RegistrySearchService(IRegistryKeyProcessor registryKeyProcessorFactory, ITaskTracker taskTracker)
    {
        _registryKeyProcessor = registryKeyProcessorFactory;
        _taskTracker = taskTracker;
    }

    public async Task<IEnumerable<SearchMatch>> ExecuteSearch(IEnumerable<RegistryHive> registryHives)
    {
        var searchMatches = new List<SearchMatch>();

        await Parallel.ForEachAsync(registryHives, async (registryHive, _) =>
        {
            using var baseKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
            await ProcessRegistryHive(baseKey, searchMatches);
            _taskTracker.CompleteTask();
        });

        return searchMatches;
    }

    private ValueTask ProcessRegistryHive(RegistryKey key, List<SearchMatch> searchMatches)
    {
        var stack = new Stack<RegistryKey>();
        stack.Push(key);

        while (stack.Count > 0)
        {
            var parentKey = stack.Pop();
            foreach (var subKeyName in parentKey.GetSubKeyNames())
            {
                if (!parentKey.TryOpenSubKey(subKeyName, out var childKey)) continue;

                if (childKey.SubKeyCount == 0)
                {
                    _registryKeyProcessor.ProcessRegistryKey(childKey, searchMatches);
                    childKey.Dispose();
                }
                else
                    stack.Push(childKey);
            }

            _registryKeyProcessor.ProcessRegistryKey(parentKey, searchMatches);
            parentKey.Dispose();
        }

        return ValueTask.CompletedTask;
    }
}