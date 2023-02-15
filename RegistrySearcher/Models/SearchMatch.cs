using System.Text.Json.Serialization;

namespace RegistrySearcher.Models;

public class SearchMatch
{
    private string _key;
    private string _valueData;
    private string _valueName;

    [JsonPropertyName("Key")]
    public string Key
    {
        get => _key;
        set
        {
            if (string.IsNullOrEmpty(_key)) NumberOfAssignments++;
            _key = value;
        }
    }

    [JsonPropertyName("ValueName")]
    public string ValueName
    {
        get => _valueName;
        set
        {
            _valueName = value;
            NumberOfAssignments++;
        }
    }

    [JsonPropertyName("ValueData")]
    public string ValueData
    {
        get => _valueData;
        set
        {
            _valueData = value;
            NumberOfAssignments++;
        }
    }

    [JsonIgnore] public int NumberOfAssignments { get; private set; }
}