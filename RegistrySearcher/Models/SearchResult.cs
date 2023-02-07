using System.Text.Json;

namespace RegistrySearcher.Models;

public class SearchResult
{
    private readonly List<SearchMatch> _searchMatches;
    private readonly string _serializedSearchMatches;

    /// <param name="searchMatches">the list of search matches that is returned as a result of the search service.</param>
    public SearchResult(List<SearchMatch> searchMatches)
    {
        _searchMatches = searchMatches;
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
        var json = JsonSerializer.Serialize(_searchMatches, options);
        return json;
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