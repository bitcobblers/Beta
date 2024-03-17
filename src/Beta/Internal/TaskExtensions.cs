using System.Runtime.CompilerServices;

namespace Beta.Internal;

public static class TaskExtensions
{
    public static ConfiguredTaskAwaitable<T> NoMarshal<T>(this Task<T> awaitable) =>
        awaitable.ConfigureAwait(false);
}
