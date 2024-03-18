namespace Beta.Internal;

/// <summary>
///     Defines common extensions used by Beta.
/// </summary>
internal static class CommonExtensions
{
    /// <summary>
    ///     Performs an action on each non-null item in the collection.
    /// </summary>
    /// <param name="collection">The collection to enumerate.</param>
    /// <param name="action">The action to perform on each item.</param>
    /// <typeparam name="T">The type to iterate.</typeparam>
    public static void ForEach<T>(this IEnumerable<T?>? collection, Action<T> action)
    {
        if (collection == null)
        {
            return;
        }

        foreach (var item in collection)
        {
            if (item != null)
            {
                action(item);
            }
        }
    }
}
