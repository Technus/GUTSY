using System.Reflection;
using System.Security;
using System.Runtime.InteropServices;

namespace GeneralUnifiedTestSystemYard.Core;

public static class IDictionaryExtensions
{
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    public static void LoadFromFolder<Thing>(
        this IDictionary<string, Thing> dictionary,
        string filter = "*.dll",
        string path = ".")
        where Thing : IIdentifiable
    {
        if (Directory.Exists(path))
        {
            foreach (var file in Directory.EnumerateFiles(path, filter))
            {
                LoadFromDLL(dictionary, file);
            }
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                LoadFromFolder(dictionary, filter, dir);
            }
        }
    }

    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="BadImageFormatException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="TargetInvocationException"></exception>
    /// <exception cref="MethodAccessException"></exception>
    /// <exception cref="MemberAccessException"></exception>
    /// <exception cref="InvalidComObjectException"></exception>
    /// <exception cref="COMException"></exception>
    /// <exception cref="TypeLoadException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    public static void LoadFromDLL<Thing>(
        this IDictionary<string, Thing> dictionary,
        string path)
        where Thing : IIdentifiable
    {
        foreach (var type in Assembly.LoadFile(Path.GetFullPath(path)).GetExportedTypes())
        {
            if (type.IsAssignableTo(typeof(Thing)))
            {
                if (Activator.CreateInstance(type) is Thing thing)
                {
                    dictionary.Add(thing.GetID(), thing);
                }
            }
        }
    }
    public static T? GetFirstByName<T>(this IDictionary<string,T> dictionary,string name) where T : IIdentifiable
    {
        return dictionary.Where(kv => kv.Key.Equals(name, StringComparison.Ordinal)).Select(kv => kv.Value).First();
    }

    public static Dictionary<string, T> GetDictionaryByName<T,D >(this IDictionary<string, T> dictionary, string name) where T : IIdentifiable
    {
        return new(dictionary.Where(kv => kv.Key.EndsWith(name)).Select(kv => kv.Value).ToDictionary(v => v.GetID()));
    }

    public static Dictionary<string, T> GetDictionaryInPath<T>(this IDictionary<string, T> dictionary, string path) where T : IIdentifiable
    {
        return new(dictionary.Where(kv => kv.Key.StartsWith(path) && !kv.Key[path.Length..].Contains('.')).Select(kv => kv.Value).ToDictionary(v => v.GetID()));
    }

    public static Dictionary<string, T> GetDictionaryInPathRecursive<T>(this IDictionary<string, T> dictionary, string path) where T : IIdentifiable
    {
        return new(dictionary.Where(kv => kv.Key.StartsWith(path)).Select(kv => kv.Value).ToDictionary(v => v.GetID()));
    }
}
