using System.Text.Json.Serialization;

namespace RegistrySearcher.Models;

public class SearchMatch
{
    private string _registryKeyName;
    private string _valueData;
    private string _valueName;

    [JsonPropertyName("Registry Key")]
    public string RegistryKey
    {
        get => _registryKeyName;
        set
        {
            if (string.IsNullOrEmpty(_registryKeyName)) NumberOfAssignments++;
            _registryKeyName = value;
        }
    }

    [JsonPropertyName("Value Name")]
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