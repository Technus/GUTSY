using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security;
using GeneralUnifiedTestSystemYard.Exceptions;

namespace GeneralUnifiedTestSystemYard.ClassExtensions;

public static class DictionaryExtensions
{
    #if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    #endif
    public static bool IsEmpty<T>(this ICollection<T> collection) => collection.Count == 0;

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
            try
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
            }
            catch
            {
                path = ".";
            }

        if (!filter.ToLowerInvariant().EndsWith(".dll")) filter += ".dll";//only load .NET *.dll, no need for *.so

        if (Directory.Exists(path))
        {
            foreach (var file in Directory.EnumerateFiles(path, filter)) dictionary.LoadFromFile(file);

            foreach (var dir in Directory.EnumerateDirectories(path)) LoadFromFolder(dictionary, filter, dir);
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
    private static void LoadFromFile<T>(
        this IDictionary<string, T> dictionary,
        string path)
        where T : IIdentifiable
    {
        //Previously used Assembly.LoadFile(Path.GetFullPath(path)).GetExportedTypes(),
        //Should fix issues with DI if ever any exists in this project
        foreach (var type in AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(path))
                     .GetExportedTypes())
            try
            {
                if (type.IsAssignableTo(typeof(T)) && !type.IsAbstract)
                    if (Activator.CreateInstance(type) is T thing)
                        dictionary.Add(thing.Identifier, thing);
            }
            catch (Exception e)
            {
                e = new DynamicLoadingException($"Failed to load: {type.ToStringFully()}", e);
                Console.WriteLine(e);
            }
    }

    public static void LoadFromAssembly<T>(
        this IDictionary<string, T> dictionary,
        Assembly? assembly = null)
        where T : IIdentifiable
    {
        assembly ??= Assembly.GetExecutingAssembly();
        foreach (var type in assembly.ExportedTypes)
            try
            {
                if (type.IsAssignableTo(typeof(T)) && !type.IsAbstract)
                    if (Activator.CreateInstance(type) is T thing)
                        dictionary.Add(thing.Identifier, thing);
            }
            catch (Exception e)
            {
                e = new DynamicLoadingException($"Failed to load: {type.ToStringFully()}", e);
                Console.WriteLine(e);
            }
    }

    public static T? GetFirstByName<T>(this IDictionary<string, T> dictionary, string name) where T : IIdentifiable
    {
        return dictionary.Where(kv =>
                kv.Key.Equals(name, StringComparison.Ordinal))
            .Select(kv => kv.Value).FirstOrDefault();
    }

    public static Dictionary<string, T> GetDictionaryByName<T>(this IDictionary<string, T> dictionary, string name)
        where T : IIdentifiable
    {
        return new Dictionary<string, T>(dictionary.Where(kv =>
                kv.Key.EndsWith(name))
            .Select(kv => kv.Value).ToDictionary(v => v.Identifier));
    }

    public static Dictionary<string, T> GetDictionaryInPath<T>(this IDictionary<string, T> dictionary, string path)
        where T : IIdentifiable
    {
        return new Dictionary<string, T>(dictionary.Where(kv =>
                kv.Key.StartsWith(path) && !kv.Key[path.Length..].Contains('.'))
            .Select(kv => kv.Value).ToDictionary(v => v.Identifier));
    }

    public static Dictionary<string, T> GetDictionaryInPathRecursive<T>(this IDictionary<string, T> dictionary,
        string path) where T : IIdentifiable
    {
        return new Dictionary<string, T>(dictionary.Where(kv =>
                kv.Key.StartsWith(path))
            .Select(kv => kv.Value).ToDictionary(v => v.Identifier));
    }
}