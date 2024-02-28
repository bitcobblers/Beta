using System.Runtime.CompilerServices;

namespace Beta;

public static class TaskExtension
{
    public static ConfiguredTaskAwaitable<T> NoMarshal<T>(this Task<T> awaitable) => awaitable.ConfigureAwait(false);
}