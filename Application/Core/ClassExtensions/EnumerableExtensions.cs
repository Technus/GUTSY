namespace GeneralUnifiedTestSystemYard.Core.ClassExtensions;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Lambda based For Each for each enumerable
    /// </summary>
    /// <typeparam name="T">any enumerable</typeparam>
    /// <param name="enumerable">the thing to enumerate</param>
    /// <param name="action">what to do for each</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable) action(item);
    }
}