using System.Text.Json;
using RegistrySearcher.Enums;

namespace RegistrySearcher.Models;

public class SearchResult
{
    private readonly JsonSaveOption _jsonSaveOption;
    private readonly List<SearchMatch> _searchMatches;
    private readonly string _serializedSearchMatches;

    /// <param name="searchMatches">the list of search matches that is returned as a result of the search service.</param>
    /// <param name="jsonSaveOption">type of serialization of search matches</param>
    public SearchResult(List<SearchMatch> searchMatches, JsonSaveOption jsonSaveOption)
    {
        _searchMatches = searchMatches;
        _jsonSaveOption = jsonSaveOption;
        _serializedSearchMatches = SerializeSearchMatches();
    }

    /// <summary>
    ///     Allows you to get a serialized string based on search matches.
    /// </summary>
    /// <returns>serialized json string</returns>
    public string GetSerializedSearchMatches()
    {
        return _serializedSearchMatches;
    }

    /// <summary>
    ///     Serializes a SearchMatch type list into a string
    /// </summary>
    /// <returns>serialized json string</returns>
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
            if (list.Contains(searchMatch.RegistryKey)) continue;

            list.Add(searchMatch.RegistryKey);
        }

        var onlyRegistryKeyNames = JsonSerializer.Serialize(list, options);
        return onlyRegistryKeyNames;
    }


    /// <summary>
    ///     Saves the scan result to the desktop directory
    /// </summary>
    /// <returns>the full name of the search results file</returns>
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