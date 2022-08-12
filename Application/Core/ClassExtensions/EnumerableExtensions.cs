namespace GeneralUnifiedTestSystemYard.Core.ClassExtensions;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Lambda based For Each for each enumerable.
    ///     Will run for each element the action.
    /// </summary>
    /// <typeparam name="T">any enumerable content</typeparam>
    /// <param name="enumerable">the thing to enumerate</param>
    /// <param name="action">what to do for each</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable) action(item);
    }
}