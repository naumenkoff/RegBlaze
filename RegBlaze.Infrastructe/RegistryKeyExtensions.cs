using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace RegBlaze.Infrastructe;

public static class RegistryKeyExtensions
{
    public static bool TryOpenSubKey(this RegistryKey registryKey, string name, [MaybeNullWhen(false)] out RegistryKey key)
    {
        try
        {
            key = registryKey.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey | RegistryRights.EnumerateSubKeys);
            return key is not null;
        }
        catch (Exception)
        {
            key = null;
            return false;
        }
    }
}