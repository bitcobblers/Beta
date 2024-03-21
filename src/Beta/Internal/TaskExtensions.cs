using System.Runtime.CompilerServices;

namespace Beta.Internal;

/// <summary>
///     Defines extensions used by the <see cref="Task" /> class.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    ///     Prevents marshaling the current context when the task is awaited.
    /// </summary>
    /// <param name="awaitable">The task being awaited.</param>
    /// <typeparam name="T">The type of task.</typeparam>
    /// <returns>The updated awaitable.</returns>
    /// <remarks>
    ///     This is to be used when awaiting tasks to that do not need to be marshaled back to the original context.
    ///     By default, TPL will marshal the continuation back to the original context. This can cause an unnecessary
    ///     performance hit when the original context is not needed.  By disabling it the continuation will simply
    ///     run on the thread that the task was executed on.
    /// </remarks>
    public static ConfiguredTaskAwaitable<T> NoMarshal<T>(this Task<T> awaitable) =>
        awaitable.ConfigureAwait(false);
}