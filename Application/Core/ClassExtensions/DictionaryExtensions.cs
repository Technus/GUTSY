using System.Reflection;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace GeneralUnifiedTestSystemYard.Core.ClassExtensions;

public static class DictionaryExtensions
{
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="SecurityException"></exception>
    public static void LoadFromFolder<T>(
        this IDictionary<string, T> dictionary,
        string filter = "*",
        string path = "")
        where T : IIdentifiable
    {
        if (path == "")
        {
            try
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
            }
            catch
            {
                path = ".";

            }
        }

        if (OperatingSystem.IsWindows())
        {
            if (!filter.ToLowerInvariant().EndsWith(".dll"))
            {
                filter += ".dll";
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            if (!filter.ToLowerInvariant().EndsWith(".so"))
            {
                filter += ".so";
            }
        }

        if (Directory.Exists(path))
        {
            foreach (var file in Directory.EnumerateFiles(path, filter))
            {
                dictionary.LoadFromFile(file);
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
    public static void LoadFromFile<T>(
        this IDictionary<string, T> dictionary,
        string path)
        where T : IIdentifiable
    {
        //Previously used Assembly.LoadFile(Path.GetFullPath(path)).GetExportedTypes(), Should fix issues with DI if ever any exists in this project
        foreach (var type in AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(path)).GetExportedTypes())
        {
            if (type.IsAssignableTo(typeof(T)))
            {
                if (Activator.CreateInstance(type) is T thing)
                {
                    dictionary.Add(thing.Identifier, thing);
                }
            }
        }
    }

    public static T? GetFirstByName<T>(this IDictionary<string, T> dictionary, string name) where T : IIdentifiable
    {
        return dictionary.Where(kv => 
                kv.Key.Equals(name, StringComparison.Ordinal))
            .Select(kv => kv.Value).FirstOrDefault();
    }

    public static Dictionary<string, T> GetDictionaryByName<T>(this IDictionary<string, T> dictionary, string name) where T : IIdentifiable
    {
        return new Dictionary<string, T>(dictionary.Where(kv => 
                kv.Key.EndsWith(name))
            .Select(kv => kv.Value).ToDictionary(v => v.Identifier));
    }

    public static Dictionary<string, T> GetDictionaryInPath<T>(this IDictionary<string, T> dictionary, string path) where T : IIdentifiable
    {
        return new Dictionary<string, T>(dictionary.Where(kv => 
                kv.Key.StartsWith(path) && !kv.Key[path.Length..].Contains('.'))
            .Select(kv => kv.Value).ToDictionary(v => v.Identifier));
    }

    public static Dictionary<string, T> GetDictionaryInPathRecursive<T>(this IDictionary<string, T> dictionary, string path) where T : IIdentifiable
    {
        return new Dictionary<string, T>(dictionary.Where(kv => 
                kv.Key.StartsWith(path))
            .Select(kv => kv.Value).ToDictionary(v => v.Identifier));
    }
}
