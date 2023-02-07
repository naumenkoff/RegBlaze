namespace RegistrySearcher.Enums;

public enum JsonSaveOption
{
    /// <summary>
    ///     Serializes the SearchMatch collection only by the name of the registry key
    /// </summary>
    OnlyKeys,

    /// <summary>
    ///     Serializes the SearchMatch collection including the registry key name, value name, and value data.
    /// </summary>
    WholeMatch
}