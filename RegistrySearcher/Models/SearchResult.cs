using System.Collections.Concurrent;
using System.Text.Json;
using RegistrySearcher.Enums;

namespace RegistrySearcher.Models;

/// <summary>
///     Represents a search result object that contains the results of a Windows registry search operation in a serialized
///     JSON format.
/// </summary>
public class SearchResult
{
    private readonly JsonSaveOption _jsonSaveOption;
    private readonly ConcurrentBag<SearchMatch> _searchMatches;
    private readonly string _serializedSearchMatches;

    /// <summary>
    ///     Initializes a new instance of the SearchResult class with the specified search matches and JSON serialization
    ///     option.
    /// </summary>
    /// <param name="searchMatches">The collection of search matches obtained from a Windows registry search.</param>
    /// <param name="jsonSaveOption">The JSON serialization option that specifies how to format the search result data.</param>
    public SearchResult(ConcurrentBag<SearchMatch> searchMatches, JsonSaveOption jsonSaveOption)
    {
        _searchMatches = searchMatches;
        _jsonSaveOption = jsonSaveOption;
        _serializedSearchMatches = SerializeSearchMatches();
    }

    /// <summary>
    ///     Gets the serialized JSON format of the search result.
    /// </summary>
    /// <returns>The serialized JSON string of the search result.</returns>
    public string GetSerializedSearchMatches()
    {
        return _serializedSearchMatches;
    }

    /// <summary>
    ///     Serializes the search matches into a JSON format based on the specified JSON serialization option.
    /// </summary>
    /// <returns>The serialized JSON string of the search result.</returns>
    private string SerializeSearchMatches()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        if (_jsonSaveOption == JsonSaveOption.WholeMatch)
        {
            var wholeSearchMatch = JsonSerializer.Serialize(_searchMatches, options);
            return wholeSearchMatch;
        }

        var list = new List<string>();
        foreach (var searchMatch in _searchMatches)
        {
            if (list.Contains(searchMatch.Key)) continue;

            list.Add(searchMatch.Key);
        }

        list.Sort();
        var onlyRegistryKeyNames = JsonSerializer.Serialize(list, options);
        return onlyRegistryKeyNames;
    }

    /// <summary>
    ///     Saves the search result to a JSON file and returns the file path.
    /// </summary>
    /// <returns>The file path of the saved search result.</returns>
    public async Task<string> SaveSearchResult()
    {
        var desktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var fileName = $"SearchResult #{DateTime.Now.ToBinary()}";
        var resultFilePath = Path.Combine(desktopDirectory, $"{fileName}.json");

        await using var stream = new StreamWriter(resultFilePath);
        await stream.WriteAsync(_serializedSearchMatches);
        await stream.FlushAsync();

        return resultFilePath;
    }
}